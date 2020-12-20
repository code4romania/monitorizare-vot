using System.Threading.Tasks;
using FluentAssertions;
using VoteMonitor.Api.IntegrationTests.Setup;
using Xunit;

namespace VoteMonitor.Api.IntegrationTests.Ngo.Commands
{
    public class SetNgoOrganizerFlagTests : EndpointsTests
    {
        public SetNgoOrganizerFlagTests(CustomWebApplicationFactory factory) : base(factory)
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
            var response = await client.PatchAsJsonAsync($"/api/v1/ngo/{id}/status", new
            {
                isActive = true
            });

            // Assert
            response.Should().Be200Ok();
            var updatedNgoResponse = await client.GetAsync($"/api/v1/ngo/{id}");
            updatedNgoResponse.Should().BeAs(new
            {
                isActive = true
            });
        }

        [Fact]
        public async Task Prevents_Anonymous_Access()
        {
            // Arrange
            var client = Factory.CreateClient();

            // Act
            var response = await client.PatchAsJsonAsync("/api/v1/ngo/1/status", new { });

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
            var response = await client.PatchAsJsonAsync("/api/v1/ngo/1/status", new { });

            // Assert
            response.Should().Be403Forbidden();
        }
    }
}