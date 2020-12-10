// <copyright file="CharCodec.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace FullStack.Extensions.Text.Codec
{
    /// <summary>
    /// Character encoding.
    /// </summary>
    public enum CharCodec
    {
        /// <summary>
        /// See <see cref="System.Text.Encoding.ASCII"/>.
        /// </summary>
        Ascii,

        /// <summary>
        /// See <see cref="System.Text.Encoding.Unicode"/>.
        /// </summary>
        Unicode,

        /// <summary>
        /// See <see cref="System.Text.Encoding.UTF8"/>.
        /// </summary>
        Utf8,
    }
}
