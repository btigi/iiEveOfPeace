namespace ii.EveOfPeace;

public class ChunkyProcessor
{
    public List<Chunk> Read(string filename)
    {
        using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs);

        var signature = br.ReadBytes(12);
        var signatureTerminator = br.ReadBytes(4);
        var version = br.ReadInt32();
        var platform = br.ReadInt32();

        var chunks = new List<Chunk>();
        while (br.BaseStream.Position < br.BaseStream.Length)
        {
            chunks.Add(ReadChunk(br));
        }

        return chunks;
    }

    private Chunk ReadChunk(BinaryReader br)
    {
        var chunk = new Chunk();
        chunk.ChunkType = System.Text.Encoding.ASCII.GetString(br.ReadBytes(4));
        chunk.ChunkType = System.Text.Encoding.ASCII.GetString(br.ReadBytes(4));
        chunk.Version = br.ReadInt32();
        var chunkSize = br.ReadInt32();
        var chunkNameSize = br.ReadInt32();
        chunk.Name = System.Text.Encoding.ASCII.GetString(br.ReadBytes(chunkNameSize));
        chunk.Data = br.ReadBytes(chunkSize);

        return chunk;
    }
}

public class Chunk
{
    public string ChunkType { get; set; }
    public string ChunkId { get; set; }
    public int Version { get; set; }
    public string Name { get; set; }
    public byte[] Data { get; set; }
}