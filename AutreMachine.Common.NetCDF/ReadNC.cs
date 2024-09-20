using System.Reflection;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Imperative;

namespace AutreMachine.Common.NetCDF
{
    public class ReadNC<T> where T : new()
    {
        string path;
        public ReadNC(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Reads a nc file and launch extraction process
        /// </summary>
        /// <param name="path"></param>
        public T? Process()
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

                return ret;

            }

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
