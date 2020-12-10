// <copyright file="Operation.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc
{
    using FullStack.Svc.Abstractions;

    /// <summary>
    /// Base implementation for an operation.
    /// </summary>
    /// <typeparam name="TReq">The request type.</typeparam>
    /// <typeparam name="TRes">The response type.</typeparam>
    public abstract class Operation<TReq, TRes> : OperationBase<TReq, TRes>
    {
        /// <summary>
        /// Performs pre-operation actions.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="opData">Operation data.</param>
        protected void PreOp(TReq request, ref OperationData opData)
        {
            opData.Stage = OperationStage.CheckingRequest;
            opData.Request = request;
            var requestErrors = this.CheckRequest(request);
            this.AssertValid(requestErrors, opData, "Request is invalid");
        }

        /// <summary>
        /// Performs post-operation actions.
        /// </summary>
        /// <param name="originalRequest">The request.</param>
        /// <param name="response">The response.</param>
        /// <param name="opData">Operation data.</param>
        protected void PostOp(
            TReq originalRequest,
            TRes response,
            ref OperationData opData)
        {
            opData.Stage = OperationStage.CheckingResponse;
            opData.Response = response;
            var responseErrors = this.CheckResponse(response, originalRequest);
            this.AssertValid(responseErrors, opData, "Response is invalid");
        }
    }
}
