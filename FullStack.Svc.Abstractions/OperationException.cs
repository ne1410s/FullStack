// <copyright file="OperationException.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Svc.Abstractions
{
    using System;

    /// <summary>
    /// Errors that occur during mapped operations.
    /// </summary>
    public class OperationException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="OperationException"/> class.
        /// </summary>
        /// <param name="operationData">Data regarding the operation.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The root cause.</param>
        public OperationException(
            OperationData operationData,
            string message = null,
            Exception inner = null)
                : base(message, inner)
        {
            this.OperationData = operationData;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="OperationException"/>
        /// class.
        /// </summary>
        public OperationException()
        { }

        /// <summary>
        /// Initialises a new instance of the <see cref="OperationException"/>
        /// class.
        /// </summary>
        /// <param name="message">A message.</param>
        public OperationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initialises a new instance of the <see cref="OperationException"/>
        /// class.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <param name="innerException">Inner exception.</param>
        public OperationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Gets the operation data yielded thus far.
        /// </summary>
        public OperationData OperationData { get; }
    }
}
