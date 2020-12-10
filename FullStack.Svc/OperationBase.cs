// <copyright file="OperationBase.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc
{
    using System.Collections.Generic;
    using FullStack.Svc.Abstractions;
    using FullStack.Validity;

    /// <summary>
    /// Base implementation for an operation.
    /// </summary>
    /// <typeparam name="TReq">The request type.</typeparam>
    /// <typeparam name="TRes">The response type.</typeparam>
    public abstract class OperationBase<TReq, TRes>
    {
        /// <summary>
        /// Checks the request data for errors.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Any errors.</returns>
        protected virtual IList<InvalidItem> CheckRequest(TReq request)
        {
            request.Validate(out var errors);
            return errors;
        }

        /// <summary>
        /// Checks the response data for errors.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="originalRequest">The original request.</param>
        /// <returns>Any errors.</returns>
        protected virtual IList<InvalidItem> CheckResponse(
            TRes response,
            TReq originalRequest)
        {
            response.Validate(out var errors);
            return errors;
        }

        /// <summary>
        /// Raises an exception if invalid items are found.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <param name="operationData">The operation data.</param>
        /// <param name="message">The message.</param>
        protected void AssertValid(
            IList<InvalidItem> errors,
            OperationData operationData,
            string message)
        {
            if (errors != null && errors.Count != 0)
            {
                throw new InvalidItemsException(errors, operationData, message);
            }
        }
    }
}
