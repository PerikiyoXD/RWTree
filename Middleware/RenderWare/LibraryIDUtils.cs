namespace RWTree.Middleware.RenderWare;

public static class LibraryIdUtils
{
    public static uint LibraryIdPack(uint version, uint build)
    {
        if (version <= 0x31000)
            return version >> 8;

        return (((version - 0x30000) & 0x3FF00) << 14) | ((version & 0x3F) << 16) | (build & 0xFFFF);
    }

    public static uint LibraryIdUnpackVersion(uint libid)
    {
        if ((libid & 0xFFFF0000) != 0)
            return (((libid >> 14) & 0x3FF00) + 0x30000) | ((libid >> 16) & 0x3F);

        return libid << 8;
    }

    public static uint LibraryIdUnpackBuild(uint libid)
    {
        if ((libid & 0xFFFF0000) != 0)
            return libid & 0xFFFF;

        return 0;
    }

    public static void PrintVersionInfo(uint version)
    {
        var unpackedVersion = LibraryIdUnpackVersion(version);
        var unpackedBuild = LibraryIdUnpackBuild(version);

        var renderwareVersion = (unpackedVersion >> 16) & 0xF; // 0x000F0000
        var majorRevision = (unpackedVersion >> 12) & 0xF; // 0x0000F000
        var minorRevision = (unpackedVersion >> 8) & 0xF; // 0x00000F00
        var binaryRevision = unpackedVersion & 0xFF; // 0x000000FF
        var buildNumber = unpackedBuild;

        // Print debug messages
        Console.WriteLine($"Unpacked version: {unpackedVersion:X}");
        Console.WriteLine($"Unpacked build: {unpackedBuild:X}");
        Console.WriteLine($"Renderware version: {renderwareVersion}");
        Console.WriteLine($"Major revision: {majorRevision}");
        Console.WriteLine($"Minor revision: {minorRevision}");
        Console.WriteLine($"Binary revision: {binaryRevision}");
        Console.WriteLine(
            $"Chunk header version: {renderwareVersion}.{majorRevision}.{minorRevision}.{binaryRevision}b{buildNumber}");
    }


    public static string GetVersionString(uint version)
    {
        var unpackedVersion = LibraryIdUnpackVersion(version);
        var unpackedBuild = LibraryIdUnpackBuild(version);

        var renderwareVersion = (unpackedVersion >> 16) & 0xF; // 0x000F0000
        var majorRevision = (unpackedVersion >> 12) & 0xF; // 0x0000F000
        var minorRevision = (unpackedVersion >> 8) & 0xF; // 0x00000F00
        var binaryRevision = unpackedVersion & 0xFF; // 0x000000FF
        var buildNumber = unpackedBuild;

        return $"{renderwareVersion}.{majorRevision}.{minorRevision}.{binaryRevision}b{buildNumber}";
    }
}