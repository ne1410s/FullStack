// <copyright file="ChurnParams.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Extensions.Crypto.Core
{
    using System;

    /// <summary>
    /// Folder churn parameters.
    /// </summary>
    [Flags]
    public enum FolderChurnParams
    {
        /// <summary>
        /// Use defaults.
        /// </summary>
        Defaults = 0 << 0,

        /// <summary>
        /// Non-default option of only inspecting the immediate contents.
        /// </summary>
        IsShallow = 1 << 0,
    }

    /// <summary>
    /// File churn parameters.
    /// </summary>
    [Flags]
    public enum FileChurnParams
    {
        /// <summary>
        /// Use defaults.
        /// </summary>
        Defaults = 0 << 0,

        /// <summary>
        /// Non default option of decrypting.
        /// </summary>
        IsDecrypt = 1 << 0,

        /// <summary>
        /// Non default option of retaining the source file.
        /// </summary>
        KeepSource = 1 << 1,

        /// <summary>
        /// Non default option of overwriting any existing target.
        /// </summary>
        RedoTarget = 1 << 2,
    }
}
