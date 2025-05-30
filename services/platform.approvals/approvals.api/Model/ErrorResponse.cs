namespace approvals.api.Model
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Source { get; set; }
    }
}
