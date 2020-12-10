// <copyright file="FullStackClient.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc.Http
{
    using System.Net.Http;

    /// <summary>
    /// The thinnest http client wrapper.
    /// </summary>
    public class FullStackClient : HttpClient, IHttpClient
    { }
}
