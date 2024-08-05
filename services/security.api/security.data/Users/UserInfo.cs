using security.data.Roles;

namespace security.data.User
{
    public class UserInfo
    {
        public string? Name { get; set; }
        public string? PreferredUsername { get; set; }
        public string? Email { get; set; }
        public IEnumerable<Role>? Roles { get; set; }
    }
}
