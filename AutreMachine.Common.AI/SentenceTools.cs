using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutreMachine.Common.AI
{
    public class SentenceTools
    {
        /// <summary>
        /// Splits an input text into sentences (works mainly for English, French...)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxLength">If set, blocks the lengtrh of returned sentences and cut them</param>
        /// <returns></returns>
        public static string[] SplitIntoSentences(string input, int? maxLength = null)
        {
            string[] sentences = Regex.Split(input, @"(?<=[\.!\?])\s+");

            // Check maxLength
            if (maxLength != null)
            {
                var ret = new List<string>();
                foreach (var sentence in sentences)
                {
                    if (sentence.Length <= maxLength)
                        ret.Add(sentence);
                    else
                    {
                        var sentence2 = sentence;
                        while (sentence2.Length > maxLength)
                        {
                            ret.Add(sentence2.Substring(0, maxLength.Value));
                            sentence2 = sentence2.Substring(maxLength.Value);
                        }
                    }
                }
                return ret.ToArray();
            }

            return sentences;
        }
    }
}
