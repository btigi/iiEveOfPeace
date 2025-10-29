using ii.EveOfPeace.Models;
using System.Text;
using System.IO.Compression;

namespace ii.EveOfPeace;

public class SgaProcessor
{
    public List<(string filename, string filepath, byte[] data)> Read(string filename)
    {
        using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs);

        var header = ReadArchiveHeader(br);

        if (header.Version != 2)
        {
            throw new InvalidDataException($"Only SGA V2 is supported. Found version: {header.Version}");
        }

        var tocHeader = ReadTocHeader(br);

        // Drive definitions
        var drives = new List<SgaDrive>();
        for (int i = 0; i < tocHeader.DriveCount; i++)
        {
            drives.Add(ReadDriveDefinition(br));
        }

        // Folder definitions
        var folders = new List<SgaFolder>();
        for (int i = 0; i < tocHeader.FolderCount; i++)
        {
            folders.Add(ReadFolderDefinition(br));
        }

        // File definitions
        var files = new List<SgaFile>();
        for (int i = 0; i < tocHeader.FileCount; i++)
        {
            files.Add(ReadFileDefinition(br));
        }

        // Name list
        var nameListStartPos = 180 + tocHeader.NameOffset;
        fs.Seek(nameListStartPos, SeekOrigin.Begin);
        var nameListBytes = br.ReadBytes((int)(header.DataOffset - nameListStartPos));

        // Resolve names for folders and files and build full paths
        ResolveFolderNames(folders, nameListBytes);
        ResolveFileNames(files, nameListBytes);
        BuildFilePaths(drives, folders, files);

        // Extract and decompress file data
        var result = new List<(string filename, string filepath, byte[] data)>();
        foreach (var file in files)
        {
            var data = ExtractFileData(fs, header.DataOffset, file);
            result.Add((file.Name, file.FullPath, data));
        }

