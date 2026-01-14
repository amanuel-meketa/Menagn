using approvals.application.DTOs.TemplateHub;
using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace approvals.infrastructure.Persistence.Repositories
{
    public class TemplateHubRepository : ITemplateHubRepository
    {
        private readonly HttpClient _http;
        private readonly GitHubTemplateOptions _options;

        public TemplateHubRepository(HttpClient http, IOptions<GitHubTemplateOptions> options)
        {
            _http = http;
            _options = options.Value;
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("TemplateHubApp");
            if (!string.IsNullOrEmpty(_options.Token))
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.Token);
        }

        public async Task<List<TemplateIndexItem>> GetTemplateIndexAsync()
        {
            var url = $"https://raw.githubusercontent.com/{_options.Owner}/{_options.Repo}/{_options.Branch}/{_options.IndexFile}";
            var content = await _http.GetStringAsync(url);
            return JsonSerializer.Deserialize<List<TemplateIndexItem>>(content) ?? new List<TemplateIndexItem>();
        }

        public async Task<ApprovalTemplate?> GetTemplateAsync(string key)
        {
            try
            {
                var templateUrl = $"https://raw.githubusercontent.com/{_options.Owner}/{_options.Repo}/{_options.Branch}/{key}/template.json";
                var stagesUrl = $"https://raw.githubusercontent.com/{_options.Owner}/{_options.Repo}/{_options.Branch}/{key}/stages.json";

                var templateContent = await _http.GetStringAsync(templateUrl);
                var stagesContent = await _http.GetStringAsync(stagesUrl);

                var template = JsonSerializer.Deserialize<ApprovalTemplate>(templateContent);
                var stages = JsonSerializer.Deserialize<List<StageDefinition>>(stagesContent);

                if (template != null && stages != null)
                {
                    template.StageDefinitions = stages;
                    return template;
                }
            }
            catch
            {
                // optionally log errors
            }
            return null;
        }
    }

    // Options class
    public class GitHubTemplateOptions
    {
        public string Owner { get; set; } = default!;
        public string Repo { get; set; } = default!;
        public string Branch { get; set; } = "main";
        public string IndexFile { get; set; } = "index.json";
        public string Token { get; set; } = string.Empty;
    }
}
