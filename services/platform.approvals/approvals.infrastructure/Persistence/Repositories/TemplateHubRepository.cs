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

        private static readonly JsonSerializerOptions JsonOptions =
            new()
            {
                PropertyNameCaseInsensitive = true
            };

        public TemplateHubRepository(HttpClient http, IOptions<GitHubTemplateOptions> options)
        {
            _http = http;
            _options = options.Value;
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("TemplateHubApp");

            if (!string.IsNullOrWhiteSpace(_options.Token))
                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.Token);
        }

        public async Task<List<TemplateIndexItemDtos>> GetTemplateIndexAsync()
        {
            var url = $"https://raw.githubusercontent.com/{_options.Owner}/{_options.Repo}/{_options.Branch}/{_options.IndexFile}";

            var content = await _http.GetStringAsync(url);

            return JsonSerializer.Deserialize<List<TemplateIndexItemDtos>>(content, JsonOptions)
                   ?? new List<TemplateIndexItemDtos>();
        }

        public async Task<ApprovalTemplate?> GetTemplateAsync(string key)
        {
            try
            {
                var templateUrl =
                    $"https://raw.githubusercontent.com/{_options.Owner}/{_options.Repo}/{_options.Branch}/{key}/template.json";

                var stagesUrl =
                    $"https://raw.githubusercontent.com/{_options.Owner}/{_options.Repo}/{_options.Branch}/{key}/stages.json";

                var templateContent = await _http.GetStringAsync(templateUrl);
                var stagesContent = await _http.GetStringAsync(stagesUrl);

                var template = JsonSerializer.Deserialize<ApprovalTemplate>(templateContent, JsonOptions);
                var stages = JsonSerializer.Deserialize<List<StageDefinition>>(stagesContent, JsonOptions);

                if (template == null || stages == null)
                    return null;

                // Generate IDs and attach stages
                template.TemplateId = Guid.NewGuid();

                foreach (var stage in stages)
                {
                    stage.StageDefId = Guid.NewGuid();
                    stage.TemplateId = template.TemplateId;
                }

                template.StageDefinitions = stages;

                return template;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching template data from GitHub.", ex);
            }
        }
    }

    public class GitHubTemplateOptions
    {
        public string Owner { get; set; } = default!;
        public string Repo { get; set; } = default!;
        public string Branch { get; set; } = "main";
        public string IndexFile { get; set; } = "index.json";
        public string Token { get; set; } = string.Empty;
    }
}
