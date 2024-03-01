using Application.Auth.Commands;
using Application.Commands;
using ApplicationTests.Utils;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ApplicationTests.API
{
    [TestClass]
    public class PlaceBetEndpointTest : TestAppServer
    {
        private static string _token = "";
        private static readonly string _username = "root";

        public PlaceBetEndpointTest() : base()
        {
        }

        public async Task<string> GetToken()
        {
            if (string.IsNullOrEmpty(_token))
            {
                var payload = JsonSerializer.Serialize(new AuthenticationCommand { Username = _username, Password = "Root123!" });
                var context = await new TestAppServer()
                    .CreateClient()
                    .SendAsync(new()
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("/authenticate", UriKind.Relative),
                        Content = new StringContent(payload, Encoding.UTF8, "application/json")
                    });
                var response = await context.Content.ReadFromJsonAsync<AuthenticationCommandResponse>();
                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    _token = response.Token;
                }
            }
            return _token;
        }

        [TestMethod]
        public async Task TestPlaceBetSuccess()
        {
            // Arrange
            var payload = JsonSerializer.Serialize(new PlaceBetCommand { Username = _username, Number = 1, Points = 100 });
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetToken());

            // Act
            var context = await client.SendAsync(new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/bet", UriKind.Relative),
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            });

            var response = await context.Content.ReadFromJsonAsync<PlaceBetCommandResponse>();

            // Assert
            Assert.IsTrue(context.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, context.StatusCode);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task TestPlaceBetInvalidNumber()
        {
            // Arrange
            var payload = JsonSerializer.Serialize(new PlaceBetCommand { Username = _username, Number = -1, Points = 100 });
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetToken());

            // Act
            var context = await client.SendAsync(new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/bet", UriKind.Relative),
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            });

            var response = await context.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, context.StatusCode);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Contains("Number"));
        }

        [TestMethod]
        public async Task TestPlaceBetInvalidPoints()
        {
            // Arrange
            var payload = JsonSerializer.Serialize(new PlaceBetCommand { Username = _username, Number = 1, Points = -100 });
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetToken());

            // Act
            var context = await client.SendAsync(new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/bet", UriKind.Relative),
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            });

            var response = await context.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, context.StatusCode);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Contains("Points"));
        }
    }
}
