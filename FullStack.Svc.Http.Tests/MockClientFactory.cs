using System.Net.Http;
using System.Threading;
using FullStack.Svc.Http.Tests.Models;
using Moq;

namespace FullStack.Svc.Http.Tests
{
    public static class MockClientFactory
    {
        public static HttpClient Create(MockHttpResponse response)
        {
            var clientMock = new Mock<HttpClient>();

            clientMock
                .Setup(m => m.SendAsync(
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new HttpResponseMessage
                {
                    Content = new StringContent(response.Body),
                    StatusCode = response.Status,
                });

            return clientMock.Object;
        }
    }
}
