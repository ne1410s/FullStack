// <copyright file="ISyncOperation.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc.Abstractions
{
    /// <summary>
    /// A contract for that which performs an operation synchronously.
    /// </summary>
    /// <typeparam name="TReq">The request type.</typeparam>
    /// <typeparam name="TRes">The response type.</typeparam>
    public interface ISyncOperation<TReq, TRes>
    {
        /// <summary>
        /// Performs the operation.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        TRes Operate(TReq request);
    }
}
