// <copyright file="OperationStage.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc.Abstractions
{
    /// <summary>
    /// A stage in the mapped operation lifecycle.
    /// </summary>
    public enum OperationStage
    {
        /// <summary>
        /// Checking the outer request.
        /// </summary>
        CheckingRequest = 0,

        /// <summary>
        /// Mapping the request to the inner request model.
        /// </summary>
        MappingRequest = 1,

        /// <summary>
        /// Checking the inner request.
        /// </summary>
        CheckingInnerRequest = 2,

        /// <summary>
        /// Performing the operation.
        /// </summary>
        Operating = 3,

        /// <summary>
        /// Checking the inner response.
        /// </summary>
        CheckingInnerResponse = 4,

        /// <summary>
        /// Mapping the inner response to the final response model.
        /// </summary>
        MappingResponse = 5,

        /// <summary>
        /// Checking the final response.
        /// </summary>
        CheckingResponse = 6,
    }
}
