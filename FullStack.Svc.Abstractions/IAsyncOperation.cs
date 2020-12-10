// <copyright file="IAsyncOperation.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc.Abstractions
{
    using System.Threading.Tasks;

    /// <summary>
    /// Contract for that which performs an operation asynchronously.
    /// </summary>
    /// <typeparam name="TReq">The request type.</typeparam>
    /// <typeparam name="TRes">The response type.</typeparam>
    public interface IAsyncOperation<TReq, TRes>
    {
        /// <summary>
        /// Performs the operation.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        Task<TRes> OperateAsync(TReq request);
    }
}
