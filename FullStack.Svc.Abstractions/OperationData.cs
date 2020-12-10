// <copyright file="OperationData.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc.Abstractions
{
    /// <summary>
    /// Data relating to the mapped operation processing.
    /// </summary>
    public class OperationData
    {
        /// <summary>
        /// Gets or sets the stage reached.
        /// </summary>
        public OperationStage Stage { get; set; }

        /// <summary>
        /// Gets or sets the initial request item.
        /// </summary>
        public object Request { get; set; }

        /// <summary>
        /// Gets or sets the inner request, if available.
        /// </summary>
        public object InnerRequest { get; set; }

        /// <summary>
        /// Gets or sets the inner response, if available.
        /// </summary>
        public object InnerResponse { get; set; }

        /// <summary>
        /// Gets or sets the final response, if available.
        /// </summary>
        public object Response { get; set; }
    }
}
