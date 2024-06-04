using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Text.Json;
using Polly;
using Polly.Timeout;

namespace AutreMachine.Common
{
    public class APICaller<T>
    {
        /// <summary>
        /// Call Get without parameters
        /// </summary>
        /// <param name="client"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<ServiceResponse<T>> Get(HttpClient client, string query)
        {
            return await Get(client, query, new object[] { });
        }

        /// <summary>
        /// Call Get with parameters - and encode parameters
        /// </summary>
        /// <param name="client"></param>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public static async Task<ServiceResponse<T>> Get(HttpClient? client, string query, params object[] parameters)
        {
            if (client == null)
                return (ServiceResponse<T>.Ko("Http not provided"));

            // Create the query
            //string finalQuery = Path.Combine(parts);
            string finalQuery = CreateQuery(query, parameters);

            HttpResponseMessage response = await client.GetAsync(finalQuery);
            ServiceResponse<T>? resp = ServiceResponse<T>.Default;
            if (response.IsSuccessStatusCode)
            {
                /*var timeoutPolicy =
                    Policy.TimeoutAsync(
                        TimeSpan.FromSeconds(60),
                        TimeoutStrategy.Optimistic,
                        async (context, timespan, task) =>
                        {
                            //write here the cancel request 
                            Console.WriteLine("Request canceled due to timeout.");
                        });*/

                string respStr = "";
                //var policyResult = await timeoutPolicy.ExecuteAndCaptureAsync(async () => {
                respStr = await response.Content.ReadAsStringAsync();
                //});

                // Warning !
                // we use a Json Serializer Settings to cope with the 3 classes that implement IAssetRate,
                // as NewtonSoft is not able to Deserialize an interface (basically it sends the concrete type)
                // resp = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(respStr, new JsonSerializerSettings()
                // {
                //    TypeNameHandling = TypeNameHandling.Auto
                // });
                try
                {
                    resp = JsonSerializer.Deserialize<ServiceResponse<T>>(respStr);
                }
                catch (Exception ex)
                {
                    return (ServiceResponse<T>.Ko(ex.Message));
                    //throw new Exception("Error during parsing : " + ex.Message);
                }
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return (ServiceResponse<T>.Ko("Unauthorized"));
                }
                //throw new UnauthorizedAccessException();

                //throw new Exception("Error : " + response.StatusCode + " - " + content);
                return (ServiceResponse<T>.Ko("Error : " + response.StatusCode + " - " + content));
            }

            if (resp == null)
                return ServiceResponse<T>.Ko("Could not deserialize");

            return ServiceResponse<T>.Ok(resp.Content);

        }

        public static string CreateQuery(string query, params object[] parameters)
        {
            if (parameters.Length == 0)
                return query;

            // Create the query
            string[] parts = new string[parameters.Length + 1];
            parts[0] = query;
            int nb = 1;
            string finalQuery = query;
            if (finalQuery.EndsWith("/"))
                finalQuery = finalQuery.Substring(0, finalQuery.Length - 1);

            foreach (var param in parameters)
            {
                if (param != null && param.ToString() != "")
                {
                    var part = "";
                    if (param is float)
                    {
                        // Pass the separator to .
                        //part = param.ToString();
                        var test = (float)Convert.ToDecimal(param, CultureInfo.GetCultureInfo("en-US"));
                        part = test.ToString().Replace(",", "."); // HttpUtility.UrlEncode(test.ToString());
                    }
                    else
                        part = HttpUtility.UrlEncode(param.ToString());
                    if (part != null)
                    {
                        parts[nb++] = part;
                        finalQuery = string.Concat(finalQuery, "/" + part);
                    }
                }
            }

            return finalQuery;
        }

        public static async Task<ServiceResponse<T>> Put<U>(HttpClient client, string query, U? request)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(query, request);
            ServiceResponse<T>? resp = ServiceResponse<T>.Default;
            if (response.IsSuccessStatusCode)
            {
                var respStr = await response.Content.ReadAsStringAsync();
                //resp = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(respStr);
                resp = JsonSerializer.Deserialize<ServiceResponse<T>>(respStr);

            }
            else
                throw new Exception("Error : " + response.StatusCode + " - " + await response.Content.ReadAsStringAsync());

            if (resp == null)
                return ServiceResponse<T>.Ko("Could not deserialize.");

            return resp;
        }

        public static async Task<ServiceResponse<T>> Post<U>(HttpClient client, string query, U request, double? timeout = null, Func<Context, TimeSpan, Task, string>? timeoutLambda = null)
        {
            // New policy to avoid 00:00:10 timeout
            // Useful when calling AI services (ex : LM Studio)
            HttpResponseMessage? response = null;
            if (timeout != null && (timeout < 0 || timeout > 500))
                return ServiceResponse<T>.Ko("Timeout should be between 0 and 500 secoinds.");

            if (timeout != null)
            {
                var timeoutPolicy =
                        Policy.TimeoutAsync(
                            TimeSpan.FromSeconds(timeout.Value),
                            TimeoutStrategy.Optimistic,
                            async (context, timespan, task) =>
                            {
                                //write here the cancel request 
                                if (timeoutLambda != null)
                                    timeoutLambda(context, timespan, task);
                                //Console.WriteLine("Request canceled due to timeout.");
                            });


                var policyResult = await timeoutPolicy.ExecuteAndCaptureAsync(async () =>
                {
                    response = await client.PostAsJsonAsync(query, request);
                });
            }
            else
                response = await client.PostAsJsonAsync(query, request);

            ServiceResponse<T>? resp = ServiceResponse<T>.Default;
            if (response != null && response.IsSuccessStatusCode)
            {
                var respStr = await response.Content.ReadAsStringAsync();
                //resp = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(respStr);
                resp = JsonSerializer.Deserialize<ServiceResponse<T>?>(respStr);
            }
            else
                throw new Exception("Error : " + response?.StatusCode);

            if (resp == null)
                return ServiceResponse<T>.Ko("Could not deserialize.");

            return resp;
        }

        public static async Task<T?> Post(HttpClient client, string query, string content, string contentType)
        {
            // Check : https://stackoverflow.com/questions/10679214/how-to-set-the-content-type-header-for-an-httpclient-request
            client.DefaultRequestHeaders
                .Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(contentType));
            var req = new HttpRequestMessage(HttpMethod.Post, query);
            //var str = HttpUtility.UrlEncode(JsonSerializer.Serialize(request));
            req.Content = new StringContent(content,
                Encoding.UTF8,
                contentType);

            var response = await client.SendAsync(req);

            //HttpResponseMessage response = await client.PostAsJsonAsync(query, request);
            T? resp = default(T);
            if (response.IsSuccessStatusCode)
            {
                var respStr = await response.Content.ReadAsStringAsync();
                //resp = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(respStr);
                resp = JsonSerializer.Deserialize<T>(respStr);
            }
            else
                throw new Exception("Error : " + response.StatusCode + " - " + await response.Content.ReadAsStringAsync());

            return resp;
        }

        public static async Task<T?> Delete(HttpClient client, string query, params object[] parameters)
        {
            string finalQuery = CreateQuery(query, parameters);

            HttpResponseMessage response = await client.DeleteAsync(finalQuery);
            T? resp = default(T);
            if (response.IsSuccessStatusCode)
            {
                var respStr = await response.Content.ReadAsStringAsync();
                //resp = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(respStr);
                resp = JsonSerializer.Deserialize<T>(respStr);
            }
            else
                throw new Exception("Error : " + response.StatusCode);

            return resp;
        }

    }



}
