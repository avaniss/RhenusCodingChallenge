using Application.Commands;
using ApplicationTests.Utils;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ApplicationTests.API
{
    [TestClass]
    public class UserRegistrationEndpointTest : TestAppServer
    {

        public UserRegistrationEndpointTest() : base()
        {
        }

        [TestMethod]
        public async Task testRegistrationSuccess()
        {
            // Arrange
            var name = "testName";
            var username = "testUsername";
            var password = "Pass123!";
            var payload = JsonSerializer.Serialize(new UserRegistrationCommand { Name = name, Username = username, Password = password });

            // Act
            var context = await CreateClient().SendAsync(new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/register", UriKind.Relative),
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            });

            var user = await context.Content.ReadFromJsonAsync<User>();

            // Assert
            Assert.IsTrue(context.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, context.StatusCode);
            Assert.IsNotNull(user);
            Assert.IsTrue(user.Id >= 0);
            Assert.AreEqual(name, user.Name);
            Assert.AreEqual(username, user.Username);
            Assert.AreEqual(password, user.Password);
        }

        [TestMethod]
        public async Task testRegistrationInvalidName()
        {
            // Arrange
            var name = "";
            var username = "testUsername";
            var password = "Pass123!";

            var payload = JsonSerializer.Serialize(new UserRegistrationCommand { Name = name, Username = username, Password = password });

            // Act
            var context = await CreateClient().SendAsync(new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/register", UriKind.Relative),
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            });

            var res = await context.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, context.StatusCode);
            Assert.IsTrue(res.Contains("Name"));
        }

        [TestMethod]
        public async Task testRegistrationInvalidUsername()
        {
            // Arrange
            var name = "test";
            var username = "";
            var password = "Pass123!";

            var payload = JsonSerializer.Serialize(new UserRegistrationCommand { Name = name, Username = username, Password = password });

            // Act
            var context = await CreateClient().SendAsync(new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/register", UriKind.Relative),
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            });

            var res = await context.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, context.StatusCode);
            Assert.IsTrue(res.Contains("Username"));
        }

        [TestMethod]
        public async Task testRegistrationInvalidPassword()
        {
            // Arrange
            var name = "test";
            var username = "testUsername";
            var password = "invalidPass";

            var payload = JsonSerializer.Serialize(new UserRegistrationCommand { Name = name, Username = username, Password = password });

            // Act
            var context = await CreateClient().SendAsync(new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/register", UriKind.Relative),
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            });

            var res = await context.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, context.StatusCode);
            Assert.IsTrue(res.Contains("Password"));
        }
    }
}
