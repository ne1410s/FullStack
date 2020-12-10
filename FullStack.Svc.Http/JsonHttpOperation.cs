// <copyright file="JsonHttpOperation.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc.Http
{
    using System.Net.Http;
    using Newtonsoft.Json;

    /// <summary>
    /// Base implementation for an http operation with a request content type
    /// of application/json.
    /// </summary>
    /// <typeparam name="TReq">The request type.</typeparam>
    /// <typeparam name="TRes">The response type.</typeparam>
    public abstract class JsonHttpOperation<TReq, TRes> : HttpOperation<TReq, TRes>
    {
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="JsonHttpOperation{TReq, TRes}"/> class.
        /// </summary>
        /// <param name="baseUrl">The base url.</param>
        /// <param name="method">The http method.</param>
        /// <param name="client">The client.</param>
        public JsonHttpOperation(string baseUrl, HttpMethod method, HttpClient client)
            : base(baseUrl, method, client)
        { }

        /// <inheritdoc/>
        protected override string Accept => "application/json";

        /// <inheritdoc/>
        protected override string ContentType => "application/json";

        /// <inheritdoc/>
        protected override TRes MapOut(
            HttpResponseMessage innerResponse,
            HttpRequestMessage innerRequest,
            TReq originalRequest)
        {
            //TODO: Doing the async ok?
            var responseJson = innerResponse.Content.ReadAsStringAsync().Result;
            return this.Deserialise<TRes>(responseJson);
        }

        /// <inheritdoc/>
        protected override HttpContent PrepareContent(TReq request)
        {
            var requestJson = this.Serialise(request);
            return new StringContent(requestJson, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Gets a json string from an object.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>A json string.</returns>
        protected string Serialise<T>(T obj)
        {
            //TODO: expose params to use JsonSerSettings
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Gets an object from a json string.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="json">The json.</param>
        /// <returns>An object.</returns>
        protected T Deserialise<T>(string json)
        {
            //TODO: expose params to use JsonSerSettings
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
