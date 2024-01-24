using System.IO;
using System.Windows.Controls;
using RWTree.Middleware.RenderWare.Stream;
using RWTree.Middleware.RenderWare.Stream.Chunks;

namespace RWTree.Middleware.RenderWare;

public class Dff : IBinaryReadWrite, ITreeItem
{
    public ClumpChunk Clump;
    // public List<MeshInstance3D> Nodes;


    public void Read(BinaryReader binaryReader)
    {
        Console.WriteLine($"DFFReader.Read: Reading DFF file name");

        var firstChunk = Chunk.ReadChunk(binaryReader);

        if (firstChunk.Header.Type != ChunkType.Clump || !(firstChunk is ClumpChunk clumpChunk))
            throw new Exception("Invalid DFF file");
        
        Clump = clumpChunk;
    }

    public void Write(BinaryWriter binaryWriter)
    {
        throw new NotImplementedException();
    }

    // public List<Node3D> GenerateNodeHierarchy()
    // {
    //     // Create a new list to hold the generated MeshInstance3D nodes
    //     List<Node3D> nodes = new();
    //
    //     // Iterate through each frame in the clump
    //     for (int nodeIndex = 0; nodeIndex < clump.FrameList.FrameListStruct.FrameCount; nodeIndex++)
    //     {
    //         // Get the frame
    //         var frame = clump.FrameList.FrameListStruct.Frames[nodeIndex];
    //         var nodeName = clump.FrameList.GetNodeNameChunk(nodeIndex).NodeName;
    //         var nodeParent = frame.ParentIndex;
    //
    //         Node3D node = new()
    //         {
    //             Transform = frame.Transform,
    //             Name = nodeName
    //         };
    //
    //         if (nodeParent != -1)
    //         {
    //             nodes[nodeParent].AddChild(node);
    //         }
    //
    //         // Add the generated MeshInstance3D node to the list
    //         nodes.Add(node);
    //     }
    //
    //     // For each node which is an atomic (renderable),
    //     // we replace the node with a mesh.
    //
    //     // Iterate through each atomic in the clump
    //     for (int atomicIndex = 0; atomicIndex < clump.Atomics.Count; atomicIndex++)
    //     {
    //         // Get the atomic
    //         var atomic = clump.Atomics[atomicIndex];
    //         var frameIndex = atomic.AtomicStruct.FrameIndex;
    //
    //         // Generate the mesh for this atomic
    //         var meshInstance = GenerateMesh(atomic);
    //
    //         Console.WriteLine($"Replacing node '{nodes[frameIndex].Name}' with mesh '{meshInstance.Name}'");
    //
    //         // Add the generated MeshInstance3D node to the list
    //         nodes[frameIndex].ReplaceBy(meshInstance);
    //
    //         Console.WriteLine($"Replaced node '{nodes[frameIndex].Name}' with mesh '{meshInstance.Name}'");
    //     }
    //
    //     // Return the list of generated MeshInstance3D nodes
    //     return nodes;
    // }
    //
    // private MeshInstance3D GenerateMesh(AtomicChunk atomic)
    // {
    //     // Get the frame and geometry data for this atomic
    //     var frameIndex = atomic.AtomicStruct.FrameIndex;
    //     var frame = clump.FrameList.FrameListStruct.Frames[frameIndex];
    //     var geometryIndex = atomic.AtomicStruct.GeometryIndex;
    //     var geometry = clump.GeometryList.Geometries[geometryIndex];
    //
    //     ArrayMesh arrayMesh = BuildArrayMesh(geometry);
    //
    //     // Create a new MeshInstance3D node and set its transform and mesh
    //     var meshInstance = CreateMeshInstance3D(frame, frameIndex, arrayMesh);
    //
    //     // Return the generated MeshInstance3D node
    //     return meshInstance;
    // }
    //
    // private ArrayMesh BuildArrayMesh(GeometryChunk geometry)
    // {
    //     // Get the vertices and triangle data for this geometry
    //     var vertices = geometry.GeometryStruct.MorphTargets[0].Vertices;
    //     var triangles = geometry.GeometryStruct.Triangles;
    //
    //     ArrayMesh arrayMesh = new();
    //
    //     // Dictionary to hold the triangle vertex indices for each material
    //     Dictionary<int, List<List<int>>> surfaceTrianglesVertexIndices = new();
    //
    //     Dictionary<int, List<Vector2>> surfaceTrianglesUVs = new();
    //     Dictionary<int, List<Vector3>> surfaceTrianglesNormals = new();
    //     Dictionary<int, List<Vector3>> materialTriangleVertices = new();
    //
    //     for (int triangleIndex = 0; triangleIndex < triangles.Count; triangleIndex++)
    //     {
    //         var triangle = triangles[triangleIndex];
    //
    //         // If the material index doesn't exist in the dictionary, add it
    //         if (!surfaceTrianglesVertexIndices.ContainsKey(triangle.MaterialIndex))
    //         {
    //             surfaceTrianglesVertexIndices.Add(triangle.MaterialIndex, new List<List<int>>());
    //             surfaceTrianglesUVs.Add(triangle.MaterialIndex, new List<Vector2>());
    //             surfaceTrianglesNormals.Add(triangle.MaterialIndex, new List<Vector3>());
    //             materialTriangleVertices.Add(triangle.MaterialIndex, new List<Vector3>());
    //         }
    //
    //         // Define the vertex indices for this triangle
    //         var vertexIndex1 = triangle.VertexIndex1;
    //         var vertexIndex2 = triangle.VertexIndex2;
    //         var vertexIndex3 = triangle.VertexIndex3;
    //
    //         // Add the vertex indices to the dictionary
    //         surfaceTrianglesVertexIndices[triangle.MaterialIndex].Add(new List<int> { vertexIndex1, vertexIndex2, vertexIndex3 });
    //
    //         if (geometry.GeometryStruct.HasTextureCoordinates)
    //         {
    //             // Add the UVs to the dictionary
    //             surfaceTrianglesUVs[triangle.MaterialIndex].Add(geometry.GeometryStruct.TextureCoordinates[vertexIndex1]);
    //             surfaceTrianglesUVs[triangle.MaterialIndex].Add(geometry.GeometryStruct.TextureCoordinates[vertexIndex2]);
    //             surfaceTrianglesUVs[triangle.MaterialIndex].Add(geometry.GeometryStruct.TextureCoordinates[vertexIndex3]);
    //         }
    //
    //         if (geometry.GeometryStruct.HasNormals)
    //         {
    //             // Add the inverted normals to the dictionary
    //             surfaceTrianglesNormals[triangle.MaterialIndex].Add(geometry.GeometryStruct.MorphTargets[0].Normals[vertexIndex1] * -1);
    //             surfaceTrianglesNormals[triangle.MaterialIndex].Add(geometry.GeometryStruct.MorphTargets[0].Normals[vertexIndex2] * -1);
    //             surfaceTrianglesNormals[triangle.MaterialIndex].Add(geometry.GeometryStruct.MorphTargets[0].Normals[vertexIndex3] * -1);
    //         }
    //
    //         // Add the triangle vertices to the dictionary
    //         materialTriangleVertices[triangle.MaterialIndex].Add(vertices[vertexIndex1]);
    //         materialTriangleVertices[triangle.MaterialIndex].Add(vertices[vertexIndex2]);
    //         materialTriangleVertices[triangle.MaterialIndex].Add(vertices[vertexIndex3]);
    //     }
    //
    //     // Build each surface for each material
    //     for (int materialIndex = 0; materialIndex < surfaceTrianglesVertexIndices.Count; materialIndex++)
    //     {
    //         // Get the triangle UVs for this material
    //         var trianglesUVs = surfaceTrianglesUVs[materialIndex];
    //
    //         // Get the triangle normals for this material
    //         var trianglesNormals = surfaceTrianglesNormals[materialIndex];
    //
    //         // Get the triangle vertices for this material
    //         var triangleVertices = materialTriangleVertices[materialIndex];
    //
    //         // Create a new array to hold the mesh data
    //         var arrayMeshDataArray = new Godot.Collections.Array();
    //         arrayMeshDataArray.Resize((int)Mesh.ArrayType.Max);
    //
    //         // Add the triangle vertices to the array mesh data array
    //         arrayMeshDataArray[(int)Mesh.ArrayType.Vertex] = triangleVertices.ToArray();
    //
    //         if (geometry.GeometryStruct.HasTextureCoordinates)
    //         {
    //             // Add the triangle UVs to the array mesh data array
    //             arrayMeshDataArray[(int)Mesh.ArrayType.TexUV] = trianglesUVs.ToArray();
    //         }
    //
    //         if (geometry.GeometryStruct.HasNormals)
    //         {
    //             // Add the triangle normals to the array mesh data array
    //             arrayMeshDataArray[(int)Mesh.ArrayType.Normal] = trianglesNormals.ToArray();
    //         }
    //
    //         // Add the array mesh data array to the array mesh
    //         arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrayMeshDataArray);
    //     }
    //
    //     // Create each material for each surface
    //     for (int materialIndex = 0; materialIndex < surfaceTrianglesVertexIndices.Count; materialIndex++)
    //     {
    //         // Get the material for this surface
    //         var material = geometry.MaterialList.Materials[materialIndex];
    //
    //         // Create a new material for this surface
    //         StandardMaterial3D surfaceMaterial = new()
    //         {
    //             AlbedoColor = material.MaterialStruct.Color,
    //
    //             // Set the material's metallic
    //             Metallic = material.MaterialStruct.Specular,
    //
    //             // Allow doubleside rendering
    //             CullMode = BaseMaterial3D.CullModeEnum.Disabled,
    //         };
    //
    //         // Add the material to the array mesh
    //         arrayMesh.SurfaceSetMaterial(materialIndex, surfaceMaterial);
    //     }
    //
    //     // Return the dictionary of triangle vertex indices
    //     return arrayMesh;
    // }
    //
    // private MeshInstance3D CreateMeshInstance3D(FrameListStructChunk.FrameData frame, int frameIndex, ArrayMesh arrayMesh)
    // {
    //     var nodeName = clump.FrameList.GetNodeNameChunk(frameIndex).NodeName;
    //
    //     // Create a new MeshInstance3D node and set its transform and mesh
    //     var meshInstance = new MeshInstance3D
    //     {
    //         Transform = frame.Transform,
    //         Name = nodeName,
    //         Mesh = arrayMesh
    //     };
    //
    //     // Disable vlo and dam elements
    //     if (nodeName.Contains("_vlo") || nodeName.Contains("_dam"))
    //     {
    //         meshInstance.Visible = false;
    //     }
    //
    //     return meshInstance;
    // }
    
    public TreeViewItem ToTreeViewItem()
    {
        TreeViewItem tree = new() { Header = "DFF" };

        TreeViewItem clumpTree = Clump.ToTreeViewItem();
        tree.Items.Add(clumpTree);
        
        return tree;
    }
}