        return result;
    }

    private SgaArchiveHeader ReadArchiveHeader(BinaryReader br)
    {
        var header = new SgaArchiveHeader();

        var signatureBytes = br.ReadChars(8);
        header.Signature = new string(signatureBytes);
        if (header.Signature != "_ARCHIVE")
        {
            throw new InvalidDataException("Not a valid SGA file.");
        }

        header.Version = br.ReadUInt32();
        header.FileHash = br.ReadBytes(16);
        var nameBytes = br.ReadBytes(128);
        header.ArchiveName = Encoding.Unicode.GetString(nameBytes).TrimEnd('\0');
        header.TocHash = br.ReadBytes(16);
        header.TocSize = br.ReadUInt32();
        header.DataOffset = br.ReadUInt32();

        return header;
    }

    private SgaTocHeader ReadTocHeader(BinaryReader br)
    {
        var tocHeader = new SgaTocHeader();

        tocHeader.DriveOffset = br.ReadUInt32();
        tocHeader.DriveCount = br.ReadUInt16();
        tocHeader.FolderOffset = br.ReadUInt32();
        tocHeader.FolderCount = br.ReadUInt16();
        tocHeader.FileOffset = br.ReadUInt32();
        tocHeader.FileCount = br.ReadUInt16();
        tocHeader.NameOffset = br.ReadUInt32();
        tocHeader.NameCount = br.ReadUInt16();

        return tocHeader;
    }

    private SgaDrive ReadDriveDefinition(BinaryReader br)
    {
        var drive = new SgaDrive();

        var aliasBytes = br.ReadBytes(64);
        drive.Alias = Encoding.ASCII.GetString(aliasBytes).TrimEnd('\0');
        var nameBytes = br.ReadBytes(64);
        drive.Name = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0');
        drive.FirstFolder = br.ReadUInt16();
        drive.LastFolder = br.ReadUInt16();
        drive.FirstFile = br.ReadUInt16();
        drive.LastFile = br.ReadUInt16();
        drive.RootFolder = br.ReadUInt16();

        return drive;
    }

    private SgaFolder ReadFolderDefinition(BinaryReader br)
    {
        var folder = new SgaFolder();

        folder.NameOffset = br.ReadUInt32();
        folder.FirstFolder = br.ReadUInt16();
        folder.LastFolder = br.ReadUInt16();
        folder.FirstFile = br.ReadUInt16();
        folder.LastFile = br.ReadUInt16();

        return folder;
    }

    private SgaFile ReadFileDefinition(BinaryReader br)
    {
        var file = new SgaFile();

        file.NameOffset = br.ReadUInt32();
        file.StorageFlag = (StorageType)br.ReadUInt32();
        file.DataOffset = br.ReadUInt32();
        file.CompressedSize = br.ReadUInt32();
        file.DecompressedSize = br.ReadUInt32();

        return file;
    }

    private void ResolveFolderNames(List<SgaFolder> folders, byte[] nameListBytes)
    {
        foreach (var folder in folders)
        {
            folder.Name = ReadNullTerminatedString(nameListBytes, (int)folder.NameOffset);
        }
    }

    private void ResolveFileNames(List<SgaFile> files, byte[] nameListBytes)
    {
        foreach (var file in files)
        {
            file.Name = ReadNullTerminatedString(nameListBytes, (int)file.NameOffset);
        }
    }

    private string ReadNullTerminatedString(byte[] buffer, int offset)
    {
        var endIndex = offset;
        while (endIndex < buffer.Length && buffer[endIndex] != 0)
        {
            endIndex++;
        }

        return Encoding.ASCII.GetString(buffer, offset, endIndex - offset);
    }

    private void BuildFilePaths(List<SgaDrive> drives, List<SgaFolder> folders, List<SgaFile> files)
    {
        // Build folder paths
        foreach (var drive in drives)
        {
            BuildFolderPaths(drive, folders);
        }

        // Build file paths
        foreach (var drive in drives)
        {
            for (int i = drive.FirstFile; i < drive.LastFile; i++)
            {
                if (i < files.Count)
                {
                    var file = files[i];
                    file.FullPath = Path.Combine(drive.Alias, file.Name);
                }
            }
        }

        // Add files from folders
        foreach (var folder in folders)
        {
            for (int i = folder.FirstFile; i < folder.LastFile; i++)
            {
                if (i < files.Count)
                {
                    var file = files[i];
                    file.FullPath = Path.Combine(folder.FullPath ?? string.Empty, file.Name);
                }
            }
        }
    }

    private void BuildFolderPaths(SgaDrive drive, List<SgaFolder> folders)
    {
        // Build paths recursively
        for (int i = drive.FirstFolder; i < drive.LastFolder; i++)
        {
            if (i < folders.Count)
            {
                BuildFolderPath(folders[i], folders, drive.Alias);
            }
        }
    }

    private void BuildFolderPath(SgaFolder folder, List<SgaFolder> allFolders, string basePath)
    {
        folder.FullPath = Path.Combine(basePath, folder.Name);

        // Recursively build subfolder paths
        for (int i = folder.FirstFolder; i < folder.LastFolder; i++)
        {
            if (i < allFolders.Count)
            {
                BuildFolderPath(allFolders[i], allFolders, folder.FullPath);
            }
        }
    }

    private byte[] ExtractFileData(FileStream fs, uint dataBlockOffset, SgaFile file)
    {
        // Seek to the file's data position in the data block
        fs.Seek(dataBlockOffset + file.DataOffset, SeekOrigin.Begin);

        // Read the compressed data
        var compressedData = new byte[file.CompressedSize];
        fs.Read(compressedData, 0, (int)file.CompressedSize);

        // Handle based on storage type
        return file.StorageFlag switch
        {
            StorageType.Raw => compressedData,
            StorageType.BufferCompressed => DecompressZlib(compressedData, (int)file.DecompressedSize),
            StorageType.StreamCompressed => DecompressZlib(compressedData, (int)file.DecompressedSize),
            _ => throw new NotSupportedException($"Unknown storage type: {file.StorageFlag}")
        };
    }

    private byte[] DecompressZlib(byte[] compressedData, int decompressedSize)
    {
        // Skip the first 2 bytes (zlib header)
        using var compressedStream = new MemoryStream(compressedData, 2, compressedData.Length - 2);
        using var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
        using var decompressedStream = new MemoryStream(decompressedSize);

        deflateStream.CopyTo(decompressedStream);

        return decompressedStream.ToArray();
    }
}