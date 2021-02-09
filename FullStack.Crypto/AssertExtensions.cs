// <copyright file="AssertExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Crypto
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    /// <summary>
    /// Extensions relating to assertions.
    /// </summary>
    internal static class AssertExtensions
    {
        /// <summary>
        /// Asserts that a stream can be appropriately read.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ArgumentException">Stream not readable.</exception>
        public static void AssertReadable(this Stream stream)
        {
            if (stream == null || !stream.CanRead || !stream.CanSeek || stream.Length == 0)
            {
                throw new ArgumentException("Stream not readable");
            }
        }

        /// <summary>
        /// Asserts that a stream can be appropriately written to.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ArgumentException">Stream not writeable.</exception>
        public static void AssertWriteable(this Stream stream)
        {
            if (stream == null || !stream.CanWrite || !stream.CanSeek || stream.Length != 0)
            {
                throw new ArgumentException("Stream not writeable");
            }
        }

        /// <summary>
        /// Asserts that a hashing algorithm transform can be re-used.
        /// </summary>
        /// <param name="hasher">The hashing algorithm.</param>
        /// <exception cref="ArgumentException">Algo not reusable.</exception>
        public static void AssertReusable(this HashAlgorithm hasher)
        {
            if (!hasher.CanReuseTransform)
            {
                throw new ArgumentException("Hash algorithm not reusable");
            }
        }

        /// <summary>
        /// Asserts that a file exists.
        /// </summary>
        /// <param name="fi">The file.</param>
        /// <exception cref="ArgumentException">File not found.</exception>
        public static void AssertExists(this FileInfo fi)
        {
            if (!fi.Exists)
            {
                throw new ArgumentException($"File not found: {fi.FullName}");
            }
        }
    }
}
