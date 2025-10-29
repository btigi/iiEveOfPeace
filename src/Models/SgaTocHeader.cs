namespace ii.EveOfPeace.Models;

internal class SgaTocHeader
{
    public uint DriveOffset { get; set; }
    public ushort DriveCount { get; set; }
    public uint FolderOffset { get; set; }
    public ushort FolderCount { get; set; }
    public uint FileOffset { get; set; }
    public ushort FileCount { get; set; }
    public uint NameOffset { get; set; }
    public ushort NameCount { get; set; }
}