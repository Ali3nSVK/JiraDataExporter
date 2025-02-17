using RestSharp;
using System.IO;
using System.Text.Json;
using static JiraDataExporter.Data;

namespace JiraDataExporter
{
    public class JiraService
    {
        private readonly RestClient _client;
        private readonly JiraSettings _settings;

        public JiraService()
        {
            _settings = LoadSettings();
            _client = new RestClient(_settings.BaseUrl);
            _client.AddDefaultHeader("Authorization", $"Bearer {_settings.ApiToken}");
        }

        private static JiraSettings LoadSettings()
        {
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException("settings.json not found in application directory");
            }

            string jsonContent = File.ReadAllText(jsonPath);
            var settings = JsonSerializer.Deserialize<JiraSettings>(jsonContent);

            if (settings is null || string.IsNullOrEmpty(settings.BaseUrl) || string.IsNullOrEmpty(settings.ApiToken))
            {
                throw new InvalidOperationException("JIRA settings are incomplete. Please check settings.json");
            }

            return settings;
        }

        public async Task<List<JiraDto>> GetIssuesStatusAsync(IEnumerable<string> issueKeys)
        {
            var results = new List<JiraDto>();

            foreach (var issueKey in issueKeys)
            {
                try
                {
                    var dto = await GetSingleIssueStatusAsync(issueKey);
                    if (dto != null)
                    {
                        results.Add(dto);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching {issueKey}: {ex.Message}");
                }
            }

            return results;
        }

        private async Task<JiraDto> GetSingleIssueStatusAsync(string issueKey)
        {
            var request = new RestRequest($"/rest/api/2/issue/{issueKey}");
            var response = await _client.ExecuteAsync(request) ?? throw new Exception($"Failed to fetch issue {issueKey}");
           
            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            {
                throw new Exception($"Failed to fetch issue {issueKey}: {response.ErrorMessage}");
            }

            var issueData = JsonSerializer.Deserialize<JsonElement>(response.Content);
            var fields = issueData.GetProperty("fields");

            return new JiraDto
            {
                IssueKey = issueKey,
                Status = fields.GetProperty("status")
                          .GetProperty("name")
                          .GetString(),
                UpdateDate = DateTime.Parse(fields.GetProperty("updated").GetString() ?? DateTime.MinValue.ToString()),
                SecurityLevel = fields.TryGetProperty("security", out var security)
                ? security.GetProperty("name").GetString()
                : null
            };
        }
    }
}
