using System.Threading.Tasks;
using FluentAssertions;
using VoteMonitor.Api.IntegrationTests.Setup;
using Xunit;

namespace VoteMonitor.Api.IntegrationTests.Ngo.Commands
{
    public class DeleteNgoTests : EndpointsTests
    {
        public DeleteNgoTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteNgo_ReturnsOk()
        {
            // Arrange
            var client = Factory.CreateClient().WithOrganizerAuthorizationPolicy();
            var id = await WithNgo(new
            {
                shortName = "C4R",
                name = "Code4Romania",
                organizer = true,
                isActive = false
            });

            // Act
            var response = await client.DeleteAsync($"/api/v1/ngo/{id}");

            // Assert
            response.Should().Be200Ok();
        }

        [Fact]
        public async Task Prevents_Anonymous_Access()
        {
            // Arrange
            var client = Factory.CreateClient();

            // Act
            var response = await client.DeleteAsync($"/api/v1/ngo/1");

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
            var response = await client.DeleteAsync($"/api/v1/ngo/1");

            // Assert
            response.Should().Be403Forbidden();
        }
    }
}