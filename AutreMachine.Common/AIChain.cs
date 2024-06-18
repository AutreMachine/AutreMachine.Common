using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AutreMachine.Common
{
    /// <summary>
    /// Tools to chain several request to a LLM, process the output and feed it to the new input
    /// Note : for starter, use LM Studio
    /// </summary>
    public class AIChain
    {
        HttpClient? _client;
        List<Step>? _steps;

        public AIChain(string localLMStudioAddress = "http://localhost:1234/v1/")
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(localLMStudioAddress);
        }

        public AIChain(HttpClient client)
        {
            _client = client;
        }
        #region Chain questions
        public async Task<ServiceResponseEmpty> AddQuestion<T>(AIMessage[] messages, float temperature = 0.0f) where T:IConvertible
        {
            if (_steps == null)
                _steps = new List<Step>();

            _steps.Add(new Step {Messages = messages, Temperature = temperature, ResponseType = typeof(T)});

            return ServiceResponseEmpty.Ok();
        }

        
        #endregion

        #region Single Question
        /// <summary>
        /// Ask a single question and deserialize result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messages">Provide messages for user, system,...</param>
        /// <param name="temperature"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<T>> AskQuestion<T>(AIMessage[] messages, float temperature = 0.0f) where T:IConvertible
        {
            if (_client == null)
                return ServiceResponse<T>.Ko("No HTTP Client provided.");

            // Check roles are different in messages
            var nbDistinct = messages.Select(x => x.role.Trim().ToLower()).Distinct().Count();
            if (nbDistinct < messages.Length)
                return ServiceResponse<T>.Ko("Roles should be different in the input messages.");

            var url = Path.Combine("chat", "completions");
            var req = new LocalLMStudioAIRequest
            {
                temperature = 0,
                max_tokens = 1024  // to avoid infinite loops
            };
            req.messages.AddRange(messages);
            var resp = await APICaller<LocalLMStudioAIResponse>.Post<LocalLMStudioAIRequest>(_client, url, req, false);
            // return the first choice
            if (resp != null)
            {
                // Try to deserialize
                var str = resp.Content.choices.FirstOrDefault()!.message?.content;
                if (str == null)
                    return ServiceResponse<T>.Ko("Could not deserialize : Result empty");
                T? result = default(T);
                if (str is not string)
                    result = JsonSerializer.Deserialize<T>(str);
                else
                    result = (T)Convert.ChangeType(str, typeof(T));

                if (result == null)
                    return ServiceResponse<T>.Ko("Could not deserialize : " + str);

                return ServiceResponse<T>.Ok(result);
            }

            return ServiceResponse<T>.Ko("No response from LLM");
        }
        #endregion
    }

    public class AskClass
    {
        public string first { get; set; }
        public string last { get; set; }
        public int age { get; set; }
    }

    public class AnswerClass
    {
        public string answer { get; set; }
        public int squareage { get; set; }
    }

    public class LocalLMStudioAIRequest
    {
        public List<AIMessage> messages { get; set; } = new List<AIMessage>();
        public float temperature { get; set; }
        public int max_tokens { get; set; }
        public bool stream { get; set; } = false;
    }



    public class Step
    {
        public AIMessage[] Messages { get; set; }
        public float Temperature {get; set; }
        public Type ResponseType {get; set; }
    }


    public class AIMessage
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class LocalLMStudioAIResponse
    {
        public string id { get; set; }
        [JsonPropertyName("object")]
        public string obj { get; set; }
        public long created { get; set; }
        public string model { get; set; }
        public List<LocalLMStudioAIChoiceResponse> choices { get; set; } = new List<LocalLMStudioAIChoiceResponse>();

        public LocalLMStudioAIUsageResponse usage { get; set; }
    }
    public class LocalLMStudioAIChoiceResponse
    {
        public int index { get; set; }
        public AIMessage message { get; set; }
        public string finish_reason { get; set; }
    }
    public class LocalLMStudioAIUsageResponse
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }

    public class LocalLMStudioEmbeddingRequest
    {
        public string input { get; set; }
        public string model { get; set; } = "nomic-ai/nomic-embed-text-v1.5-GGUF"; ///nomic-embed-text-v1.5.Q5_K_M.gguf";
    }
}
