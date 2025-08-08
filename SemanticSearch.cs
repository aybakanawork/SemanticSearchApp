using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace SemanticSearchApp
{
    public class SemanticSearch
    {
        private readonly EmbeddingService _embeddingService;

        public SemanticSearch(EmbeddingService embeddingService)
        {
            _embeddingService = embeddingService;
        }

        public async Task<List<(Document Document, float Score)>> SearchAsync(string query, List<Document> documents, int topK = 5)
        {
            // Generate embedding for the query
            var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);

            // Calculate cosine similarity between query and all documents
            var results = documents
                .Select(doc => (
                    Document: doc,
                    Score: CalculateCosineSimilarity(queryEmbedding, doc.Embedding)
                ))
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .ToList();

            return results;
        }

        private float CalculateCosineSimilarity(float[] v1, float[] v2)
        {
            if (v1 == null || v2 == null || v1.Length != v2.Length || v1.Length == 0)
                return 0;

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
                return 0;

            return dotProduct / (norm1 * norm2);
        }
    }
}
