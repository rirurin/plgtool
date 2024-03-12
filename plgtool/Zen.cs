using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plgtool.Zen
{
    public class FPackageSummaryHeader
    {
        public static int SerializedLength = 0x40;

        public ulong Name;
        public ulong SourceName;
        public uint PackageFlags;
        public uint CookedHeaderSize;
        public uint NameMapNamesOffset;
        public uint NameMapNamesSize;
        public uint NameMapHashesOffset;
        public uint NameMapHashesSize;
        public uint ImportMapOffset;
        public uint ExportMapOffset;
        public uint ExportBundlesOffset;
        public uint GraphDataOffset;
        public uint GraphDataSize;
        public uint Padding;
        public FPackageSummaryHeader(BinaryReader reader)
        {
            Name = reader.ReadUInt64(); // Name
            SourceName = reader.ReadUInt64(); // SourceName
            PackageFlags = reader.ReadUInt32(); // PackageFlags
            CookedHeaderSize = reader.ReadUInt32(); // CookedHeaderSize
            NameMapNamesOffset = reader.ReadUInt32(); // NameMapNamesOffset
            NameMapNamesSize = reader.ReadUInt32(); // NameMapNamesSize
            NameMapHashesOffset = reader.ReadUInt32(); // NameMapHashesOffset
            NameMapHashesSize = reader.ReadUInt32(); // NameMapHashesSIze
            ImportMapOffset = reader.ReadUInt32(); // ImportMapOffset
            ExportMapOffset = reader.ReadUInt32(); // ExportMapOffset
            ExportBundlesOffset = reader.ReadUInt32(); // ExportBudlesOffset
            GraphDataOffset = reader.ReadUInt32(); // GraphDataOffset
            GraphDataSize = reader.ReadUInt32(); // GraphDataSize
            Padding = reader.ReadUInt32(); // Padding
        }
    }

    public class FExportMapEntry
    {
        public static int SerializedLength = 0x48;

        public ulong CookedSerialOffset;
        public ulong CookedSerialSize;
        public ulong ObjectName;
        public ulong OuterIndex;
        public ulong ClassIndex;
        public ulong SuperIndex;
        public ulong TemplateIndex;
        public ulong GlobalImportIndex;
        public int ObjectFlags;
        public byte FilterFlags;
        public byte[] unk;
        public FExportMapEntry(BinaryReader reader)
        {
            CookedSerialOffset = reader.ReadUInt64(); // CookedSerialOffset
            CookedSerialSize = reader.ReadUInt64(); // CookedSerialSize
            ObjectName = reader.ReadUInt64();
            OuterIndex = reader.ReadUInt64();
            ClassIndex = reader.ReadUInt64();
            SuperIndex = reader.ReadUInt64();
            TemplateIndex = reader.ReadUInt64();
            GlobalImportIndex = reader.ReadUInt64();
            ObjectFlags = reader.ReadInt32();
            FilterFlags = reader.ReadByte();
            unk = reader.ReadBytes(3);
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(CookedSerialOffset);
            writer.Write(CookedSerialSize);
            writer.Write(ObjectName);
            writer.Write(OuterIndex);
            writer.Write(ClassIndex);
            writer.Write(SuperIndex);
            writer.Write(TemplateIndex);
            writer.Write(GlobalImportIndex);
            writer.Write(ObjectFlags);
            writer.Write(FilterFlags);
            writer.Write(unk);
        }
    }
}
