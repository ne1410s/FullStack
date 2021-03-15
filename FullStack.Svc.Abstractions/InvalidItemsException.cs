// <copyright file="InvalidItemsException.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc.Abstractions
{
    using System;
    using System.Collections.Generic;
    using FullStack.Validity;

    /// <summary>
    /// Error where at least one of an item's members are deemed invalid.
    /// </summary>
    public class InvalidItemsException : OperationException
    {
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="InvalidItemsException"/> class.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <param name="operationData">>Data regarding the operation.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The root cause.</param>
        public InvalidItemsException(
            IList<InvalidItem> errors,
            OperationData operationData,
            string message = null,
            Exception inner = null)
                : base(operationData, message, inner)
        {
            this.Errors = errors;
        }

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="InvalidItemsException"/> class.
        /// </summary>
        /// <param name="operationData">Operation data.</param>
        /// <param name="message">A message.</param>
        /// <param name="inner">An inner exception.</param>
        public InvalidItemsException(
            OperationData operationData,
            string message = null,
            Exception inner = null)
                : base(operationData, message, inner)
        { }

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="InvalidItemsException"/> class.
        /// </summary>
        public InvalidItemsException()
        { }

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="InvalidItemsException"/> class.
        /// </summary>
        /// <param name="message">A message.</param>
        public InvalidItemsException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="InvalidItemsException"/> class.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <param name="innerException">Inner exception.</param>
        public InvalidItemsException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        public IList<InvalidItem> Errors { get; }
    }
}
