using System.Text;

namespace ii.EveOfPeace
{
    public class SgaProcessor
    {
        public void Read(string filename)
        {
            var result = new List<(string filename, byte[] bytes)>();
            using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader(fs);

            var signatureBytes = br.ReadChars(8);
            var signature = new string(signatureBytes);
            if (signature != "_ARCHIVE")
            {
                throw new InvalidDataException("Not a valid SGA file.");
            }

            var version = br.ReadInt32();

            var unknown1 = br.ReadInt32();
            var unknown2 = br.ReadInt32();
            var unknown3 = br.ReadInt32();
            var unknown4 = br.ReadInt32();

            var nameBytes = br.ReadBytes(128);
            var name = Encoding.Unicode.GetString(nameBytes).TrimEnd('\0');

            var unknown5 = br.ReadInt32();
            var unknown6 = br.ReadInt32();
            var unknown7 = br.ReadInt32();
            var unknown8 = br.ReadInt32();

            var dataHeaderSize = br.ReadInt32();
            var dataOffset = br.ReadInt32();
            var tocOffset = br.ReadInt32();
            var tocCount = br.ReadInt16();
            var directoryOffset = br.ReadInt32();
            var directoryCount = br.ReadInt16();
            var fileOffset = br.ReadInt32();
            var fileCount = br.ReadInt16();
            var itemOffset = br.ReadInt32();
            var itemCount = br.ReadInt16();

            var tocAliasBytes = br.ReadBytes(64);
            var tocAlias = Encoding.Unicode.GetString(nameBytes).TrimEnd('\0');

            var tocStartNameBytes = br.ReadBytes(64);
            var tocStartName = Encoding.Unicode.GetString(tocStartNameBytes).TrimEnd('\0');

            var tocStartDir = br.ReadInt16();
            var tocEndDir = br.ReadInt16();
            var tocStartFile = br.ReadInt16();
            var tocEndFile = br.ReadInt16();

            var tocFolderOffset = br.ReadInt32();

            var directoryInfos = new List<DirectoryInfo>();
            for (var i = 0; i < fileCount; i++)
            {
                fs.Seek(0, SeekOrigin.Begin);
                var directory = new DirectoryInfo();

                directoryInfos.Add(directory);
            }

            var fileInfos = new List<FileInfo>();
            for (var i = 0; i < fileCount; i++)
            {
                fs.Seek(0, SeekOrigin.Begin);
                var fileInfo = new FileInfo();

                fileInfos.Add(fileInfo);
            }
        }

        public class DirectoryInfo
        {
            public int NameOffset { get; set; }
            public Int16 SubdirectoryIdStart { get; set; }
            public Int16 SubdirectoryIdEnd { get; set; }
            public Int16 FileIdStart { get; set; }
            public Int16 FileIdEnd { get; set; }
        }

        public class FileInfo
        {
            public int NameOffset { get; set; }
            public int Unknown1 { get; set; }
            public int DataOffset { get; set; }
            public int Unknown2 { get; set; }
            public int DataLength { get; set; }
        }
    }
}