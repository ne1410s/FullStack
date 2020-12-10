// <copyright file="AsyncOperation.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc
{
    using System;
    using System.Threading.Tasks;
    using FullStack.Svc.Abstractions;

    /// <summary>
    /// Base implementation for an asynchronous operation.
    /// </summary>
    /// <typeparam name="TReq">The request type.</typeparam>
    /// <typeparam name="TRes">The response type.</typeparam>
    public abstract class AsyncOperation<TReq, TRes> :
        Operation<TReq, TRes>,
        IAsyncOperation<TReq, TRes>
    {
        /// <inheritdoc/>
        public async Task<TRes> OperateAsync(TReq request)
        {
            var opData = new OperationData { };
            try
            {
                this.PreOp(request, ref opData);
                var response = await this.OperateInternalAsync(request);
                this.PostOp(request, response, ref opData);
                return response;
            }
            catch (Exception ex)
            {
                throw (ex is InvalidItemsException invalidEx)
                    ? invalidEx
                    : new OperationException(opData, "Unexpected error", ex);
            }
        }

        /// <inheritdoc cref="OperateAsync(TReq)"/>
        protected abstract Task<TRes> OperateInternalAsync(TReq request);
    }
}
