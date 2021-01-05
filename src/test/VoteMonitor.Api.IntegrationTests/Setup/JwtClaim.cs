namespace VoteMonitor.Api.IntegrationTests.Setup
{
    public class JwtClaim
    {
        public JwtClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public string Type { get; set; }
        public string Value { get; set; }
    }
}