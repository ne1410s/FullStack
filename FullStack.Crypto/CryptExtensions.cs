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

    public static class CryptExtensions
    {
        private const int TagLength = 16;

        public static void Encrypt(
            this FileInfo fi,
            byte[] key,
            int bufferLength = 32768)
        {
            string name;
            var counter = new byte[12];
            var macBuffer = new byte[TagLength];
            var srcBuffer = new byte[bufferLength];
            var trgBuffer = new byte[bufferLength];

            using (var source = fi.Open(FileMode.Open))
            {
                var sizeBytes = BitConverter.GetBytes(fi.Length);
                var salt = source.Hash(HashAlgo.Sha256);
                source.Position = 0;
                Array.Reverse(salt, 0, 8);
                Array.Reverse(salt, 5, salt.Length - 5);
                Array.Reverse(salt);
                name = salt.AsString(ByteCodec.Hex);

                int readSize;
                using var aes = new AesGcm(key.Concat(sizeBytes).Concat(salt).ToArray().Hash(HashAlgo.Sha256));
                using var mem = new MemoryStream();
                while ((readSize = source.Read(srcBuffer, 0, bufferLength)) != 0)
                {
                    ByteExtensions.Increment(ref counter);
                    var lastRead = readSize < bufferLength;
                    if (lastRead)
                    {
                        Array.Resize(ref srcBuffer, readSize);
                        Array.Resize(ref trgBuffer, readSize);
                    }

                    aes.Encrypt(counter, srcBuffer, trgBuffer, macBuffer);
                    source.Position -= readSize;
                    source.Write(trgBuffer, 0, trgBuffer.Length);
                    mem.Write(macBuffer);

                    if (lastRead) break;
                }

                source.Write(mem.ToArray());
            }

            var target = Path.Combine(fi.DirectoryName, $"{name}{fi.Extension}");
            fi.MoveTo(target.ToLower());
        }

        public static void Decrypt(
            this FileInfo fi,
            byte[] key,
            Stream target,
            int bufferLength = 32768)
        {
            var counter = new byte[12];
            var srcBuffer = new byte[bufferLength];
            var trgBuffer = new byte[bufferLength];

            var macSize = (int)Math.Ceiling((double)fi.Length / (bufferLength + TagLength)) * TagLength;
            var macBytes = new byte[macSize];
            var originalSize = fi.Length - macSize;
            var salt = fi.Name.Substring(0, 64).AsBytes(ByteCodec.Hex);
            var sizeBytes = BitConverter.GetBytes(originalSize);

            using var source = fi.OpenRead();
            source.Position = originalSize;
            source.Read(macBytes, 0, macSize);
            source.Position = 0;
            target.SetLength(0);

            var chunkNo = 0;
            var bytesRead = 0L;
            using var aes = new AesGcm(key.Concat(sizeBytes).Concat(salt).ToArray().Hash(HashAlgo.Sha256));
            while ((bytesRead += source.Read(srcBuffer, 0, bufferLength)) <= (originalSize + bufferLength))
            {
                ByteExtensions.Increment(ref counter);
                var macBufferSpan = macBytes.AsSpan(chunkNo++ * TagLength, TagLength);
                var lastRead = bytesRead > originalSize;
                if (lastRead)
                {
                    var blockSize = (int)(originalSize % bufferLength);
                    Array.Resize(ref srcBuffer, blockSize);
                    Array.Resize(ref trgBuffer, blockSize);
                }

                aes.Decrypt(counter, srcBuffer, macBufferSpan, trgBuffer);
                target.Write(trgBuffer, 0, trgBuffer.Length);

                if (lastRead) break;
            }
        }
    }
}
