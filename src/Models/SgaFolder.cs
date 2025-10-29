namespace ii.EveOfPeace.Models;

internal class SgaFolder
{
    public uint NameOffset { get; set; }
    public ushort FirstFolder { get; set; }
    public ushort LastFolder { get; set; }
    public ushort FirstFile { get; set; }
    public ushort LastFile { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? FullPath { get; set; }
}