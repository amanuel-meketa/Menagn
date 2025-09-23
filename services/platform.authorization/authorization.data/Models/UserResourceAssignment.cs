namespace authorization.data.Models
{
    public record UserResourceAssignment
    {  
       public required string UserId { get; set; }
 
        public required string Resource { get; set; }
     
        public required IEnumerable<string> Scopes { get; set; }
    }
}
