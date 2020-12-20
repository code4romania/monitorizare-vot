using System.Net.Http;

namespace VoteMonitor.Api.IntegrationTests.Setup
{
    public static class HttpClientExtensions
    {
        public static HttpClient WithAuthorizationPolicy(this HttpClient client, Policy policy, params (string type, string value)[] otherClaims)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Jwt.CreateJwt(policy, otherClaims));
            return client;
        }

        public static HttpClient WithNgoAdminAuthorizationPolicy(this HttpClient client, params (string type, string value)[] otherClaims) =>
            client.WithAuthorizationPolicy(Policy.NgoAdmin, otherClaims);

        public static HttpClient WithOrganizerAuthorizationPolicy(this HttpClient client, params (string type, string value)[] otherClaims) =>
            client.WithAuthorizationPolicy(Policy.Organizer, otherClaims);

        public static HttpClient WithObserverAuthorizationPolicy(this HttpClient client, params (string type, string value)[] otherClaims) =>
            client.WithAuthorizationPolicy(Policy.Observer, otherClaims);
    }
}