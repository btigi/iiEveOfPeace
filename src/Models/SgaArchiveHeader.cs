namespace ii.EveOfPeace.Models;

internal class SgaArchiveHeader
{
    public string Signature { get; set; } = string.Empty;
    public uint Version { get; set; }
    public byte[] FileHash { get; set; } = [];
    public string ArchiveName { get; set; } = string.Empty;
    public byte[] TocHash { get; set; } = [];
    public uint TocSize { get; set; }
    public uint DataOffset { get; set; }
}
