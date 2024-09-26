using System.Numerics;
using System.Reflection;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Imperative;

namespace AutreMachine.Common.NetCDF
{
    /// <summary>
    /// T is the output class we want to create, U is the format of the data in the file (ex: single)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class ReadNC<T, U> where T : new() where U:INumber<U>
    {
        string path;
        T? results;
        public T? Results { get { return results; } }

        // Try to find the dimensions in the input file
        double[]? lonList = null;
        double[]? latList = null;
        long[]? timeList = null;
        //Dictionary<string, float>? data = null;
        U[,,]? data = null;
        // A multiply function between U and double
        Func<U, double, U> _multiply;

        /// <summary>
        /// Instantiates the ReadNC.
        /// </summary>
        /// <param name="path">Path of the .nc file to load</param>
        /// <param name="multiply">a Func to guide how to multiply the U data type and double</param>
        public ReadNC(string path, Func<U, double, U> multiply)
        {
            this.path = path;
            results = default(T);
            _multiply = multiply;
        }

        /// <summary>
        /// Reads a nc file and launch extraction process, then stores in results
        /// </summary>
        /// <param name="path"></param>
        public void Process(string? lonColumnName = null, string? latColumnName = null, string? timeColumnName = null, string? dataColumnName = null)
        {
            // Opens the file
            using (DataSet ds = DataSet.Open($"msds:nc?file={path}&openMode=readOnly"))
            {

                foreach (var (key, value) in ds.Metadata)
                    Console.WriteLine($"{key} : {value}");
                Console.WriteLine("");
                Console.WriteLine("Variables : ");
                Console.WriteLine("-----------");

                foreach (Variable v in ds.Variables)
                    Console.WriteLine($"{v.Name} - {v.TypeOfData} : {v.ToString()}");

                // The returnbed object
                var ret = new T();

                // Get data for all fields of target object
                var t = typeof(ReadDimension);
                var m = t.GetMethod("ExtractOneDimension");
                var fields = typeof(T).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).ToList();
                foreach (var field in fields)
                {
                    var type = field.PropertyType; //.GetType();
                    var call = m.MakeGenericMethod(type);
                    var rd = new ReadDimension();
                    try
                    {
                        var data = call.Invoke(rd, new object[] { ds, field.Name });

                        if (lonColumnName != null && field.Name == lonColumnName)
                        {
                            // Try to cast lon in a list of float
                            lonList = data as double[];
                        }
                        if (latColumnName != null && field.Name == latColumnName)
                        {
                            // Try to cast lon in a list of float
                            latList = data as double[];
                        }
                        if (timeColumnName != null && field.Name == timeColumnName)
                        {
                            // Try to cast lon in a list of float
                            timeList = data as long[];
                        }
                        if (dataColumnName != null && field.Name == dataColumnName)
                        {
                            // construct the dictionary
                            this.data = data as U[,,];
                            
                        }

                        // Ok, now set the corresponding field on the target object
                        ret.GetType().InvokeMember(field.Name,
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                            Type.DefaultBinder,
                            ret,
                            new object[1] { data });
                    }
                    catch (TargetInvocationException ex)
                    {
                        var msg = ex.Message;
                        if (ex.InnerException != null)
                            msg = ex.InnerException.Message;

                        // could not dinf the good column 
                        throw new Exception("Column " + field.Name + " not found in the input file : " + msg);
                    }
                }

                results = ret;

            }

        }

        public void ClearResults()
        {
            results = default(T);
            lonList = null;
            latList = null;
           //data = null;
        }

