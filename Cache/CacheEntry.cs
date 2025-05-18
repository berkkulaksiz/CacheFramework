// <copyright file="CacheEntry.cs" project="Cache">
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
///     Represents a cache entry with metadata.
/// </summary>
public class CacheEntry
{
    /// <summary>
    ///     Gets or sets the content of the cache entry.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    ///     Gets or sets the ETag of the cache entry.
    /// </summary>
    public string ETag { get; set; }

    /// <summary>
    ///     Gets or sets the timestamp of the cache entry.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the content is compressed.
    /// </summary>
    public bool IsCompressed { get; set; }

    /// <summary>
    ///     Gets or sets the tags associated with the cache entry.
    /// </summary>
    public Dictionary<string, string> Tags { get; set; } = new();

    /// <summary>
    ///     Gets the actual content, decompressing if necessary.
    /// </summary>
    /// <returns>The uncompressed content.</returns>
    public string GetContent()
    {
        if (!IsCompressed) return Content;

        try
        {
            var compressedBytes = Convert.FromBase64String(Content);

            using var inputStream = new MemoryStream(compressedBytes);
            using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();

            gzipStream.CopyTo(outputStream);

            return Encoding.UTF8.GetString(outputStream.ToArray());
        }
        catch (Exception)
        {
            // Fallback to returning the raw content if decompression fails
            return Content;
        }
    }
}