// <copyright file="CcmBlockChurner.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Crypto
{
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// Churns cryptographic blocks using <see cref="AesCcm"/>.
    /// </summary>
    public class CcmBlockChurner : IBlockChurner
    {
        private readonly AesCcm aes;

        /// <summary>
        /// Initialises a new instance of the <see cref="CcmBlockChurner"/> class.
        /// </summary>
        /// <param name="uniqueKey">A unique key. Length is not especially
        /// important, provided that the key is not re-used.</param>
        public CcmBlockChurner(byte[] uniqueKey)
        {
            this.aes = new AesCcm(SHA256.HashData(uniqueKey));
        }

        /// <summary>
        /// Churns a block.
        /// </summary>
        /// <param name="sourceBuffer">The source message.</param>
        /// <param name="tagBuffer">Optional authentication tag.</param>
        /// <param name="counter">The block counter.</param>
        /// <param name="encryptOrAuthlessDecrypt">True if encrypting or wishing
        /// to decrypt without authenticating a gmac.</param>
        /// <returns>The target message.</returns>
        public byte[] ChurnBlock(
            byte[] sourceBuffer,
            byte[] tagBuffer,
            byte[] counter,
            bool encryptOrAuthlessDecrypt = true)
        {
            var retVal = new byte[sourceBuffer.Length];
            if (encryptOrAuthlessDecrypt)
            {
                this.aes.Encrypt(counter, sourceBuffer, retVal, tagBuffer);
            }
            else
            {
                this.aes.Decrypt(counter, sourceBuffer, tagBuffer, retVal);
            }

            return retVal;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.aes?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
