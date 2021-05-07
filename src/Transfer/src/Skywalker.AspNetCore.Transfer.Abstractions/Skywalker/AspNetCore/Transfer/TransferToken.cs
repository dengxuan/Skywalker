// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


namespace Skywalker.AspNetCore.Transfer
{
    /// <summary>
    /// Name/Value representing a token.
    /// </summary>
    public class TransferToken
    {
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Value.
        /// </summary>
        public string Value { get; set; } = default!;
    }
}
