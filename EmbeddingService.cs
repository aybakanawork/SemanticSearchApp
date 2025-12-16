using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using NLog;

namespace SemanticSearchApp
{
    public class EmbeddingService
    {
        private readonly HttpClient _httpClient;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string OLLAMA_BASE_URL = "http://localhost:11434";
        private const string MODEL_NAME = "all-minilm";

        public EmbeddingService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(OLLAMA_BASE_URL)
            };
            Logger.Info($"EmbeddingService initialized with Ollama base URL: {OLLAMA_BASE_URL}");
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            Logger.Debug($"Starting embedding generation for text: {text.Substring(0, Math.Min(100, text.Length))}...");
            
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
                Logger.Debug($"Sending POST request to /api/embeddings with model: {MODEL_NAME}");
                var response = await _httpClient.PostAsync("/api/embeddings", content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Logger.Debug($"Received response from Ollama: {jsonResponse.Substring(0, Math.Min(200, jsonResponse.Length))}...");
                
                var result = JsonSerializer.Deserialize<OllamaEmbeddingResponse>(jsonResponse);
                var embeddingLength = result?.Embedding?.Length ?? 0;
                
                Logger.Info($"Successfully generated embedding with {embeddingLength} dimensions");
                return result?.Embedding ?? Array.Empty<float>();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error generating embedding: {ex.Message}");
                Logger.Debug($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> IsModelAvailable()
        {
            try
            {
                Logger.Info($"Checking Ollama model availability at {_httpClient.BaseAddress}");
                var response = await _httpClient.GetAsync("/api/tags");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Logger.Warn($"Error response from Ollama: {errorContent}");
                    Logger.Warn($"Status code: {response.StatusCode}");
                    return false;
                }
                
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Logger.Debug($"Ollama response: {jsonResponse}");
                
                bool isModelAvailable = jsonResponse.Contains(MODEL_NAME);
                if (isModelAvailable)
                {
                    Logger.Info($"Model '{MODEL_NAME}' is available in Ollama");
                }
                else
                {
                    Logger.Warn($"Model '{MODEL_NAME}' is NOT available in Ollama");
                }
                return isModelAvailable;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error checking model availability: {ex.Message}");
                Logger.Debug($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        private class OllamaEmbeddingResponse
        {
            [JsonPropertyName("embedding")]
            public float[]? Embedding { get; set; }
        }
    }
}
