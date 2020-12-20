using System.Threading.Tasks;
using FluentAssertions;
using VoteMonitor.Api.IntegrationTests.Setup;
using Xunit;

namespace VoteMonitor.Api.IntegrationTests.Ngo.Queries
{
    public class GetAllNgoAdminsTests : EndpointsTests
    {
        public GetAllNgoAdminsTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Returns_Success_For_Valid_Request()
        {
            // Arrange
            var client = Factory.CreateClient().WithNgoAdminAuthorizationPolicy();
            await WithNgoAdmin(new
            {
                account = "NgoAdmin",
                password = "Lkjn/1QQ"
            });

            // Act
            var response = await client.GetAsync("/api/v1/ngo/1/ngoadmin");

            // Assert
            response.Should().Be200Ok()
                .And.BeAs(new[]
                {
                    new
                    {
                        id = 1,
                        account = "NgoAdmin"
                    }
                });
        }
    }
}