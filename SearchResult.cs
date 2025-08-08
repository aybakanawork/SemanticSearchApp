namespace SemanticSearchApp
{
    public class SearchResult
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public double Relevance { get; set; }

        public SearchResult(string title, string content, double relevance = 1.0)
        {
            Title = title;
            Content = content;
            Relevance = relevance;
        }
    }
}
