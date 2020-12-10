// <copyright file="MappedOperation.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc
{
    using System.Collections.Generic;
    using FullStack.Svc.Abstractions;
    using FullStack.Validity;

    /// <summary>
    /// Base implementation for a mapped operation.
    /// </summary>
    /// <typeparam name="TReq">The initial request type.</typeparam>
    /// <typeparam name="TInnerReq">The inner request type.</typeparam>
    /// <typeparam name="TInnerRes">The inner response type.</typeparam>
    /// <typeparam name="TRes">The final response type.</typeparam>
    public abstract class MappedOperation<TReq, TInnerReq, TInnerRes, TRes> :
        OperationBase<TReq, TRes>
    {
        /// <summary>
        /// Checks the inner request data for errors.
        /// </summary>
        /// <param name="innerRequest">The inner request.</param>
        /// <param name="originalRequest">The initial request.</param>
        /// <returns>Any errors.</returns>
        protected virtual IList<InvalidItem> CheckInnerRequest(
            TInnerReq innerRequest,
            TReq originalRequest) => null;

        /// <summary>
        /// Checks the inner response data for errors.
        /// </summary>
        /// <param name="innerResponse">The inner response.</param>
        /// <param name="innerRequest">The inner request.</param>
        /// <param name="originalRequest">The initial request.</param>
        /// <returns>Any errors.</returns>
        protected virtual IList<InvalidItem> CheckInnerResponse(
            TInnerRes innerResponse,
            TInnerReq innerRequest,
            TReq originalRequest) => null;

        /// <summary>
        /// Maps the initial request to an inner request.
        /// </summary>
        /// <param name="request">The initial request.</param>
        /// <returns>The inner request.</returns>
        protected abstract TInnerReq MapIn(TReq request);

        /// <summary>
        /// Maps to the final response.
        /// </summary>
        /// <param name="innerResponse">The inner response.</param>
        /// <param name="innerRequest">The inner request.</param>
        /// <param name="originalRequest">The initial request.</param>
        /// <returns>The final response.</returns>
        protected abstract TRes MapOut(
            TInnerRes innerResponse,
            TInnerReq innerRequest,
            TReq originalRequest);

        /// <summary>
        /// Performs pre-operation actions.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="opData">Operation data.</param>
        /// <returns>The inner request.</returns>
        protected TInnerReq PreOp(TReq request, ref OperationData opData)
        {
            opData.Stage = OperationStage.CheckingRequest;
            opData.Request = request;
            var requestErrors = this.CheckRequest(request);
            this.AssertValid(requestErrors, opData, "Initial request is invalid");

            opData.Stage = OperationStage.MappingRequest;
            var innerReq = this.MapIn(request);

            opData.Stage = OperationStage.CheckingInnerRequest;
            opData.InnerRequest = innerReq;
            var innerReqErrors = this.CheckInnerRequest(innerReq, request);
            this.AssertValid(innerReqErrors, opData, "Inner request is invalid");

            return innerReq;
        }

        /// <summary>
        /// Performs post-operation actions.
        /// </summary>
        /// <param name="originalRequest">The request.</param>
        /// <param name="innerReq">The inner request.</param>
        /// <param name="innerRes">The inner response.</param>
        /// <param name="opData">Operation data.</param>
        /// <returns>The final response.</returns>
        protected TRes PostOp(
            TReq originalRequest,
            TInnerReq innerReq,
            TInnerRes innerRes,
            ref OperationData opData)
        {
            opData.Stage = OperationStage.CheckingInnerResponse;
            opData.InnerResponse = innerRes;
            var innerResErrors = this.CheckInnerResponse(innerRes, innerReq, originalRequest);
            this.AssertValid(innerResErrors, opData, "Inner response is invalid");

            opData.Stage = OperationStage.MappingResponse;
            var response = this.MapOut(innerRes, innerReq, originalRequest);

            opData.Stage = OperationStage.CheckingResponse;
            opData.Response = response;
            var responseErrors = this.CheckResponse(response, originalRequest);
            this.AssertValid(responseErrors, opData, "Final response is invalid");

            return response;
        }
    }
}
