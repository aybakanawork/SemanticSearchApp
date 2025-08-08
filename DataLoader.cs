using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SemanticSearchApp
{
    public class DataLoader
    {
        private readonly string _dataDirectory;

        public DataLoader(string dataDirectory = "data")
        {
            _dataDirectory = dataDirectory;
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }

        public async Task<List<Document>> LoadDocumentsAsync()
        {
            var documents = new List<Document>();
            
            try
            {
                var files = Directory.GetFiles(_dataDirectory, "*.txt", SearchOption.AllDirectories);
                
                foreach (var file in files)
                {
                    var content = await File.ReadAllTextAsync(file);
                    var title = Path.GetFileNameWithoutExtension(file);
                    var id = Guid.NewGuid().ToString();
                    
                    documents.Add(new Document(id, title, content, file));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading documents: {ex.Message}");
                throw;
            }

            return documents;
        }
    }
}
