namespace SemanticSearchApp
{
    public class Document
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public float[]? Embedding { get; set; }

        public Document(string id, string title, string content, string filePath)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Embedding = Array.Empty<float>();
        }
    }
}
