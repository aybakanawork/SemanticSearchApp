namespace SemanticSearchApp
{
    public class Document
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string FilePath { get; set; }
        public float[] Embedding { get; set; }

        public Document(string id, string title, string content, string filePath)
        {
            Id = id;
            Title = title;
            Content = content;
            FilePath = filePath;
            Embedding = Array.Empty<float>();
        }
    }
}
