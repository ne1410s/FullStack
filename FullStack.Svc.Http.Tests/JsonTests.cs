using System.Net.Http;
using System.Threading.Tasks;
using FullStack.Svc.Abstractions;
using FullStack.Svc.Http.Tests.Models;
using Xunit;

namespace FullStack.Svc.Http.Tests
{
    public class JsonTests
    {
        private readonly HttpClient clientJson200;
        private readonly HttpClient clientText400;

        public JsonTests()
        {
            this.clientJson200 = MockClientFactory.Create(new MockHttpResponse
            {
                Status = System.Net.HttpStatusCode.OK,
                Body = "{ \"Hits\": [ \"hit1\", \"hit2\", \"hit3\" ] }",
            });

            this.clientText400 = MockClientFactory.Create(new MockHttpResponse
            {
                Status = System.Net.HttpStatusCode.BadRequest,
                Body = "The request contained an egregious error",
            });
        }

        [Fact]
        public async Task Json_Ok()
        {
            // Arrange
            var endpoint = new JsonTestEndpoint<TextQuery, HitsResult>(clientJson200);
            var query = new TextQuery { Text = "Leaf blower" };

            // Act
            var result = await endpoint.OperateAsync(query);

            // Assert
            Assert.True(result.Hits.Count > 0);
        }

        [Fact]
        public async Task Json_NotOk()
        {
            // Arrange
            var endpoint = new JsonTestEndpoint<TextPatternQuery, HitsResult>(clientText400);
            var query = new TextPatternQuery { Text = "Almos" };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidItemsException>(() => endpoint.OperateAsync(query));
        }
    }
}
