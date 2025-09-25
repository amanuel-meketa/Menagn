namespace authorization.data.Models
{
    public class Tuple
    {
        public string User { get; set; }          // e.g., "user:123"
        public string Relation { get; set; }      // e.g., "can_read"
        public string ObjectType { get; set; }    // e.g., "approvalInstance"
        public string ObjectId { get; set; }      // e.g., "all" or specific id
    }

}
