using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Embedding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common.AI
{
    /// <summary>
    /// Interface for lcoal or remote AI services
    /// </summary>
    public interface IAIService
    {
        Task<ServiceResponse<EmbeddingResult>> CalculateEmbeddings(string text);
        Task<ChatResult> TranslateToFrench(string source);
        Task<ChatResult> TranslateToFrench(Uri url);
        Task<ChatResult> Summarize(string source);
        Task<ChatResult> Summarize(Uri url);
        /// <summary>
        /// Asks a question to the local OpenAI service
        /// Provides a System context
        /// Provides a User question
        /// </summary>
        /// <param name="assistantContent"></param>
        /// <param name="userContent"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> AskQuestion(string assistantContent, string userContent);
        Task<ServiceResponse<string>> AskQuestion(AIMessage[] messages, float temperature = 0.7f);
    }
}
