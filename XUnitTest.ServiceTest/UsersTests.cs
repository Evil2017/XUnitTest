using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using XUnitTest.ServiceTest.Helpers;

namespace XUnitTest.ServiceTest
{
    public class UsersTests : IClassFixture<CustomWebApplicationFactory<XUnitTest.Mvc.Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<XUnitTest.Mvc.Startup>
            _factory;

        public UsersTests(CustomWebApplicationFactory<XUnitTest.Mvc.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
        [Fact]
        public async Task Post_DeleteAllMessagesHandler_ReturnsRedirectToRoot()
        {
            // Arrange
            var defaultPage = await _client.GetAsync("/");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            //Act
            var response = await _client.SendAsync(
                (IHtmlFormElement)content.QuerySelector("form[id='messages']"),
                (IHtmlButtonElement)content.QuerySelector("button[id='deleteAllBtn']"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/", response.Headers.Location.OriginalString);
        }
    }
}
