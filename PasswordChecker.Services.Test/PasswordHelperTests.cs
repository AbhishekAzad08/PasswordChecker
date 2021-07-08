using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Xunit;
using System.Net.Http;
using Moq.Protected;
using System.Threading;
using PasswordChecker.Services.Enums;

namespace PasswordChecker.Services.Test
{
    public class PasswordHelperTests
    {
        private readonly AppSettings _appSettings = new AppSettings
        {
            PwnedApiBaseUrl = "https://api.pwnedpasswords.com",
            BreachEndpoint = "/range/558F5"
        };

        [Theory]
        [InlineData("P@$$w0rd01",PasswordScore.VeryStrong)]
        [InlineData("password", PasswordScore.VeryWeak)]
        [InlineData("Password", PasswordScore.Weak)]
        [InlineData("Password1", PasswordScore.Strong)]
        [InlineData("Passw0rd", PasswordScore.Medium)]
        public  void  Get_PasswordScore_Verify(string input,PasswordScore expected)
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            var passHelperMock = new PasswordHelper(Options.Create(_appSettings), mockFactory.Object);
            var score=passHelperMock.GetPasswordScore(input);
            Assert.Equal(expected, score);
        }

        [Theory]
        [InlineData("P@$$w0rd01", "0DC51F76BB27416FBE236F24C027769897A:54", 54)]
        [InlineData("P@ssw0rd", "2DC183F740EE76F27B78EB39C8AD972A757:57368", 57368)]
        public async Task Get_Password_Breach_Success(string input,string content, int expectedCount)
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();//Mocking of HttpMessageHandler will ensure the client calling actual endpoints are faked by intercepting it 
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content),
                });
            var client = new HttpClient(mockHttpMessageHandler.Object);
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var passHelperMock = new PasswordHelper(Options.Create(_appSettings), mockFactory.Object);
            var breachCount = await passHelperMock.GetPasswordBreachCount(input);
            Assert.Equal(expectedCount, breachCount);
        }
    }
}
