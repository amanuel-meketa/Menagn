namespace security.sharedUtils.Utilities
{
    public class DateTimeHelper
    {
        // Converts a Unix timestamp (in milliseconds) to a formatted date string.
        public static string? FormatUnixTimestamp(object? timestamp, string format = "M/d/yyyy, h:mm:ss tt")
        {
            if (timestamp == null)
                return null;

            try
            {
                long unixMilliseconds = Convert.ToInt64(timestamp);
                return DateTimeOffset.FromUnixTimeMilliseconds(unixMilliseconds).ToString(format);
            }
            catch
            {
                return null;
            }
        }
    }
}
