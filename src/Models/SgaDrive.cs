namespace ii.EveOfPeace.Models;

internal class SgaDrive
{
    public string Alias { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ushort FirstFolder { get; set; }
    public ushort LastFolder { get; set; }
    public ushort FirstFile { get; set; }
    public ushort LastFile { get; set; }
    public ushort RootFolder { get; set; }
}