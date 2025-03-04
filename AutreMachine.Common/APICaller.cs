using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Text.Json;
using System.Reflection.Metadata;
using System.Net.Mime;
using AutreMachine.Common;

namespace AutreMachine.Common
{
    /// <summary>
    /// Class used to call API endpoints and deserialize in return
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class APICaller<T>
    {
        /// <summary>
        /// Need to pass a Case Insensitive options
        /// </summary>
        static JsonSerializerOptions serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// Call Get without parameters
        /// </summary>
        /// <param name="client"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<ServiceResponse<T>> Get(HttpClient? client, string query)
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

            if (client.BaseAddress == null)
                return ServiceResponse<T>.Ko("HTTP BaseAdress should not be empty.");

            // Create the query in the form 
            // .../param1/param2/param3/...
            string finalQuery = CreateQuery(query, parameters);

            HttpResponseMessage response = await client.GetAsync(finalQuery);
            ServiceResponse<T>? resp = ServiceResponse<T>.Default;
            if (response.IsSuccessStatusCode)
            {

                string respStr = await response.Content.ReadAsStringAsync();

                try
                {
                    resp = JsonSerializer.Deserialize<ServiceResponse<T>>(respStr, serializerOptions);
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
                //throw new UnauthorizedAccessException();

                //throw new Exception("Error : " + response.StatusCode + " - " + content);
                return (ServiceResponse<T>.Ko("Error : " + response.StatusCode + " - " + content));
            }

            if (resp == null)
                return ServiceResponse<T>.Ko("Could not deserialize");

            return resp;
            //return ServiceResponse<T>.Ok(resp.Content);

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

        public static async Task<ServiceResponse<T>> Put<U>(HttpClient? client, string query, U? request)
        {
            if (client == null)
                return (ServiceResponse<T>.Ko("Http not provided"));

            if (client.BaseAddress == null)
                return ServiceResponse<T>.Ko("HTTP BaseAdress should not be empty.");

            HttpResponseMessage response = await client.PutAsJsonAsync(query, request);
            ServiceResponse<T>? resp = ServiceResponse<T>.Default;
            if (response.IsSuccessStatusCode)
            {
                var respStr = await response.Content.ReadAsStringAsync();
                resp = JsonSerializer.Deserialize<ServiceResponse<T>>(respStr, serializerOptions);

            }
            else
                throw new Exception("Error : " + response.StatusCode + " - " + await response.Content.ReadAsStringAsync());

            if (resp == null)
                return ServiceResponse<T>.Ko("Could not deserialize.");

            return resp;
        }

        /// <summary>
        /// Send a POST message 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="client"></param>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <param name="isResponseServiceResponse">True (by default) if it uses the ServiceResponse mechanism ; otherwise, pass false</param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static async Task<ServiceResponse<T>> Post<U>(HttpClient? client, string query, U request, bool isResponseServiceResponse = true, string? contentType = null)
        {
            if (client == null)
                return (ServiceResponse<T>.Ko("Http not provided"));

            if (client.BaseAddress == null)
                return ServiceResponse<T>.Ko("HTTP BaseAdress should not be empty.");

            HttpResponseMessage? response = null;
            if (client.BaseAddress == null)
                return ServiceResponse<T>.Ko("HTTP BaseAdress should not be empty.");

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

                    if (request != null)
                    {
                        if (request.GetType().IsPrimitive || typeof(U) == typeof(string))
                            req.Content = new StringContent(request.ToString() ?? "",
                            Encoding.UTF8,
                            contentType);
                        else if (request is FormUrlEncodedContent)
                            req.Content = request as FormUrlEncodedContent;
                        else
                            req.Content = new StringContent(JsonSerializer.Serialize(request, serializerOptions),
                            Encoding.UTF8,
                            contentType);
                    }



                    response = await client.SendAsync(req);
                }
                else
                {
                    client.DefaultRequestHeaders
                        .Accept
                        .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var req = new HttpRequestMessage(HttpMethod.Post, query);
                    req.Content = new StringContent(JsonSerializer.Serialize(request, serializerOptions),
                        Encoding.UTF8,
                        "application/json");

                    response = await client.SendAsync(req);
                }

                ServiceResponse<T>? resp = ServiceResponse<T>.Default;
                if (response != null && response.IsSuccessStatusCode)
                {
                    var respStr = await response.Content.ReadAsStringAsync();

                    // Try to deserialize
                    if (isResponseServiceResponse)
                        resp = JsonSerializer.Deserialize<ServiceResponse<T>?>(respStr, serializerOptions);
                    else
                    {
                        // Try on T
                        var content = JsonSerializer.Deserialize<T?>(respStr, serializerOptions);
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


        public static async Task<ServiceResponse<T>> Delete(HttpClient? client, string query, params object[] parameters)
        {
            if (client == null)
                return (ServiceResponse<T>.Ko("Http not provided"));

            if (client.BaseAddress == null)
                return ServiceResponse<T>.Ko("HTTP BaseAdress should not be empty.");

            string finalQuery = CreateQuery(query, parameters);

            HttpResponseMessage response = await client.DeleteAsync(finalQuery);
            ServiceResponse<T>? resp = ServiceResponse<T>.Default;
            if (response != null && response.IsSuccessStatusCode)
            {
                var respStr = await response.Content.ReadAsStringAsync();
                resp = JsonSerializer.Deserialize<ServiceResponse<T>>(respStr, serializerOptions);
            }
            else
            {
                return ServiceResponse<T>.Ko("Error : " + response.StatusCode);
            }

            return resp;
        }

    }



}
