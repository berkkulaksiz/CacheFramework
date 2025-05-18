// <copyright file="BrokenCircuitException.cs" project="Cache">
// 
//    Copyright (c) MicroFrame Solutions. All rights reserved.
//    Author:    berkkulaksiz
//    CreatedAt:   18.05.2025
//    UpdatedAt: 18.05.2025
// 
//    Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// 
// </copyright>

namespace Cache;

/// <summary>
///     Exception thrown when a circuit is broken.
/// </summary>
public class BrokenCircuitException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BrokenCircuitException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public BrokenCircuitException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BrokenCircuitException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public BrokenCircuitException(string message, Exception innerException) : base(message, innerException)
    {
    }
}