using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AutreMachine.Common
{
    public interface IServiceResponse
    {
        bool Succeeded { get; set; }
        string Message { get; set; }
    }

    public class ServiceResponse<T> : IServiceResponse
    {
        [JsonPropertyName("succeeded")]
        public bool Succeeded { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public T? Content { get; set; } = default(T);

        public static ServiceResponse<T> Ok(T? content)
        {
            return new()
            {
                Succeeded = true,
                Message = "",
                Content = content
            };
        }

        public static ServiceResponse<T> Ko(string message)
        {
            return new()
            {
                Succeeded = false,
                Message = message,
                Content = default(T)
            };
        }

        [JsonIgnore]
        public static ServiceResponse<T> Default { get { return new ServiceResponse<T> { Content = default(T) }; } }
    }

    public class ServiceResponseEmpty : IServiceResponse
    {

         [JsonPropertyName("succeeded")]
        public bool Succeeded { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;


        public static ServiceResponseEmpty Ok()
        {
            return new()
            {
                Succeeded = true,
                Message = ""
                
            };
        }

        public static ServiceResponseEmpty Ko(string message)
        {
            return new()
            {
                Succeeded = false,
                Message = message
            };
        }
    }



}
