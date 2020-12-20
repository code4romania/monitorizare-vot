using System.Threading.Tasks;
using FluentAssertions;
using VoteMonitor.Api.IntegrationTests.Setup;
using Xunit;

namespace VoteMonitor.Api.IntegrationTests.Ngo.Queries
{
    public class GetNgoDetailsTests : EndpointsTests
    {
        public GetNgoDetailsTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Returns_Success_For_Valid_Request()
        {
            // Arrange
            var client = Factory.CreateClient().WithOrganizerAuthorizationPolicy();

            // Act
            var response = await client.GetAsync("/api/v1/ngo/1");

            // Assert
            response.Should().Be200Ok()
                .And.BeAs(new
                {
                    id = 1,
                    shortName = "C4R",
                    name = "Code4Romania",
                    organizer = true,
                    isActive = false
                });
        }

        [Fact]
        public async Task Prevents_Anonymous_Access()
        {
            // Arrange
            var client = Factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/ngo/1");

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
            var response = await client.GetAsync("/api/v1/ngo/1");

            // Assert
            response.Should().Be403Forbidden();
        }
    }
}