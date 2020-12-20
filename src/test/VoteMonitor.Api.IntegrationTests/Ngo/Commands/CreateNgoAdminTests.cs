using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using VoteMonitor.Api.IntegrationTests.Setup;
using Xunit;

namespace VoteMonitor.Api.IntegrationTests.Ngo.Commands
{
    public class CreateNgoAdminTests : EndpointsTests
    {
        public CreateNgoAdminTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Returns_Success_For_Valid_Request()
        {
            // Arrange
            var client = Factory.CreateClient().WithNgoAdminAuthorizationPolicy();

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/ngo/1/ngoadmin", new
            {
                account = "NgoAdmin",
                password = "Lkjn/1QQ"
            });

            // Assert
            response.Should().Be200Ok();
        }

        [Fact]
        public async Task Prevents_Anonymous_Access()
        {
            // Arrange
            var client = Factory.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/ngo/1/ngoadmin", new { });

            // Assert
            response.Should().Be401Unauthorized();
        }

        [Theory]
        [InlineData(Policy.Observer)]
        [InlineData(Policy.Organizer)]
        public async Task Forbids_Unauthorized_Access(Policy unauthorizedPolicy)
        {
            // Arrange
            var client = Factory.CreateClient().WithAuthorizationPolicy(unauthorizedPolicy);

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/ngo/1/ngoadmin", new { });

            // Assert
            response.Should().Be403Forbidden();
        }

        [Fact]
        public async Task Forbids_Unauthorized_Access_For_Other_Ngo_Admins()
        {
            // Arrange
            var otherNgoAdminId = "2";
            var client = Factory.CreateClient().WithNgoAdminAuthorizationPolicy();

            // Act
            var response = await client.PostAsJsonAsync($"/api/v1/ngo/{otherNgoAdminId}/ngoadmin", new { });

            // Assert
            response.Should().Be400BadRequest()
                .And.BeAs(new
                {
                    detail = "Cannot create admins that are not in your Ngo"
                });
        }
    }
}