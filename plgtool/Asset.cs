using plgtool.Imports;
using plgtool.Zen;
using System.Text;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace plgtool
{
    public class Asset
    {
        public UAsset asset;
        public ArrayPropertyData plgDatas;
        public List<PlgData> entries;
        public string exportFileName;
        public string patchFileName;

        public Asset(string assetName, string patchName, string? exportName, List<PlgData> entries)
        {
            asset = new UAsset(assetName, EngineVersion.VER_UE4_27);
            patchFileName = patchName;
            if (exportName != null) exportFileName = exportName;
            else exportFileName = Path.Combine(Path.GetDirectoryName(patchName), Path.GetFileNameWithoutExtension(patchName) + "_MODIFIED" + Path.GetExtension(patchName));
            var outPlgData = (asset.Exports[0] as NormalExport).Data[0] as StructPropertyData;
            plgDatas = outPlgData.Value[0] as ArrayPropertyData;
            this.entries = entries;
        }

        // Inject custom PLG data into cooked asset using UAssetAPI
        // (not using CUE4Parse because that's for datamining Fortnite :true:)
        public void Inject()
        {
            plgDatas.Value = new PropertyData[entries.Count];
            for (int i = 0; i < entries.Count; i++)
                plgDatas.Value[i] = entries[i].Serialize(this);
        }
        // Patch custom info into IO Store asset (what I did for Atlus Script Tools)
        public void Serialize()
        {
            using (var assetStream = asset.WriteData())
            {
                using (var patchFile = new BinaryReader(File.Open(patchFileName, FileMode.Open))) // read IO Store asset
                {
                    using (var exportFile = new BinaryWriter(File.Open(exportFileName, FileMode.Create)))
                    {
                        var header = new FPackageSummaryHeader(patchFile); // read patch file
                        patchFile.BaseStream.Position = 0;
                        byte[] headerData = new byte[header.ExportMapOffset]; // write asset header to first export
                        patchFile.Read(headerData, 0, (int)header.ExportMapOffset);
                        exportFile.Write(headerData);

                        var export = new FExportMapEntry(patchFile); // write export with modified length
                        var contentLength = assetStream.Length - 4 - header.CookedHeaderSize;
                        export.CookedSerialSize = (ulong)contentLength;
                        export.Write(exportFile);
                        
                        var restOfHeaderLength = header.GraphDataOffset + header.GraphDataSize - header.ExportBundlesOffset;
                        patchFile.BaseStream.Position = header.ExportBundlesOffset;
                        byte[] restOfHeaderData = new byte[restOfHeaderLength]; // write rest of asset header
                        patchFile.Read(restOfHeaderData, 0, (int)restOfHeaderLength);
                        exportFile.Write(restOfHeaderData);

                        patchFile.BaseStream.Position = header.GraphDataOffset + header.GraphDataSize;
                        assetStream.Position = header.CookedHeaderSize;
                        byte[] contentData = new byte[contentLength]; // write data
                        assetStream.Read(contentData, 0, (int)contentLength);
                        exportFile.Write(contentData);
                        Console.WriteLine($"Wrote {exportFile.BaseStream.Length} bytes");
                    }
                }
            }
        }

        public FName GetOrAddName(string name)
        {
            var ustr = new FString(name, Encoding.UTF8);
            if (asset.ContainsNameReference(ustr)) return new FName(asset, asset.AddNameReference(ustr), 0);
            else return new FName(asset, asset.AddNameReference(ustr), 0);
        }
    }
}
