using System.ComponentModel.DataAnnotations;

namespace authorization.api.Configs
{
    public class OpenFGAConfig
    {
        public const string SectionName = "AuthZProvider";

        [Required]
        [Url]
        public string? BaseUrl { get; set; }
        [Required]
        public string? StoreName { get; set; } 
        public string? ApiToken { get; set; } 
        public string? AuthorizationModelId { get; set; }
        public string SchemaVersion { get; set; } = "1.1";
    }
}
