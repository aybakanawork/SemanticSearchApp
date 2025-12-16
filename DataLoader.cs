using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NLog;

namespace SemanticSearchApp
{
    public class DataLoader
    {
        private readonly string _dataDirectory;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DataLoader(string dataDirectory = "data")
        {
            _dataDirectory = dataDirectory;
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
                Logger.Info($"Created data directory: {_dataDirectory}");
            }
            else
            {
                Logger.Debug($"Data directory exists: {_dataDirectory}");
            }
        }

        public async Task<List<Document>> LoadDocumentsAsync()
        {
            var documents = new List<Document>();
            
            try
            {
                Logger.Info($"Starting to load documents from directory: {_dataDirectory}");
                var files = Directory.GetFiles(_dataDirectory, "*.txt", SearchOption.AllDirectories);
                Logger.Info($"Found {files.Length} text files to load");
                
                foreach (var file in files)
                {
                    Logger.Debug($"Loading document from file: {file}");
                    var content = await File.ReadAllTextAsync(file);
                    var title = Path.GetFileNameWithoutExtension(file);
                    var id = Guid.NewGuid().ToString();
                    
                    documents.Add(new Document(id, title, content, file));
                    Logger.Debug($"Successfully loaded document: {title} (ID: {id}, Size: {content.Length} bytes)");
                }
                
                Logger.Info($"Successfully loaded {documents.Count} documents");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading documents: {ex.Message}");
                Logger.Debug($"Stack trace: {ex.StackTrace}");
                throw;
            }

            return documents;
        }
    }
}
