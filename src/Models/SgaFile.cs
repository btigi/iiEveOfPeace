namespace ii.EveOfPeace.Models;

internal class SgaFile
{
    public uint NameOffset { get; set; }
    public StorageType StorageFlag { get; set; }
    public uint DataOffset { get; set; }
    public uint CompressedSize { get; set; }
    public uint DecompressedSize { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
}