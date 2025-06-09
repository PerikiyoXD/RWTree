using System.IO;
using System.Windows.Controls;
using RWTree.Application.Ports;
using RWTree.Domain.Exceptions;
using RWTree.Domain.Models;
using RWTree.Infrastructure.Parsing;
using RWTree.Infrastructure.Parsing.RenderWare;
using RWTree.Infrastructure.Parsing.RenderWare.Chunks;

namespace RWTree.Infrastructure.Adapters;

/// <summary>
/// DFF parser adapter with defensive programming
/// </summary>
public sealed class DffParserAdapter : IDffParserPort
{
    private readonly ILoggingPort _loggingPort;

    public DffParserAdapter(ILoggingPort loggingPort)
    {
        _loggingPort = loggingPort ?? throw new ArgumentNullException(nameof(loggingPort));
    }

    public async Task<IEnumerable<ChunkNode>> ParseAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));

        if (!stream.CanRead)
            throw new DffParsingException("Stream is not readable");

        try
        {
            _loggingPort.LogDebug("Starting DFF parsing, stream length: {Length}", stream.Length);

            return await Task.Run(() =>
            {
                using var binaryReader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

                var dff = new Dff();
                dff.Read(binaryReader);

                if (dff.Clump == null)
                {
                    throw new DffParsingException("No clump found in DFF file");
                }

                var chunks = new List<ChunkNode>();

                // Convert the legacy structure to new domain model
                var rootNode = ConvertChunkToNode(dff.Clump);
                chunks.Add(rootNode);

                _loggingPort.LogDebug("DFF parsing completed, found {ChunkCount} root chunks", chunks.Count);

                return chunks.AsEnumerable();
            }, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _loggingPort.LogInformation("DFF parsing was cancelled");
            throw;
        }
        catch (Exception ex) when (ex is not DffParsingException)
        {
            _loggingPort.LogError("Unexpected error during DFF parsing", ex);
            throw new DffParsingException("Failed to parse DFF file", null, stream.Position, ex);
        }
    }

    public bool CanParse(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));

        if (!stream.CanRead || !stream.CanSeek)
            return false;

        try
        {
            var originalPosition = stream.Position;

            // Try to read chunk header
            using var binaryReader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

            if (stream.Length < 12) // Minimum header size
            {
                stream.Position = originalPosition;
                return false;
            }

            var header = ChunkHeader.ReadHeader(binaryReader);
            stream.Position = originalPosition;

            // Check if it's a valid clump chunk (DFF files start with clump)
            return header.Type == ChunkType.Clump;
        }
        catch
        {
            try
            {
                stream.Position = 0; // Reset to beginning if possible
            }
            catch
            {
                // Stream might not support seeking
            }

            return false;
        }
    }

    public void ValidateFormat(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));

        if (!stream.CanRead)
            throw new DffParsingException("Stream is not readable");

        if (!stream.CanSeek)
            throw new DffParsingException("Stream must support seeking for validation");

        var originalPosition = stream.Position;

        try
        {
            stream.Position = 0;

            if (stream.Length < 12)
                throw new DffParsingException("File too small to be a valid DFF file");

            using var binaryReader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

            var header = ChunkHeader.ReadHeader(binaryReader);

            if (header.Type != ChunkType.Clump)
                throw new DffParsingException($"Expected Clump chunk at start of file, found {header.Type}");

            if (header.Size <= 0)
                throw new DffParsingException("Invalid chunk size");

            _loggingPort.LogDebug("DFF format validation passed");
        }
        catch (Exception ex) when (ex is not DffParsingException)
        {
            throw new DffParsingException("Invalid DFF file format", null, stream.Position, ex);
        }
        finally
        {
            try
            {
                stream.Position = originalPosition;
            }
            catch
            {
                // Best effort to restore position
            }
        }
    }

    private ChunkNode ConvertChunkToNode(Chunk chunk)
    {
        ArgumentNullException.ThrowIfNull(chunk, nameof(chunk));

        // Use the thread-safe ToChunkNode method instead of ToTreeViewItem
        return chunk.ToChunkNode();
    }

    private static string GetChunkDisplayName(Chunk chunk)
    {
        // Try to get a meaningful name from the chunk
        return chunk.Type switch
        {
            ChunkType.Clump => "Clump",
            ChunkType.Atomic => "Atomic",
            ChunkType.Geometry => "Geometry",
            ChunkType.Material => "Material",
            ChunkType.Texture => "Texture",
            ChunkType.FrameList => "Frame List",
            ChunkType.GeometryList => "Geometry List",
            ChunkType.MaterialList => "Material List",
            _ => chunk.GetType().Name.Replace("Chunk", "")
        };
    }

    private static void AddChunkProperties(ChunkNode node, Chunk chunk)
    {
        // Add common properties
        node.AddProperty("ChunkType", chunk.Type.ToString());
        node.AddProperty("HeaderSize", chunk.Header.Size);

        // Add chunk-specific properties based on type
        switch (chunk)
        {
            case ClumpChunk clump:
                node.AddProperty("AtomicCount", clump.Atomics?.Count ?? 0);
                node.AddProperty("GeometryCount", clump.GeometryList?.Geometries?.Count ?? 0);
                break;

            case AtomicChunk atomic:
                node.AddProperty("FrameIndex", atomic.AtomicStruct?.FrameIndex ?? -1);
                node.AddProperty("GeometryIndex", atomic.AtomicStruct?.GeometryIndex ?? -1);
                break;

            case GeometryChunk geometry:
                node.AddProperty("VertexCount", geometry.GeometryStruct?.VertexCount ?? 0);
                node.AddProperty("TriangleCount", geometry.GeometryStruct?.TriangleCount ?? 0);
                node.AddProperty("HasNormals", geometry.GeometryStruct?.HasNormals ?? false);
                node.AddProperty("HasTextureCoordinates", geometry.GeometryStruct?.HasTextureCoordinates ?? false);
                break;
        }
    }
}