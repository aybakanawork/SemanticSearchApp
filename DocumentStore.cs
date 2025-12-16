using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace SemanticSearchApp
{
    public class DocumentStore
    {
        private List<Document> _documents;
        private readonly DataLoader _dataLoader;
        private readonly EmbeddingService _embeddingService;
        private readonly SemanticSearch _semanticSearch;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public event EventHandler<string>? ProcessingStatusChanged;

        public DocumentStore(DataLoader dataLoader)
        {
            _dataLoader = dataLoader;
            _embeddingService = new EmbeddingService();
            _semanticSearch = new SemanticSearch(_embeddingService);
            _documents = new List<Document>();
            Logger.Info("DocumentStore initialized");
        }

        public async Task InitializeAsync()
        {
            Logger.Info("Starting DocumentStore initialization");
            ProcessingStatusChanged?.Invoke(this, "Loading documents...");
            _documents = await _dataLoader.LoadDocumentsAsync();
            Logger.Info($"Loaded {_documents.Count} documents");
            
            ProcessingStatusChanged?.Invoke(this, "Checking Ollama availability...");
            Logger.Info("Checking Ollama model availability");
            try 
            {
                if (!await _embeddingService.IsModelAvailable())
                {
                    Logger.Fatal("Embedding model is not available");
                    var errorMessage = "Embedding model is not available. Please ensure:\n" +
                                     "1. Ollama is running (check if service is started)\n" +
                                     "2. The model 'all-minilm' is installed (run 'ollama pull all-minilm')\n" +
                                     "3. Ollama API is accessible at http://localhost:11434";
                    throw new Exception(errorMessage);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error connecting to Ollama: {ex.Message}");
                throw new Exception($"Error connecting to Ollama: {ex.Message}");
            }

            ProcessingStatusChanged?.Invoke(this, "Generating embeddings...");
            Logger.Info($"Starting embedding generation for {_documents.Count} documents");
            int successCount = 0;
            int failureCount = 0;
            
            foreach (var doc in _documents)
            {
                try
                {
                    Logger.Debug($"Generating embedding for document: {doc.Title}");
                    doc.Embedding = await _embeddingService.GenerateEmbeddingAsync(doc.Content);
                    successCount++;
                    Logger.Debug($"Successfully generated embedding for: {doc.Title}");
                }
                catch (Exception ex)
                {
                    failureCount++;
                    Logger.Error($"Error generating embedding for {doc.Title}: {ex.Message}");
                    ProcessingStatusChanged?.Invoke(this, $"Error generating embedding for {doc.Title}: {ex.Message}");
                }
            }
            
            Logger.Info($"Embedding generation completed: {successCount} successful, {failureCount} failed");
            ProcessingStatusChanged?.Invoke(this, "Ready");
        }

        public async Task<List<SearchResult>> SearchDocumentsAsync(string query)
        {
            Logger.Info($"Starting search for query: '{query}'");
            ProcessingStatusChanged?.Invoke(this, "Searching...");

            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    Logger.Debug("Empty query provided, returning all documents");
                    return _documents.Select(d => new SearchResult(d, 1.0f)).ToList();
                }

                Logger.Debug($"Performing semantic search on {_documents.Count} documents");
                var results = await _semanticSearch.SearchAsync(query, _documents);
                Logger.Info($"Search completed: found {results.Count} results");
                return results.Select(r => new SearchResult(r.Document, r.Score)).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error during search: {ex.Message}");
                Logger.Debug($"Stack trace: {ex.StackTrace}");
                throw;
            }
            finally
            {
                ProcessingStatusChanged?.Invoke(this, "Ready");
            }
        }

        public Document? GetDocumentById(string id)
        {
            Logger.Debug($"Retrieving document by ID: {id}");
            var document = _documents.FirstOrDefault(d => d.Id == id);
            if (document != null)
            {
                Logger.Debug($"Found document: {document.Title}");
            }
            else
            {
                Logger.Warn($"Document not found for ID: {id}");
            }
            return document;
        }
    }
}
