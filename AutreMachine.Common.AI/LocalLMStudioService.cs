using Azure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rystem.OpenAi;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Embedding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace AutreMachine.Common.AI
{
    public class LocalLMStudioService : IAIService
    {
        static string BASE_ADDRESS = "http://localhost:1234/v1/";
        static string AGENT = "LocalLMStudioAPITool";


        IHttpClientFactory _clientFactory;
        HttpClient _client;

        public LocalLMStudioService(IHttpClientFactory clientFactory, string? baseAddress = null)
        {
            _clientFactory = clientFactory;
            _client = _clientFactory.CreateClient();

            _client.BaseAddress = new Uri(baseAddress == null ? BASE_ADDRESS : baseAddress);
            _client.DefaultRequestHeaders.Add("user-agent", AGENT);

            _client.Timeout = Timeout.InfiniteTimeSpan;

        }

        public async Task<ServiceResponse<string>> AskQuestion(string assistantContent, string userContent)
        {
            AIMessage[] messages = [new AIMessage
            {
                role = "assistant",
                content = assistantContent
            },
            new AIMessage
            {
                role = "user",
                content = userContent
            }];

            return await AskQuestion(messages);

        }

        public async Task<ServiceResponse<string>> AskQuestion(AIMessage[] messages, float temperature = 0.7f)
        {
            if (messages.Length == 0)
                return ServiceResponse<string>.Ko("No message provided.");

            var url = Path.Combine("chat", "completions");

            var req = new LocalLMStudioAIRequest
            {
                temperature = temperature,
                max_tokens = 1024  // to avoid infinite loops
            };
            req.messages.AddRange(messages);

            var resp = await APICaller<LocalLMStudioAIResponse>.Post(_client, url, req, false);

            // return the first choice
            if (resp != null && resp.Content?.choices?.FirstOrDefault() != null)
            {
                return ServiceResponse<string>.Ok(resp.Content.choices.FirstOrDefault()!.message.content);
            }

            return ServiceResponse<string>.Ko("No response from Local OpenAI");

        }



        public async Task<ServiceResponse<EmbeddingResult>> CalculateEmbeddings(string text)
        {
            /* Embeddings
             * 
             * curl http://localhost:1234/v1/embeddings \
  -H "Content-Type: application/json" \
  -d '{
    "input": "Your text string goes here",
    "model": "model-identifier-here"
  }'
             */

            var url = "embeddings";

            // check : https://lmstudio.ai/docs/text-embeddings
            var req = new LocalLMStudioEmbeddingRequest
            {
                input = text
            };

            var resp = await APICaller<EmbeddingResult>.Post(_client, url, req);

            // return the first choice
            if (resp != null)
            {
                return resp;
            }

            return ServiceResponse<EmbeddingResult>.Ko("No response from Local OpenAI");
        }

        public Task<ChatResult> Summarize(string source)
        {
            throw new NotImplementedException();
        }

        public Task<ChatResult> Summarize(Uri url)
        {
            throw new NotImplementedException();
        }

        public Task<ChatResult> TranslateToFrench(string source)
        {
            throw new NotImplementedException();
        }

        public Task<ChatResult> TranslateToFrench(Uri url)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<string>>> AskQuestionsOnPDF(string[] questions, string pdfPath)
        {
            // Get the PDF file
            var pdf = await PDFTools.ReadPDFFile(pdfPath);
            if (!pdf.Succeeded)
                return ServiceResponse<List<string>>.Ko(pdf.Message);

            var text = pdf.Content.Replace("  ", " ").Replace("\n", "; ").Replace(';', ' ');
            var questSerialized = "";
            for (int i = 0; i < questions.Length; i++)
            {
                questSerialized += $"{i + 1}. {questions[i]}\\n";
            }

            var results = new List<string>();

            var document = "<document>";
            var templatePrompt = @"Extract key pieces of information from this regulation document.
                If a particular piece of information is not present, output ""Not specified"".
                When you extract a key piece of information, include the closest page number.
                Use the following format:\n0. Who is the author\n";
            templatePrompt += questSerialized + "\nDocument: \"<document>\"\n\n0. Who is the author: Tom Anderson (Page 1)\n1.";

            var chunks = createChunks(text, 1500);
            var sb = new StringBuilder();
            foreach (var chunk in chunks)
            {
                sb.Append(chunk);

                var result = await extractChunk(chunk, templatePrompt);
                if (result.Succeeded && result.Content != null)
                    results.Add(result.Content);
            }

            using (var fs = File.CreateText("result.txt"))
            {
                fs.Write(sb.ToString());
                fs.Close();
            }

            // Filter only without Not Specified
            results = results.Where(x => !x.Contains("Not specified")).ToList();

            return ServiceResponse<List<string>>.Ok(results);
        }

        private IEnumerable<string> createChunks(string text, int size)
        {
            var remaining = text;
            int chunkSize = 0;
            var chunk = "";
            while (remaining.Length > 0)
            {
                var match = Regex.Match(remaining, @"(?<=[\.!\?])\s+");
                if (match.Success)
                {
                    chunk = chunk + remaining.Substring(0, match.Index);
                    //yield return remaining.Substring(0, match.Index);
                    remaining = remaining.Substring(match.Index);
                }
                else
                {
                    if (remaining.Length > size)
                    {
                        chunk = chunk + remaining.Substring(0, size);
                        remaining = remaining.Substring(size);
                        //yield return remaining.Substring(0, maxLength.Value);
                    }
                    else
                    {
                        chunk += remaining;
                        remaining = "";
                    }
                }

                if (chunk.Length > size)
                {
                    yield return chunk;
                    chunk = "";
                }
            }


        }

        private async Task<ServiceResponse<string>> extractChunk(string document, string templatePrompt)
        {
            var prompt = templatePrompt.Replace("<document>", document);
            var messages = new AIMessage[] { new AIMessage {role="system", content="You help extract information from documents." },
                new AIMessage {role="user", content=prompt } };

            var answer = await AskQuestion(messages);
            if (!answer.Succeeded)
                return answer;

            return ServiceResponse<string>.Ok(answer.Content);

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
