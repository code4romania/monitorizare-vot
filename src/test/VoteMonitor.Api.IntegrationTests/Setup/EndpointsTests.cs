using FluentAssertions;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace VoteMonitor.Api.IntegrationTests.Setup
{
    [Collection("Endpoints tests")]
    public class EndpointsTests : IAsyncLifetime
    {
        protected readonly CustomWebApplicationFactory Factory;

        public EndpointsTests(CustomWebApplicationFactory factory)
        {
            Factory = factory;
        }

        public Task InitializeAsync() => Factory.Respawn();

        protected async Task<int> WithNgo(object payload)
        {
            var client = Factory.CreateClient().WithOrganizerAuthorizationPolicy();

            var response = await client.PostAsJsonAsync("/api/v1/ngo", payload);

            response.Should().Be200Ok();

            var model = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

            return ((JsonElement)model!["id"]).GetInt32();
        }

        protected async Task<int> WithNgoAdmin(object payload)
        {
            var client = Factory.CreateClient().WithNgoAdminAuthorizationPolicy();

            var response = await client.PostAsJsonAsync("/api/v1/ngo/1/ngoadmin", payload);

            response.Should().Be200Ok();

            var model = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

            return ((JsonElement)model!["id"]).GetInt32();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
