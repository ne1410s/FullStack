// <copyright file="MappedSyncOperation.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc
{
    using System;
    using FullStack.Svc.Abstractions;

    /// <summary>
    /// Base implementation for a mapped synchronous operation.
    /// </summary>
    /// <typeparam name="TReq">The initial request type.</typeparam>
    /// <typeparam name="TInnerReq">The inner request type.</typeparam>
    /// <typeparam name="TInnerRes">The inner response type.</typeparam>
    /// <typeparam name="TRes">The final response type.</typeparam>
    public abstract class MappedSyncOperation<TReq, TInnerReq, TInnerRes, TRes> :
        MappedOperation<TReq, TInnerReq, TInnerRes, TRes>,
        ISyncOperation<TReq, TRes>
    {
        /// <inheritdoc/>
        public TRes Operate(TReq request)
        {
            var opData = new OperationData { };
            try
            {
                var innerReq = this.PreOp(request, ref opData);
                var innerRes = this.OperateInner(request, innerReq);
                var response = this.PostOp(request, innerReq, innerRes, ref opData);
                return response;
            }
            catch (Exception ex)
            {
                throw (ex is InvalidItemsException invalidEx)
                    ? invalidEx
                    : new OperationException(opData, "Unexpected error", ex);
            }
        }

        /// <summary>
        /// Performs the inner operation.
        /// </summary>
        /// <param name="request">The initial request.</param>
        /// <param name="innerRequest">The inner request.</param>
        /// <returns>The inner response.</returns>
        protected abstract TInnerRes OperateInner(
            TReq request,
            TInnerReq innerRequest);
    }
}
