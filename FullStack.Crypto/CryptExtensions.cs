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
        private static readonly byte[] CtrPad = { 0, 0, 0, 0 };

        public static void Encrypt(this FileInfo fi, byte[] key, int bufferLength = 32768)
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
                using var aes = key.GenerateAes(salt);
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
            var originalSize = fi.GetOriginalSize(bufferLength);
            var macBuffer = new byte[TagLength];
            var srcBuffer = new byte[bufferLength];
            var trgBuffer = new byte[bufferLength];
            var salt = fi.Name.Substring(0, 64).AsBytes(ByteCodec.Hex);

            target.SetLength(0);
            using var source = fi.OpenRead();
            using var aes = key.GenerateAes(salt);
            var totalBlocks = (long)Math.Ceiling(originalSize / (double)bufferLength);
            for (var b = 0; b < totalBlocks; b++)
            {
                var read = aes.DecryptBlock(source, originalSize, srcBuffer, macBuffer, trgBuffer);
                target.Write(trgBuffer, 0, read);
            }
        }

        public static long GetOriginalSize(this FileInfo fi, int bufferLength)
        {
            var denominator = (double)fi.Length / (bufferLength + TagLength);
            var macSize = (int)Math.Ceiling(denominator) * TagLength;
            return fi.Length - macSize;
        }

        public static AesGcm GenerateAes(this byte[] key, byte[] salt)
        {
            var keyBytesRaw = key.Concat(salt.Reverse()).ToArray();
            return new AesGcm(keyBytesRaw.Hash(HashAlgo.Sha256));
        }

        public static int DecryptBlock(
            this AesGcm aes,
            Stream source,
            long originalSize,
            byte[] srcBuffer,
            byte[] macBuffer,
            byte[] trgBuffer)
        {
            var readSize = source.Read(srcBuffer, 0, srcBuffer.Length);

            var position = source.Position;
            var blockNumber = (long)Math.Ceiling((double)position / srcBuffer.Length);
            var finalBlock = position > originalSize;
            if (finalBlock)
            {
                readSize = (int)(originalSize % srcBuffer.Length);
                Array.Resize(ref srcBuffer, readSize);
                Array.Resize(ref trgBuffer, readSize);
            }

            var countBytes = BitConverter.GetBytes(blockNumber);
            var counter = BitConverter.IsLittleEndian
                ? countBytes.Concat(CtrPad).ToArray()
                : CtrPad.Concat(countBytes).ToArray();

            source.Position = originalSize + (TagLength * (blockNumber - 1));
            source.Read(macBuffer, 0, TagLength);
            source.Position = position;

            aes.Decrypt(counter, srcBuffer, macBuffer, trgBuffer);
            return readSize;
        }
    }
}
