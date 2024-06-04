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
using System.Reflection.Metadata;
using System.Net.Mime;

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

            try
            {
                client.DefaultRequestHeaders.Add("Host", "AutreMachine");
                //client.DefaultRequestVersion = new Version("2.0");
                
                // Create the query
                //string finalQuery = Path.Combine(parts);
                string finalQuery = CreateQuery(query, parameters);

                //HttpResponseMessage response = await client.GetAsync((finalQuery);
                var request = new HttpRequestMessage(HttpMethod.Get,
                    finalQuery);
                request.Headers.Add("Accept", "*/*");
                request.Headers.Add("User-Agent", "AutreMachine-Common");

                //var req = new HttpRequestMessage(HttpMethod.Get, finalQuery);
                //req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36");

                var response = await client.SendAsync(request);

                    //var str = HttpUtility.UrlEncode(JsonSerializer.Serialize(request));

                ServiceResponse<T>? resp = ServiceResponse<T>.Default;
                if (response.IsSuccessStatusCode)
                {

                    string respStr = await response.Content.ReadAsStringAsync();


                    try
                    {
                        resp = JsonSerializer.Deserialize<ServiceResponse<T>>(respStr);
                    }
                    catch (Exception ex)
                    {
                        return (ServiceResponse<T>.Ko(ex.Message));
                    }
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return (ServiceResponse<T>.Ko("Unauthorized"));
                    }
                    return (ServiceResponse<T>.Ko("Error : " + response.StatusCode + " - " + content));
                }

                if (resp == null)
                    return ServiceResponse<T>.Ko("Could not deserialize");

                return ServiceResponse<T>.Ok(resp.Content);
            }
            catch (Exception ex)
            {
                return ServiceResponse<T>.Ko("Error : " + ex.Message);
            }

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
                //if (request is IServiceResponse)
                    resp = JsonSerializer.Deserialize<ServiceResponse<T>>(respStr);
                //else
                  //  resp = JsonSerializer.Deserialize<T>(respStr);

            }
            else
                throw new Exception("Error : " + response.StatusCode + " - " + await response.Content.ReadAsStringAsync());

            if (resp == null)
                return ServiceResponse<T>.Ko("Could not deserialize.");

            return resp;
        }

        public static async Task<ServiceResponse<T>> Post<U>(HttpClient client, string query, U request, bool isResponseServiceResponse = true, string? contentType = null) 
        {
            HttpResponseMessage? response = null;

            try
            {
                // Specific case if contentType
                // Check : https://stackoverflow.com/questions/10679214/how-to-set-the-content-type-header-for-an-httpclient-request
                if (contentType != null)
                {
                    client.DefaultRequestHeaders
                        .Accept
                        .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(contentType));
                    var req = new HttpRequestMessage(HttpMethod.Post, query);
                    //var str = HttpUtility.UrlEncode(JsonSerializer.Serialize(request));
                    req.Content = new StringContent(JsonSerializer.Serialize(request),
                        Encoding.UTF8,
                        contentType);

                    response = await client.SendAsync(req);
                }
                else
                {
                    client.DefaultRequestHeaders
                        .Accept
                        .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var req = new HttpRequestMessage(HttpMethod.Post, query);
                    //var str = HttpUtility.UrlEncode(JsonSerializer.Serialize(request));
                    req.Content = new StringContent(JsonSerializer.Serialize(request),
                        Encoding.UTF8,
                        "application/json");

                    response = await client.SendAsync(req);
                    //response = await client.PostAsJsonAsync(query, request);
                }

                ServiceResponse<T>? resp = ServiceResponse<T>.Default;
                if (response != null && response.IsSuccessStatusCode)
                {
                    var respStr = await response.Content.ReadAsStringAsync();
                    
                    // ry to deserialize
                    if (isResponseServiceResponse)
                        resp = JsonSerializer.Deserialize<ServiceResponse<T>?>(respStr);
                    else
                    {
                        // Try on T
                        var content = JsonSerializer.Deserialize<T?>(respStr);
                        if (content == null)
                            resp = ServiceResponse<T>.Ko("Could not deserialize");
                        else
                            resp = ServiceResponse<T>.Ok(content);
                    }
                }
                else
                    return ServiceResponse<T>.Ko("Error : " + response?.StatusCode);

                if (resp == null)
                    return ServiceResponse<T>.Ko("Could not deserialize.");

                return resp;
            }
            catch (Exception ex)
            {
                return ServiceResponse<T>.Ko("Error : " + ex.Message);
            }
        }


        public static async Task<ServiceResponse<T>> Delete(HttpClient client, string query, params object[] parameters)
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
            {
                return ServiceResponse<T>.Ko("Error : " + response.StatusCode);
            }

            return ServiceResponse<T>.Ok(resp);
        }

    }



}
