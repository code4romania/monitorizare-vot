using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using VoteMonitor.Api.IntegrationTests.Setup;
using Xunit;

namespace VoteMonitor.Api.IntegrationTests.Ngo.Commands
{
    public class UpdateNgoTests : EndpointsTests
    {
        public UpdateNgoTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Returns_Success_For_Valid_Request()
        {
            // Arrange
            var id = await WithNgo(new
            {
                shortName = "C4R",
                name = "Code4Romania",
                organizer = true,
                isActive = false
            });
            var client = Factory.CreateClient().WithOrganizerAuthorizationPolicy();

            // Act
            var response = await client.PostAsJsonAsync($"/api/v1/ngo/{id}", new
            {
                shortName = "UUU",
                name = "Code4Romania-Updated",
                organizer = false,
                isActive = true
            });

            // Assert
            response.Should().Be200Ok();
            var updatedNgoResponse = await client.GetAsync($"/api/v1/ngo/{id}");
            updatedNgoResponse.Should().BeAs(new
            {
                shortName = "UUU",
                name = "Code4Romania-Updated",
                organizer = false,
                isActive = true
            });
        }

        [Fact]
        public async Task Prevents_Anonymous_Access()
        {
            // Arrange
            var client = Factory.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/ngo/1", new { });

            // Assert
            response.Should().Be401Unauthorized();
        }

        [Theory]
        [InlineData(Policy.Observer)]
        [InlineData(Policy.NgoAdmin)]
        public async Task Forbids_Unauthorized_Access(Policy unauthorizedPolicy)
        {
            // Arrange
            var client = Factory.CreateClient().WithAuthorizationPolicy(unauthorizedPolicy);

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/ngo/1", new { });

            // Assert
            response.Should().Be403Forbidden();
        }
    }
}