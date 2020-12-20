using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using VoteMonitor.Api.IntegrationTests.Setup;
using Xunit;

namespace VoteMonitor.Api.IntegrationTests.Ngo.Commands
{
    public class CreateNgoTests : EndpointsTests
    {
        public CreateNgoTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Returns_Success_For_Valid_Request()
        {
            // Arrange
            var client = Factory.CreateClient().WithOrganizerAuthorizationPolicy();

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/ngo", new
            {
                shortName = "NewNgo",
                name = "New Ngo",
                organizer = false,
                isActive = true
            });

            // Assert
            response.Should().Be200Ok()
                .And.BeAs(new
                {
                    shortName = "NewNgo",
                    name = "New Ngo",
                    organizer = false,
                    isActive = true
                })
                .And.Satisfy(givenModelStructure: new
                {
                    id = default(int)
                }, assertion: model => model.id.Should().BePositive());
        }

        [Fact]
        public async Task Prevents_Anonymous_Access()
        {
            // Arrange
            var client = Factory.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/ngo", new
            {
                shortName = "NewNgo",
                name = "New Ngo",
                organizer = false,
                isActive = true
            });

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
            var response = await client.PostAsJsonAsync("/api/v1/ngo", new { });

            // Assert
            response.Should().Be403Forbidden();
        }
    }
}