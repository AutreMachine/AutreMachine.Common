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
using AutreMachine.Common;

namespace AutreMachine.Common
{
    /// <summary>
    /// Class used to call API endpoints and deserialize in return
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class APICallerEmpty
    {
        /// <summary>
        /// Need to pass a Case Insensitive options
        /// </summary>
        static JsonSerializerOptions serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };


        public static async Task<ServiceResponseEmpty> Put<U>(HttpClient client, string query, U? request)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(query, request);

            if (response.IsSuccessStatusCode)
            {
                return ServiceResponseEmpty.Ok();
            }
            else
                return ServiceResponseEmpty.Ko("Error : " + response.StatusCode + " - " + await response.Content.ReadAsStringAsync());

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
        public static async Task<ServiceResponseEmpty> Post<U>(HttpClient client, string query, U request, bool isResponseServiceResponse = true, string? contentType = null)
        {
            HttpResponseMessage? response = null;
            if (client.BaseAddress == null)
                return ServiceResponseEmpty.Ko("HTTP BaseAdress should not be empty.");

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
                            req.Content = new StringContent(request.ToString()??"",
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

                if (response != null && response.IsSuccessStatusCode)
                {
                    return ServiceResponseEmpty.Ok();
                }
                else
                    return ServiceResponseEmpty.Ko("Error : " + response?.StatusCode);

            }
            catch (Exception ex)
            {
                return ServiceResponseEmpty.Ko("Error : " + ex.Message);
            }
        }


        public static async Task<ServiceResponseEmpty> Delete(HttpClient client, string query, params object[] parameters)
        {
            string finalQuery = APICaller<bool>.CreateQuery(query, parameters);

            HttpResponseMessage response = await client.DeleteAsync(finalQuery);
            
            if (response.IsSuccessStatusCode)
            {
                return ServiceResponseEmpty.Ok();
            }
            else
            {
                return ServiceResponseEmpty.Ko("Error : " + response.StatusCode);
            }

        }

    }



}
