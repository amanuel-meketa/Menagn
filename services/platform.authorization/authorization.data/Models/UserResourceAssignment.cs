namespace authorization.data.Models
{
    public class UserResourceAssignment
    {
        required
        public string UserId { get; set; }

        required
        public string Resource { get; set; }

        required
        public IEnumerable<string> Scopes { get; set; }
    }
}
