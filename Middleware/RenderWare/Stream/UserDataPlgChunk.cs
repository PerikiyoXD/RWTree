using System.IO;
using System.Text;
using RWTree.Middleware.RenderWare.Stream.Chunks;

namespace RWTree.Middleware.RenderWare.Stream;

public class UserDataPlgChunk(ExtensionChunk chunk, ChunkHeader header) : Chunk(chunk, header)
{
    public uint EntryCount { get; set; }
    public Dictionary<uint, DictionaryEntry> Entries { get; set; } = new();

    public override void Read(BinaryReader binaryReader)
    {
        base.Read(binaryReader);

        EntryCount = binaryReader.ReadUInt32();

        for (uint i = 0; i < EntryCount; i++)
        {
            var entry = new DictionaryEntry();
            entry.Read(binaryReader);
            Entries.Add(i, entry);
        }
        
        binaryReader.BaseStream.Seek(StartPosition + Header.Size + 12, SeekOrigin.Begin);
    }

    public override void Write(BinaryWriter binaryWriter)
    {
        base.Write(binaryWriter);
        
        binaryWriter.Write(EntryCount);
        
        foreach (var entry in Entries)
        {
            entry.Value.Write(binaryWriter);
        }
    }

    public class DictionaryEntry : IBinaryReadWrite
    {
        public SizedString Name { get; set; }
        public UserDataType DataType { get; set; }
        public uint ObjectCount { get; set; }
        public Dictionary<uint, object> Data { get; set; } = new();
        
        public void Read(BinaryReader binaryReader)
        {
            Name = new SizedString();
            Name.Read(binaryReader);
            
            DataType = (UserDataType)binaryReader.ReadUInt32();
            ObjectCount = binaryReader.ReadUInt32();
            
            
            for (var i = 0; i < ObjectCount; i++)
            {
                
                object data;
                switch (DataType)
                {
                    case UserDataType.Integer:
                        data = binaryReader.ReadInt32();
                        break;
                    case UserDataType.Float:
                        data = binaryReader.ReadSingle();
                        break;
                    case UserDataType.String:
                        var sizedString = new SizedString();
                        sizedString.Read(binaryReader);
                        data = sizedString;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                Data.Add((uint)i, data);
            }
        }
        
        public void Write(BinaryWriter binaryWriter)
        {
            Name.Write(binaryWriter);
            binaryWriter.Write((uint)DataType);
            binaryWriter.Write(ObjectCount);
            
            foreach (var data in Data)
            {
                switch (DataType)
                {
                    case UserDataType.Integer:
                        binaryWriter.Write((int)data.Value);
                        break;
                    case UserDataType.Float:
                        binaryWriter.Write((float)data.Value);
                        break;
                    case UserDataType.String:
                        ((SizedString)data.Value).Write(binaryWriter);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    
    public enum UserDataType
    {
        Integer = 0x00000001,
        Float = 0x00000002,
        String = 0x00000003,
    }

    public class SizedString : IBinaryReadWrite
    {
        public uint Length { get; set; }
        public string Data { get; set; }

        public void Read(BinaryReader binaryReader)
        {
            Length = binaryReader.ReadUInt32();
            Data = Encoding.UTF8.GetString(binaryReader.ReadBytes((int)Length));
        }

        public void Write(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Length);
            binaryWriter.Write(Encoding.UTF8.GetBytes(Data));
        }
    }
}