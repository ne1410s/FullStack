// <copyright file="HttpOperation.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc.Http
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base implementation for an http operation.
    /// </summary>
    /// <typeparam name="TReq">The request type.</typeparam>
    /// <typeparam name="TRes">The response type.</typeparam>
    public abstract class HttpOperation<TReq, TRes>
        : MappedAsyncOperation<TReq, HttpRequestMessage, HttpResponseMessage, TRes>
    {
        private readonly HttpClient client;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="HttpOperation{TReq, TRes}"/> class.
        /// </summary>
        /// <param name="baseUrl">The base url.</param>
        /// <param name="method">The http method.</param>
        /// <param name="client">The client.</param>
        public HttpOperation(string baseUrl, HttpMethod method, HttpClient client)
        {
            this.client = client;
            this.Method = method;
            this.BaseUrl = baseUrl;
            this.DefaultUrl = $"{baseUrl?.Trim('/')}/{this.RelativePath?.TrimStart('/')}";
        }

        /// <summary>
        /// Gets the default full url, based on base url and path.
        /// </summary>
        public string DefaultUrl { get; }

        /// <summary>
        /// Gets the http method.
        /// </summary>
        public HttpMethod Method { get; }

        /// <summary>
        /// Gets a list of http methods that may contain body content.
        /// </summary>
        protected static IList<HttpMethod> BodyMethods { get; } = new[]
        {
            HttpMethod.Post,
            HttpMethod.Put,
            HttpMethod.Patch,
        };

        /// <summary>
        /// Gets the base url.
        /// </summary>
        protected string BaseUrl { get; }

        /// <summary>
        /// Gets the relative path.
        /// </summary>
        protected virtual string RelativePath => string.Empty;

        /// <summary>
        /// Gets the accept type.
        /// </summary>
        protected virtual string Accept { get; }

        /// <summary>
        /// Gets the content type.
        /// </summary>
        protected virtual string ContentType { get; }

        /// <inheritdoc/>
        protected override HttpRequestMessage MapIn(TReq request)
        {
            var url = this.PrepareUrl(request);
            var isBodyMethod = BodyMethods.Contains(this.Method);
            var content = isBodyMethod ? this.PrepareContent(request) : null;
            var innerReq = new HttpRequestMessage(this.Method, url)
            {
                Content = content,
                Headers = { },
            };

            var preparedHeaders = this
                .PrepareHeaders(request)
                .Where(h => h.Value != null);

            foreach (var header in preparedHeaders)
            {
                innerReq.Headers.Add(header.Key, header.Value);
            }

            return innerReq;
        }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> OperateInnerAsync(
            TReq request,
            HttpRequestMessage innerRequest)
        {
            return await this.client.SendAsync(innerRequest, default(CancellationToken));
        }

        /// <summary>
        /// Prepares the final url. The default is baseUrl/path.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The full url to use.</returns>
        protected virtual string PrepareUrl(TReq request) => this.DefaultUrl;

        /// <summary>
        /// Prepares the request headers. The default is an empty set.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The request headers to use.</returns>
        protected virtual IDictionary<string, string> PrepareHeaders(TReq request)
        {
            var headers = new Dictionary<string, string>();
            var isBodyMethod = BodyMethods.Contains(this.Method);
            headers["Accept"] = this.Accept;
            headers["Content-Type"] = isBodyMethod ? this.ContentType : null;
            return headers;
        }

        /// <summary>
        /// Prepares the request content. The default is null.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The http content.</returns>
        protected virtual HttpContent PrepareContent(TReq request) => null;
    }
}
