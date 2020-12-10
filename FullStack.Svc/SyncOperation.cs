// <copyright file="SyncOperation.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc
{
    using System;
    using FullStack.Svc.Abstractions;

    /// <summary>
    /// Base implementation for a synchronous operation.
    /// </summary>
    /// <typeparam name="TReq">The request type.</typeparam>
    /// <typeparam name="TRes">The response type.</typeparam>
    public abstract class SyncOperation<TReq, TRes> :
        Operation<TReq, TRes>,
        ISyncOperation<TReq, TRes>
    {
        /// <inheritdoc/>
        public TRes Operate(TReq request)
        {
            var opData = new OperationData { };
            try
            {
                this.PreOp(request, ref opData);
                var response = this.OperateInternal(request);
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

        /// <inheritdoc cref="Operate(TReq)"/>
        protected abstract TRes OperateInternal(TReq request);
    }
}
