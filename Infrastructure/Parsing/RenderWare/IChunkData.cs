using RWTree.Domain.Models;

namespace RWTree.Infrastructure.Parsing.RenderWare;

/// <summary>
/// Interface for extracting data from chunks without creating UI components
/// </summary>
public interface IChunkData
{
    /// <summary>
    /// Gets the chunk data as a ChunkNode without creating UI components
    /// </summary>
    /// <returns>ChunkNode containing the chunk data</returns>
    ChunkNode ToChunkNode();
}