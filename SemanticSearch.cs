using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using NLog;

namespace SemanticSearchApp
{
    public class SemanticSearch
    {
        private readonly EmbeddingService _embeddingService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SemanticSearch(EmbeddingService embeddingService)
        {
            _embeddingService = embeddingService;
        }

        public async Task<List<(Document Document, float Score)>> SearchAsync(string query, List<Document> documents, int topK = 5)
        {
            // Generate embedding for the query
            var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);
            Logger.Debug($"Query embedding dimensions: {queryEmbedding.Length}");

            // Calculate cosine similarity between query and all documents
            var results = documents
                .Select(doc => {
                    var score = CalculateCosineSimilarity(queryEmbedding, doc.Embedding);
                    Logger.Debug($"Document '{doc.Title}' - Embedding dimensions: {doc.Embedding?.Length ?? 0}, Similarity score: {score:F4}");
                    return (Document: doc, Score: score);
                })
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .ToList();

            Logger.Info($"Search results: {results.Count} documents, scores range: {(results.Count > 0 ? $"{results.Max(x => x.Score):F4} to {results.Min(x => x.Score):F4}" : "N/A")}");
            return results;
        }

        private float CalculateCosineSimilarity(float[] v1, float[] v2)
        {
            if (v1 == null || v2 == null || v1.Length != v2.Length || v1.Length == 0)
            {
                Logger.Debug($"Cosine similarity calculation skipped: v1 is {(v1 == null ? "null" : v1.Length)}, v2 is {(v2 == null ? "null" : v2.Length)}");
                return 0;
            }

            float dotProduct = 0;
            float norm1 = 0;
            float norm2 = 0;

            for (int i = 0; i < v1.Length; i++)
            {
                dotProduct += v1[i] * v2[i];
                norm1 += v1[i] * v1[i];
                norm2 += v2[i] * v2[i];
            }

            norm1 = (float)Math.Sqrt(norm1);
            norm2 = (float)Math.Sqrt(norm2);

            if (norm1 == 0 || norm2 == 0)
            {
                Logger.Debug($"Cosine similarity calculation resulted in 0: norm1={norm1}, norm2={norm2}");
                return 0;
            }

            var similarity = dotProduct / (norm1 * norm2);
            return similarity;
        }
    }
}
