using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AutreMachine.Common.Samples.APICaller
{
    public class APICallerLocalTest
    {
        public async Task<ServiceResponse<string>> AskQuestion(AIMessage[] messages, float temperature = 0.7f)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:1234/v1/");
            var url = Path.Combine("chat", "completions");
            var req = new LocalLMStudioAIRequest
            {
                temperature = 0,
                max_tokens = 1024  // to avoid infinite loops
            };
            req.messages.AddRange(messages);
            var resp = await APICaller<LocalLMStudioAIResponse>.Post<LocalLMStudioAIRequest>(client, url, req, false);
            // return the first choice
            if (resp != null && resp.Content?.choices?.FirstOrDefault() != null)
            {
                return ServiceResponse<string>.Ok(resp.Content.choices.FirstOrDefault()!.message.content);
            }

            return ServiceResponse<string>.Ko("No response from Local OpenAI");
        }

        public async Task<ServiceResponse<string>> AnswerName(string name)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://www.google.com");
            var resp = await APICaller<string>.Get(client, "search?q=test");

            return resp;
        }
    }

    
    public class LocalLMStudioAIRequest
    {
        public List<AIMessage> messages { get; set; } = new List<AIMessage>();
        public float temperature { get; set; }
        public int max_tokens { get; set; }
        public bool stream { get; set; } = false;
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
