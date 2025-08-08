using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SemanticSearchApp
{
    public class EmbeddingService
    {
        private readonly HttpClient _httpClient;
        private const string OLLAMA_BASE_URL = "http://localhost:11434/api";
        private const string MODEL_NAME = "all-minilm";

        public EmbeddingService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(OLLAMA_BASE_URL)
            };
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var requestBody = new
            {
                model = MODEL_NAME,
                prompt = text
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            try
            {
                var response = await _httpClient.PostAsync("/embeddings", content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OllamaEmbeddingResponse>(jsonResponse);

                return result?.Embedding ?? Array.Empty<float>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating embedding: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> IsModelAvailable()
        {
            try
            {
                var response = await _httpClient.GetAsync($"/tags");
                response.EnsureSuccessStatusCode();
                
                var jsonResponse = await response.Content.ReadAsStringAsync();
                // Check if the model is in the response
                return jsonResponse.Contains(MODEL_NAME);
            }
            catch
            {
                return false;
            }
        }

        private class OllamaEmbeddingResponse
        {
            public float[] Embedding { get; set; }
        }
    }
}
