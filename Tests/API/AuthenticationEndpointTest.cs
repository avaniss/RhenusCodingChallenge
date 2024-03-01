using Application.Auth.Commands;
using ApplicationTests.Utils;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ApplicationTests.API
{
    [TestClass]
    public class AuthenticationEndpointTest : TestAppServer
    {
        public AuthenticationEndpointTest() : base() { }

        [TestMethod]
        public async Task TestAuthenticationSuccess()
        {
            // Arrange
            var payload = JsonSerializer.Serialize(new AuthenticationCommand { Username = "root", Password = "Root123!" });

            // Act
            var context = await CreateClient().SendAsync(new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/authenticate", UriKind.Relative),
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            });

            var response = await context.Content.ReadFromJsonAsync<AuthenticationCommandResponse>();

            // Assert
            Assert.IsTrue(context.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, context.StatusCode);
            Assert.IsNotNull(response);
            Assert.IsFalse(string.IsNullOrEmpty(response.Token));
            Assert.IsTrue(DateTime.Now < response.Expiry);
        }

        [TestMethod]
        public async Task TestAuthenticationFailure()
        {
            // Arrange
            var payload = JsonSerializer.Serialize(new AuthenticationCommand { Username = "unkownUser", Password = "RandomPass" });

            // Act
            var context = await CreateClient().SendAsync(new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/authenticate", UriKind.Relative),
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            });

            var response = await context.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, context.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(response));
            Assert.IsTrue(response.Contains("verify"));
        }
    }
}