        /// <summary>
        /// Get an interpolated value from the results processed.
        /// Check : https://en.wikipedia.org/wiki/Trilinear_interpolation
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public ServiceResponse<U> GetInterpolation(double lon, double lat, long time)
        {
            if (lonList == null)
                return ServiceResponse<U>.Ko("Lon is null");
            if (latList == null)
                return ServiceResponse<U>.Ko("Lat is null");
            if (timeList == null)
                return ServiceResponse<U>.Ko("Time is null");
            if (data == null)
                return ServiceResponse<U>.Ko("Data is null");

            // Search values
            var lons = getIndexLimits(lonList, lon);
            var lats = getIndexLimits(latList, lat);
            var times = getIndexLimits(timeList, time);
            if (lons == null || lats == null || times == null)
                return ServiceResponse<U>.Ko("One of the dimension is null");

            // Wge should get 2^3 = 8 values from which to interpolate
            // Usually the values are : time|lat|lon : need to confirm !!
            var xd = (lon - lonList[lons.Value.v1]) / (lonList[lons.Value.v2] - lonList[lons.Value.v1]);
            var yd = (lat - latList[lats.Value.v1]) / (latList[lats.Value.v2] - latList[lats.Value.v1]);
            var zd = (time - timeList[times.Value.v1]) / (timeList[times.Value.v2] - timeList[times.Value.v1]);

            Console.WriteLine($"xd:{xd}");
            Console.WriteLine($"yd:{yd}");
            Console.WriteLine($"zd:{zd}");

            var c000 = data[times.Value.v1, lats.Value.v1, lons.Value.v1];
            var c001 = data[times.Value.v2, lats.Value.v1, lons.Value.v1];
            var c010 = data[times.Value.v1, lats.Value.v2, lons.Value.v1];
            var c011 = data[times.Value.v2, lats.Value.v2, lons.Value.v1];
            var c100 = data[times.Value.v1, lats.Value.v1, lons.Value.v2];
            var c101 = data[times.Value.v2, lats.Value.v1, lons.Value.v2];
            var c110 = data[times.Value.v1, lats.Value.v2, lons.Value.v2];
            var c111 = data[times.Value.v2, lats.Value.v2, lons.Value.v2];

            Console.WriteLine($"c000:{c000}");
            Console.WriteLine($"c001:{c001}");
            Console.WriteLine($"c010:{c010}");
            Console.WriteLine($"c011:{c011}");
            Console.WriteLine($"c100:{c100}");
            Console.WriteLine($"c101:{c101}");
            Console.WriteLine($"c110:{c110}");
            Console.WriteLine($"c111:{c111}");

            var c00 = _multiply(c000, (1.0-xd))+ _multiply(c100,xd);
            var c01 = _multiply(c001, (1.0-xd))+ _multiply(c101,xd);
            var c10 = _multiply(c010, (1.0-xd))+ _multiply(c110,xd);
            var c11 = _multiply(c011, (1.0-xd))+ _multiply(c111,xd);

            var c0 = _multiply(c00, (1.0-yd)) + _multiply(c10, yd);
            var c1 = _multiply(c01, (1.0-yd)) + _multiply(c11, yd);

            var c = _multiply(c0, (1.0 - zd)) + _multiply(c1, zd);

            return ServiceResponse<U>.Ok(c);
        }

        private (int v1, int v2)? getIndexLimits<V>(V[] input, V value) where V : IComparable<V>, IEquatable<V>
        {
            if (input.Length == 0)
                return null;

            V? v1 = input.OrderByDescending(x => x).Where(x => x.CompareTo(value) <= 0).FirstOrDefault();
            V? v2 = input.OrderBy(x => x).Where(x => x.CompareTo(value) > 0).FirstOrDefault();

            if (v1 == null || v2 == null)
                return null;

            // Get the index
            var i1 = input.ToList().FindIndex(x => x.CompareTo(v1) == 0);
            var i2 = input.ToList().FindIndex(x => x.CompareTo(v2) == 0);

            if (i1 < 0 || i2 < 0)
                return null;
            if (i1 == i2)
            {
                if (i1 == 0)
                    return (0, 1);
                if (i2 == input.Length - 1)
                    return (input.Length - 1, input.Length);
            }

            return (i1, i2);

        }



        /// <summary>
        /// Display metadata and variables informaiton
        /// </summary>
        /// <param name="path"></param>
        public void DisplayConsole()
        {
            // Opens the file
            using (DataSet ds = DataSet.Open($"msds:nc?file={path}&openMode=readOnly"))
            {

                foreach (var (key, value) in ds.Metadata)
                    Console.WriteLine($"{key} : {value}");
                Console.WriteLine("");
                Console.WriteLine("Variables : ");
                Console.WriteLine("-----------");

                foreach (Variable v in ds.Variables)
                    Console.WriteLine($"{v.Name} - {v.TypeOfData} : {v.ToString()}");
            }
        }
    }

    public class ReadDimension
    {
        public T? ExtractOneDimension<T>(DataSet source, string fieldName)
        {
            return source.GetData<T>(fieldName);
        }
    }

}
