namespace SemanticSearchApp
{
    public class SearchResult
    {
        public Document Document { get; }
        public float Score { get; }

        public string Title => Document.Title;
        public string Content => Document.Content;
        public string FilePath => Document.FilePath;

        public SearchResult(Document document, float score)
        {
            Document = document;
            Score = score;
        }
    }
}
