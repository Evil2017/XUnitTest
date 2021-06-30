using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using XUnitTest.Data;
using XUnitTest.ServiceTest.Helpers;

namespace XUnitTest.ServiceTest
{
    public class BasicTests : IClassFixture<WebApplicationFactory<XUnitTest.Mvc.Startup>>
    {
        private readonly WebApplicationFactory<XUnitTest.Mvc.Startup> _factory;

        public BasicTests(WebApplicationFactory<XUnitTest.Mvc.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        //[InlineData("/")]
        //[InlineData("/Home/Index")]
        //[InlineData("/Home/Privacy")]
        //[InlineData("/Home/Error")]
        [InlineData("/Users/Index")]
        [InlineData("/Users/Details")]
        [InlineData("/Users/Create")]
        [InlineData("/Users/Edit")]
        [InlineData("/Users/Delete")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
        [Fact]
        public async Task Post_DeleteMessageHandler_ReturnsRedirectToRoot()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var serviceProvider = services.BuildServiceProvider();

                    using (var scope = serviceProvider.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices
                            .GetRequiredService<ApplicationDbContext>();
                        var logger = scopedServices
                            .GetRequiredService<ILogger<UsersTests>>();

                        try
                        {
                            Utilities.ReinitializeDbForTests(db);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "An error occurred seeding " +
                                "the database with test messages. Error: {Message}",
                                ex.Message);
                        }
                    }
                });
            })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
            var defaultPage = await client.GetAsync("/");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            //Act
            var response = await client.SendAsync(
                (IHtmlFormElement)content.QuerySelector("form[id='messages']"),
                (IHtmlButtonElement)content.QuerySelector("form[id='messages']")
                    .QuerySelector("div[class='panel-body']")
                    .QuerySelector("button"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/", response.Headers.Location.OriginalString);
        }

    }
}
