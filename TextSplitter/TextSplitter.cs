using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;


namespace Basement.TextChunker
    {
    public enum SplitMethod
    {
        Length,
        Sentence,
        Recursive,
        Embedding
    }
    public static class TextChunker
    {


        public static List<string> SplitText(string text, int maxLength, int overlap, SplitMethod method, params string[] delimiters)
        {
            switch (method)
            {
                case SplitMethod.Length:
                    return SplitByLength(text, maxLength, overlap);
                case SplitMethod.Sentence:
                    return SplitBySentence(text, maxLength, overlap, delimiters[0]);
                case SplitMethod.Recursive:
                    return SplitByRecursive(text, maxLength, overlap, delimiters);
                case SplitMethod.Embedding:
                    return SplitByEmbedding(text, maxLength, overlap);
                default:
                    throw new NotImplementedException("The specified split method is not implemented.");
            }
        }

        private static List<string> SplitByLength(string text, int maxLength, int overlap)
        {
            var chunks = new List<string>();

            int overlapIndex = 0;
            for (int i = 0; i < text.Length; i += maxLength)
            {
                chunks.Add(text.Substring(i, Math.Min(maxLength, text.Length - i)));
                i -= overlap;
            }
            return chunks;
        }

        private static List<string> SplitBySentence(string text, int maxLength, int overlap, string delimiter)
        {
            var chunks = new List<string>();
            var sentences = text.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
            var chunk = string.Empty;

            var overlapIndex = 0;
            for (var i = 0; i < sentences.Length; i++)
            {
                var sentence = sentences[i];

                if (chunk.Length + sentence.Length + delimiter.Length <= maxLength)
                {
                    chunk += (chunk.Length > 0 ? delimiter : "") + sentence;
                    if (chunk.Length < maxLength - overlap)
                    {
                        overlapIndex++;
                    }

                }
                else
                {
                    chunk += delimiter;
                    chunks.Add(chunk);
                    chunk = sentence;
                    i = overlapIndex;
                }
            }

            if (!string.IsNullOrEmpty(chunk))
            {
                chunk += delimiter;
                chunks.Add(chunk);
            }

            return chunks;
        }


        /// <summary>
        /// Remaining the overlap function to impelement ^& Oh you want to ask pourquoi? No Why.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <param name="overlap"></param>
        /// <param name="delimiters"></param>
        /// <returns></returns>
        private static List<string> SplitByRecursive(string text, int maxLength, int overlap, string[] delimiters)
        {
            var chunks = new List<string>() { text };

            var currentText = text;

            List<string> subChunks = new List<string>();

            foreach (var delimiter in delimiters)
            {
                subChunks = chunks;
                var _subChunks = new List<string>();

                foreach (var chunk in subChunks)
                {
                    if (chunk.Length <= maxLength)
                    {
                        _subChunks.Add(chunk);
                    }
                    else
                    {
                        var parts = chunk.Split(new[] { delimiter }, StringSplitOptions.TrimEntries).ToList();
                        for (var i = 0; i < parts.Count; i++)
                        {
                            if (parts[i] == "")
                            {
                                parts.RemoveAt(i);
                            }
                        }

                        for (var i = 0; i < parts.Count; i++)
                        {
                            _subChunks.Add(parts[i] + delimiter);
                        }
                    }
                }
                chunks = _subChunks;
            }

            return chunks;
        }

        private static List<string> SplitByEmbedding(string text, int maxLength, int overlap)
        {
            // Placeholder for embedding-based splitting
            // This would require integration with a large language model
            throw new NotImplementedException("Embedding split method is not implemented.");
        }
    }
}