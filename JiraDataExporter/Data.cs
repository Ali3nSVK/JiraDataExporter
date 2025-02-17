namespace JiraDataExporter
{
    public static class Data
    {
        public class JiraSettings
        {
            public required string BaseUrl { get; set; }
            public required string ApiToken { get; set; }
        }

        public class JiraDto
        {
            public required string IssueKey { get; set; }
            public string? Status { get; set; }
            public string? SecurityLevel { get; set; }
            public DateTime? UpdateDate { get; set; }
        }
    }
}
