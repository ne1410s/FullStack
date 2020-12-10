using System.Collections.Generic;
using System.Net.Http;
using FullStack.Validity;

namespace FullStack.Svc.Http.Tests.Models
{
    public class JsonTestEndpoint<TQuery, TResult> : JsonHttpOperation<TQuery, TResult>
        where TQuery : ITextQuery
        where TResult : IHitsResult
    {
        public JsonTestEndpoint(HttpClient client)
            : base("https://www.google.com", HttpMethod.Get, client)
        { }

        protected override string PrepareUrl(TQuery request) =>
            $"{this.BaseUrl}?q={request.Text}";

        protected override TResult MapOut(
            HttpResponseMessage innerResponse,
            HttpRequestMessage innerRequest,
            TQuery originalRequest)
        {
            return base.MapOut(innerResponse, innerRequest, originalRequest);
        }
    }
}
