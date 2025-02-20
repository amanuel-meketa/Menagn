using Newtonsoft.Json;
using security.sharedUtils.Dtos.User.Common;

public class GetUserDto : UserDto
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public bool? EmailVerified { get; set; }
    public bool? Enabled { get; set; }

    [JsonProperty("createdTimestamp")]
    public long CreatedTimestampRaw { get; set; }
    public DateTime? CreatedTimestamp => CreatedTimestampRaw > 0? DateTimeOffset.FromUnixTimeMilliseconds(CreatedTimestampRaw).UtcDateTime : null;
}
