// <copyright file="CryptExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Crypto
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using FullStack.Crypto.Hash;
    using FullStack.Extensions.Text.Codec;

    /// <summary>
    /// Extensions for cryptography.
    /// </summary>
    public static class CryptExtensions
    {
        private const int TagLength = 16;
        private const int PepperLength = 32;
        private static readonly byte[] CtrPad = { 0, 0, 0, 0 };

        /// <summary>
        /// Encrypts a file in-situ, optionally writing a MAC if stream passed.
        /// </summary>
        /// <param name="fi">The file.</param>
        /// <param name="key">The key.</param>
        /// <param name="bufferLength">Buffer length.</param>
        /// <param name="mac">Optional write stream to capture MAC.</param>
        public static void Encrypt(this FileInfo fi, byte[] key, int bufferLength = 32768, Stream mac = null)
        {
            string name;
            var counter = new byte[12];
            var macBuffer = new byte[TagLength];
            var srcBuffer = new byte[bufferLength];
            var trgBuffer = new byte[bufferLength];

            using (var source = fi.Open(FileMode.Open))
            {
                var salt = source.Hash(HashAlgo.Sha256);
                source.Position = 0;
                Array.Reverse(salt, 0, 8);
                Array.Reverse(salt, 5, salt.Length - 5);
                Array.Reverse(salt);
                name = salt.AsString(ByteCodec.Hex);

                int readSize;
                var pepper = new byte[PepperLength];
                RandomNumberGenerator.Fill(pepper);
                using var aes = key.GenerateAes(salt, pepper);
                while ((readSize = source.Read(srcBuffer, 0, srcBuffer.Length)) != 0)
                {
                    ByteExtensions.Increment(ref counter);
                    var lastRead = readSize < bufferLength;
                    if (lastRead)
                    {
                        Array.Resize(ref srcBuffer, readSize);
                        Array.Resize(ref trgBuffer, readSize);
                    }

                    aes.Encrypt(counter, srcBuffer, trgBuffer, macBuffer);
                    mac?.Write(macBuffer, 0, macBuffer.Length);
                    source.Position -= readSize;
                    source.Write(trgBuffer, 0, trgBuffer.Length);
                }

                source.Write(pepper);
            }

            var target = Path.Combine(fi.DirectoryName, $"{name}{fi.Extension}").ToLower();
            fi.MoveTo(target, true);
        }

        /// <summary>
        /// Decrypts a file to stream.
        /// </summary>
        /// <param name="fi">The file.</param>
        /// <param name="key">The key.</param>
        /// <param name="target">The target stream.</param>
        /// <param name="bufferLength">The buffer length.</param>
        /// <param name="mac">Optional MAC stream. Enables the caller to check
        /// the authenticity of the message if they so wish.</param>
        public static void Decrypt(
            this FileInfo fi,
            byte[] key,
            Stream target,
            int bufferLength = 32768,
            Stream mac = null)
        {
            var macBuffer = GenerateMacBuffer();
            var srcBuffer = new byte[bufferLength];
            var trgBuffer = new byte[bufferLength];
            var salt = fi.GenerateSalt();

            target.SetLength(0);
            using var source = fi.OpenRead();
            var pepper = source.GeneratePepper();
            using var aes = key.GenerateAes(salt, pepper);
            var totalBlocks = (long)Math.Ceiling((fi.Length - PepperLength) / (double)bufferLength);

            for (var b = 0; b < totalBlocks; b++)
            {
                mac?.Read(macBuffer, 0, macBuffer.Length);
                var read = aes.DecryptBlock(source, mac != null, srcBuffer, macBuffer, trgBuffer);
                target.Write(trgBuffer, 0, read);
            }
        }

        /// <summary>
        /// Generates a MAC-capable AES-GCM cipher.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="pepper">The pepper.</param>
        /// <returns>The cipher.</returns>
        public static AesGcm GenerateAes(this byte[] key, byte[] salt, byte[] pepper)
        {
            var keyBytesRaw = pepper.Concat(key).Concat(salt.Reverse()).ToArray();
            return new AesGcm(keyBytesRaw.Hash(HashAlgo.Sha256));
        }

        /// <summary>
        /// Generates a salt from a file.
        /// </summary>
        /// <param name="fi">The file.</param>
        /// <returns>A salt.</returns>
        public static byte[] GenerateSalt(this FileInfo fi)
        {
            return fi.Name.Substring(0, 64).AsBytes(ByteCodec.Hex);
        }

        /// <summary>
        /// Generates a pepper from a stream.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>A pepper.</returns>
        public static byte[] GeneratePepper(this Stream source)
        {
            var pepper = new byte[PepperLength];
            source.Seek(-PepperLength, SeekOrigin.End);
            source.Read(pepper, 0, pepper.Length);
            source.Seek(0, SeekOrigin.Begin);
            return pepper;
        }

        /// <summary>
        /// Generates a buffer of suitable length for use in MAC processes.
        /// </summary>
        /// <returns>The mac buffer.</returns>
        public static byte[] GenerateMacBuffer()
        {
            return new byte[TagLength];
        }

        /// <summary>
        /// Decrypts a block after reading it from the stream current position.
        /// </summary>
        /// <param name="aes">The aes cipher.</param>
        /// <param name="source">The source stream.</param>
        /// <param name="authenticate">Whether to authenticate.</param>
        /// <param name="srcBuffer">The ciphertext block.</param>
        /// <param name="macBuffer">The mac buffer. If authenticating, this must
        /// be pre-filled with appropriate bytes.</param>
        /// <param name="trgBuffer">The plaintext block.</param>
        /// <returns>The bytes read.</returns>
        public static int DecryptBlock(
            this AesGcm aes,
            Stream source,
            bool authenticate,
            byte[] srcBuffer,
            byte[] macBuffer,
            byte[] trgBuffer)
        {
            var length = source.Length - PepperLength;
            var position = source.Position;
            var maxReadSize = Math.Min(length - position, srcBuffer.Length);
            var readSize = source.Read(srcBuffer, 0, (int)maxReadSize);

            var blockNumber = 1 + (long)Math.Floor((double)position / srcBuffer.Length);
            var countBytes = BitConverter.GetBytes(blockNumber);
            var counter = BitConverter.IsLittleEndian
                ? countBytes.Concat(CtrPad).ToArray()
                : CtrPad.Concat(countBytes).ToArray();

            if (authenticate)
            {
                aes.Decrypt(counter, srcBuffer.AsSpan(0, readSize), macBuffer, trgBuffer.AsSpan(0, readSize));
            }
            else
            {
                aes.Encrypt(counter, srcBuffer, trgBuffer, macBuffer);
            }

            return readSize;
        }
    }
}
