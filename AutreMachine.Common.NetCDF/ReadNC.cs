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
    public class ReadNC<T, U> where T : new() where U:INumber<U>, IMultiplyOperators<U,double,double>
    {
        string path;
        T? results;
        public T? Results { get { return results; } }

        // Try to find the dimensions in the input file
        double[]? lonList = null;
        double[]? latList = null;
        long[]? timeList = null;
        Dictionary<string, float>? data = null;
        U[,,]? inter = null;

        public ReadNC(string path)
        {
            this.path = path;
            results = default(T);
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
                            var convert = data as U[,,];
                            inter = convert as U[,,];
                            if (convert != null)
                            {
                                // Ok, we can create the dictionary
                                // Usually the values are : time|lat|lon : need to confirm !!
                                /*for (int i = 0; i < convert.GetLength(0); i++)
                                {
                                    for (int j = 0; j < convert.GetLength(1); j++)
                                    {
                                        for (int k = 0; k < convert.GetLength(2); k++)
                                        {
                                            if (this.data == null)
                                                this.data = new();
                                            this.data.Add($"{k}_{j}_{i}", convert[i, j, k]);
                                        }
                                    }
                                }*/

                            }
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
            data = null;
        }

        /// <summary>
        /// Get an interpolated value from the results processed.
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public ServiceResponse<double> GetInterpolation(double lon, double lat, long time)
        {
            if (lonList == null)
                return ServiceResponse<double>.Ko("Lon is null");
            if (latList == null)
                return ServiceResponse<double>.Ko("Lat is null");
            if (timeList == null)
                return ServiceResponse<double>.Ko("Time is null");
            if (inter == null)
                return ServiceResponse<double>.Ko("Data is null");

            // Search values
            var lons = getIndexLimits(lonList, lon);
            var lats = getIndexLimits(latList, lat);
            var times = getIndexLimits(timeList, time);
            if (lons == null || lats == null || times == null)
                return ServiceResponse<double>.Ko("One of the dimension is null");

            // Wge should get 2^3 = 8 values from which to interpolate
            // Usually the values are : time|lat|lon : need to confirm !!
            var xd = (lon - lons.Value.v1) / (lons.Value.v2 - lons.Value.v1);
            var yd = (lat - lats.Value.v1) / (lats.Value.v2 - lats.Value.v1);
            var zd = (time - times.Value.v1) / (times.Value.v2 - times.Value.v1);

            var c000 = inter[times.Value.v1, lats.Value.v1, lons.Value.v1];
            var c001 = inter[times.Value.v2, lats.Value.v1, lons.Value.v1];
            var c010 = inter[times.Value.v1, lats.Value.v2, lons.Value.v1];
            var c011 = inter[times.Value.v2, lats.Value.v2, lons.Value.v1];
            var c100 = inter[times.Value.v1, lats.Value.v1, lons.Value.v2];
            var c101 = inter[times.Value.v2, lats.Value.v1, lons.Value.v2];
            var c110 = inter[times.Value.v1, lats.Value.v2, lons.Value.v2];
            var c111 = inter[times.Value.v2, lats.Value.v2, lons.Value.v2];

            var c00 = c000*(1.0 - xd) + c100*xd;
            var c01 = c001*(1.0 - xd) + c101*xd;
            var c10 = c010*(1.0 - xd) + c110*xd;
            var c11 = c011*(1.0 - xd) + c111*xd;

            var c0 = c00*(1.0-yd) + c10*yd;
            var c1 = c01*(1.0-yd) + c11*yd;

            var c = c0 * (1.0 - zd) + c1 * zd;

            return ServiceResponse<double>.Ok(c);
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
