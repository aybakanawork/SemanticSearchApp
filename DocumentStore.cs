using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemanticSearchApp
{
    public class DocumentStore
    {
        private List<Document> _documents;
        private readonly DataLoader _dataLoader;
        private readonly EmbeddingService _embeddingService;
        public event EventHandler<string> ProcessingStatusChanged;

        public DocumentStore(DataLoader dataLoader)
        {
            _dataLoader = dataLoader;
            _embeddingService = new EmbeddingService();
            _documents = new List<Document>();
        }

        public async Task InitializeAsync()
        {
            ProcessingStatusChanged?.Invoke(this, "Loading documents...");
            _documents = await _dataLoader.LoadDocumentsAsync();
            
            ProcessingStatusChanged?.Invoke(this, "Checking Ollama availability...");
            try 
            {
                if (!await _embeddingService.IsModelAvailable())
                {
                    var errorMessage = "Embedding model is not available. Please ensure:\n" +
                                     "1. Ollama is running (check if service is started)\n" +
                                     "2. The model 'all-minilm' is installed (run 'ollama pull all-minilm')\n" +
                                     "3. Ollama API is accessible at http://localhost:11434";
                    throw new Exception(errorMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error connecting to Ollama: {ex.Message}");
            }

            ProcessingStatusChanged?.Invoke(this, "Generating embeddings...");
            foreach (var doc in _documents)
            {
                try
                {
                    doc.Embedding = await _embeddingService.GenerateEmbeddingAsync(doc.Content);
                }
                catch (Exception ex)
                {
                    ProcessingStatusChanged?.Invoke(this, $"Error generating embedding for {doc.Title}: {ex.Message}");
                }
            }
            
            ProcessingStatusChanged?.Invoke(this, "Ready");
        }

        public List<Document> SearchDocuments(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return _documents;

            // Simple text-based search
            return _documents
                .Where(doc => 
                    doc.Title.Contains(query, System.StringComparison.OrdinalIgnoreCase) ||
                    doc.Content.Contains(query, System.StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public Document GetDocumentById(string id)
        {
            return _documents.FirstOrDefault(d => d.Id == id);
        }
    }
}
