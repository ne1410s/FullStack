// <copyright file="CryptoExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Extensions.Crypto.Core
{
    using System.IO;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using FullStack.Extensions.Crypto.Assert;
    using FullStack.Extensions.Crypto.Hash;

    /// <summary>
    /// Extensions relating to cryptography.
    /// </summary>
    public static class CryptoExtensions
    {
        private static readonly Regex EXT_REGEX = new Regex(@"\.enc$");

        /// <summary>
        /// Applies a cryptographic process to a file.
        /// </summary>
        /// <param name="fi">The source file.</param>
        /// <param name="salt">The salt bytes.</param>
        /// <param name="pass">The pass bytes.</param>
        /// <param name="keyIterations">The number of key iterations.</param>
        /// <param name="params">The churn parameters.</param>
        /// <param name="targetName">Optional file name override.</param>
        /// <returns>An asynchronous task.</returns>
        public static async Task ChurnAsync(
            this FileInfo fi,
            byte[] salt,
            byte[] pass,
            int keyIterations,
            FileChurnParams @params = FileChurnParams.Defaults,
            string targetName = null)
        {
            fi.AssertExists();

            var mode = @params.HasFlag(FileChurnParams.IsDecrypt)
                ? CryptoMode.Decrypt
                : CryptoMode.Encrypt;

            var targetPath = targetName != null
                ? Path.Combine(fi.DirectoryName, targetName)
                : mode == CryptoMode.Encrypt
                    ? $"{fi.FullName}.enc"
                    : EXT_REGEX.Replace(fi.FullName, string.Empty);

            var targetInfo = new FileInfo(targetPath);
            if (!targetInfo.Exists || @params.HasFlag(FileChurnParams.RedoTarget))
            {
                targetInfo.Delete();
                try
                {
                    using var outstr = targetInfo.OpenWrite();
                    using var instr = fi.OpenRead();
                    await instr.ChurnAsync(outstr, mode, salt, pass, keyIterations);
                }
                catch
                {
                    targetInfo.Delete();
                    throw;
                }
            }

            if (!@params.HasFlag(FileChurnParams.KeepSource))
            {
                fi.Delete();
            }
        }

        /// <summary>
        /// Applies a cryptographic process to a file.
        /// </summary>
        /// <param name="fi">The source file.</param>
        /// <param name="salt">The salt bytes.</param>
        /// <param name="pass">The pass bytes.</param>
        /// <param name="keyIterations">The number of key iterations.</param>
        /// <param name="params">The churn parameters.</param>
        /// <param name="targetName">Optional file name override.</param>
        public static void Churn(
            this FileInfo fi,
            byte[] salt,
            byte[] pass,
            int keyIterations,
            FileChurnParams @params = FileChurnParams.Defaults,
            string targetName = null)
        {
            fi.AssertExists();

            var mode = @params.HasFlag(FileChurnParams.IsDecrypt)
                ? CryptoMode.Decrypt
                : CryptoMode.Encrypt;

            var targetPath = targetName != null
                ? Path.Combine(fi.DirectoryName, targetName)
                : mode == CryptoMode.Encrypt
                    ? $"{fi.FullName}.enc"
                    : EXT_REGEX.Replace(fi.FullName, string.Empty);

            var targetInfo = new FileInfo(targetPath);
            if (!targetInfo.Exists || @params.HasFlag(FileChurnParams.RedoTarget))
            {
                targetInfo.Delete();
                try
                {
                    using var outstr = targetInfo.OpenWrite();
                    using var instr = fi.OpenRead();
                    instr.Churn(outstr, mode, salt, pass, keyIterations);
                }
                catch
                {
                    targetInfo.Delete();
                    throw;
                }
            }

            if (!@params.HasFlag(FileChurnParams.KeepSource))
            {
                fi.Delete();
            }
        }

        /// <summary>
        /// Hashes a file over evenly distributed file chunks.
        /// </summary>
        /// <remarks>Highly insecure, cryptographically.</remarks>
        /// <param name="fi">The file.</param>
        /// <param name="algo">The hash algorithm.</param>
        /// <param name="reads">The number of distributed reads.</param>
        /// <param name="chunkSize">The chunk size.</param>
        /// <returns>A byte array.</returns>
        public static byte[] LightHash(
            this FileInfo fi,
            HashAlgo algo,
            int reads = 20,
            int chunkSize = 4096)
        {
            using var str = fi.OpenRead();
            return str.LightHash(algo, reads, chunkSize);
        }

        /// <summary>
        /// Writes a cryptographic operation to a target stream, and resets the
        /// position of the target stream to its beginning.
        /// </summary>
        /// <param name="source">The source (caller-managed).</param>
        /// <param name="target">The target (caller-managed).</param>
        /// <param name="mode">The crypt mode.</param>
        /// <param name="salt">The salt bytes.</param>
        /// <param name="pass">The pass bytes.</param>
        /// <param name="keyIterations">The number of key iterations.</param>
        /// <returns>An asynchronous task.</returns>
        public static async Task ChurnAsync(
            this Stream source,
            Stream target,
            CryptoMode mode,
            byte[] salt,
            byte[] pass,
            int keyIterations)
        {
            source.AssertReadable();
            target.AssertWriteable();

            ICryptoTransform cryptor;
            var rfc = new Rfc2898DeriveBytes(pass, salt, keyIterations);
            using (var aes = new AesManaged { Padding = PaddingMode.PKCS7 })
            {
                aes.IV = rfc.GetBytes(aes.IV.Length);
                aes.Key = rfc.GetBytes(aes.Key.Length);
                cryptor = mode == CryptoMode.Encrypt
                        ? aes.CreateEncryptor()
                        : aes.CreateDecryptor();
            }

            using (var crypto = new CryptoStream(source, cryptor, CryptoStreamMode.Read))
            {
                target.SetLength(0);
                await crypto.CopyToAsync(target);
            }

            target.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Writes a cryptographic operation to a target stream, and resets the
        /// position of the target stream to its beginning.
        /// </summary>
        /// <param name="source">The source (caller-managed).</param>
        /// <param name="target">The target (caller-managed).</param>
        /// <param name="mode">The crypt mode.</param>
        /// <param name="salt">The salt bytes.</param>
        /// <param name="pass">The pass bytes.</param>
        /// <param name="keyIterations">The number of key iterations.</param>
        public static void Churn(
            this Stream source,
            Stream target,
            CryptoMode mode,
            byte[] salt,
            byte[] pass,
            int keyIterations)
        {
            source.AssertReadable();
            target.AssertWriteable();

            ICryptoTransform cryptor;
            var rfc = new Rfc2898DeriveBytes(pass, salt, keyIterations);
            using (var aes = new AesManaged { Padding = PaddingMode.PKCS7 })
            {
                aes.IV = rfc.GetBytes(aes.IV.Length);
                aes.Key = rfc.GetBytes(aes.Key.Length);
                cryptor = mode == CryptoMode.Encrypt
                        ? aes.CreateEncryptor()
                        : aes.CreateDecryptor();
            }

            using (var crypto = new CryptoStream(source, cryptor, CryptoStreamMode.Read))
            {
                target.SetLength(0);
                crypto.CopyTo(target);
            }

            target.Seek(0, SeekOrigin.Begin);
        }
    }
}
