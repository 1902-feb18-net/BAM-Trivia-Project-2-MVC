using BAMTriviaProject2MVC.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration.Json;
using BAMTriviaProject2MVC.AuthModels;

namespace BAMTriviaProject2MVC.Testing
{
    public class HomeControllerTest
    {
        [Fact]
        public async Task<int> TestLoginUnSuccessful()
        {
            
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("[{'id':1,'value':'1'}]"),
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://bam1902trivia.azurewebsites.net/"),
            };
            //IConfiguration configuration = Mock.Of<IConfiguration>();
            IConfiguration configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              //.AddEnvironmentVariables()
              .Build();
            ILogger<HomeController> logger = Mock.Of<ILogger<HomeController>>();
            //Mock<IConfiguration> configuration = new Mock<IConfiguration>();
            //configuration.Setup(c => c.GetSection(It.IsAny<String>())).Returns(new Mock<IConfigurationSection>().Object);

            var sut = new HomeController(httpClient, configuration, logger);

            HttpResponseMessage apiResponse = new HttpResponseMessage();
            apiResponse.Headers.Add("Set-Cookie", "test");

            AuthLogin login = new AuthLogin
            {
                Username = "mpkagel",
                Password = "Gummies7%",
                RememberMe = true
            };

            await Assert.ThrowsAsync<System.IO.FileNotFoundException>(() => sut.Login(login));
            //await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Login(login));

            return 1;
            //private bool PassCookiesToClient(HttpResponseMessage apiResponse)
            //{
            //    // the header value contains both the name and value of the cookie itself
            //    if (apiResponse.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values) &&
            //        values.FirstOrDefault(x => x.StartsWith(Configuration["ServiceCookieName"])) is string headerValue)
            //    {
            //        // copy that cookie to the response we will send to the client
            //        Response.Headers.Add("Set-Cookie", headerValue);
            //        return true;
            //    }
            //    return false;
            //}


            //         var result = await sut
            //.GetSomethingRemoteAsync('api/test/whatever');

            // ASSERT
            //result.Should().NotBeNull(); // this is fluent assertions here...
            //result.Id.Should().Be(1);

            // also check the 'http' call was like we expected it
            //var expectedUri = new Uri("http://test.com/api/test/whatever");

            //handlerMock.Protected().Verify(
            //   "SendAsync",
            //   Times.Exactly(1), // we expected a single external request
            //   ItExpr.Is<HttpRequestMessage>(req =>
            //      req.Method == HttpMethod.Get  // we expected a GET request
            //      && req.RequestUri == expectedUri // to this uri
            //   ),
            //   ItExpr.IsAny<CancellationToken>()
            //);

        }

    }
}
