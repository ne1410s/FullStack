// <copyright file="ChurnExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Crypto
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Extensions relating to cryptography.
    /// </summary>
    public static class ChurnExtensions
    {
        private static readonly Regex EXT_REGEX = new Regex(@"\.enc$");

        /// <summary>
        /// Applies a cryptographic process to a file.
        /// </summary>
        /// <param name="sourceInfo">The source file.</param>
        /// <param name="salt">The salt bytes.</param>
        /// <param name="pass">The pass bytes.</param>
        /// <param name="params">The churn parameters.</param>
        /// <param name="targetName">Optional file name override.</param>
        public static void Churn(
            this FileInfo sourceInfo,
            byte[] salt,
            byte[] pass,
            FileChurnParams @params = FileChurnParams.Defaults,
            string targetName = null)
        {
            sourceInfo.AssertExists();

            var direction = @params.HasFlag(FileChurnParams.IsDecrypt)
                ? ChurnDirection.Decrypt
                : ChurnDirection.Encrypt;

            var targetPath = targetName != null
                ? Path.Combine(sourceInfo.DirectoryName, targetName)
                : direction == ChurnDirection.Encrypt
                    ? $"{sourceInfo.FullName}.enc"
                    : EXT_REGEX.Replace(sourceInfo.FullName, string.Empty);
            var targetInfo = new FileInfo(targetPath);

            var useMac = @params.HasFlag(FileChurnParams.SubjectAuthentication);
            var isGcm = !@params.HasFlag(FileChurnParams.IsCcm);
            var macRelative = direction == ChurnDirection.Encrypt ? targetInfo : sourceInfo;
            var macFilePath = $"{macRelative.FullName}.gmac";

            Func<string, Stream> opener = !useMac ? null : direction == ChurnDirection.Encrypt
                ? File.OpenWrite
                : File.OpenRead;

            if (!targetInfo.Exists
                || @params.HasFlag(FileChurnParams.RedoTarget)
                || (useMac && direction == ChurnDirection.Encrypt && !File.Exists(macFilePath)))
            {
                targetInfo.Delete();
                try
                {
                    using var outstr = targetInfo.OpenWrite();
                    using var instr = sourceInfo.OpenRead();
                    using var gmacStream = opener?.Invoke(macFilePath);
                    instr.Churn(outstr, direction, isGcm, salt, pass, gmacStream);
                }
                catch
                {
                    targetInfo.Delete();
                    throw;
                }
            }

            if (!@params.HasFlag(FileChurnParams.KeepSource))
            {
                sourceInfo.Delete();
            }
        }

        /// <summary>
        /// Writes a cryptographic operation to a target stream, and resets the
        /// position of the target stream to its beginning.
        /// </summary>
        /// <param name="source">The source (caller-managed).</param>
        /// <param name="target">The target (caller-managed).</param>
        /// <param name="direction">The direction.</param>
        /// <param name="salt">The salt bytes.</param>
        /// <param name="pass">The pass bytes.</param>
        /// <param name="gmacStream">Message authentication code stream.</param>
        public static void Churn(
            this Stream source,
            Stream target,
            ChurnDirection direction,
            bool isGcm,
            byte[] salt,
            byte[] pass,
            Stream gmacStream)
        {
            var srcBuffer = new byte[32768];
            var tagBuffer = new byte[16];
            var counter = new byte[12];
            var encrypt = direction == ChurnDirection.Encrypt;

            source.Seek(0, SeekOrigin.Begin);
            target.SetLength(0);

            int readSize;
            var uniqueKey = pass.Concat(salt).ToArray();
            using var churner = GetChurner(uniqueKey, isGcm);
            while ((readSize = source.Read(srcBuffer, 0, srcBuffer.Length)) != 0)
            {
                if (readSize != srcBuffer.Length)
                {
                    Array.Resize(ref srcBuffer, readSize);
                }

                ByteExtensions.Increment(ref counter);

                if (!encrypt)
                {
                    gmacStream?.Read(tagBuffer, 0, tagBuffer.Length);
                }

                var selfSymmetric = encrypt || gmacStream == null;
                var trgBuffer = churner.ChurnBlock(srcBuffer, tagBuffer, counter, selfSymmetric);

                if (encrypt)
                {
                    gmacStream?.Write(tagBuffer, 0, tagBuffer.Length);
                }

                target.Write(trgBuffer, 0, readSize);
            }

            target.Seek(0, SeekOrigin.Begin);
        }

        private static IBlockChurner GetChurner(byte[] uniqueKey, bool isGcm)
        {
            return isGcm
                ? new GcmBlockChurner(uniqueKey)
                : new CcmBlockChurner(uniqueKey);
        }
    }
}
