using Newtonsoft.Json.Linq;
using System.Text;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;
using UAssetAPI;
using plgtool.Plg;
using plgtool.Uim;

namespace plgtool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            if (args.Length < 3 || args.Length > 4)
            {
                Console.WriteLine("Usage: plgtool.exe [import json] [cooked package] [io store package]");
                Console.WriteLine("Import JSON: JSON file that was exported from Blender");
                Console.WriteLine("Cooked Package: .uasset file generated from ZenTools dump");
                Console.WriteLine("Patch Package: .uasset from Persona 3 Reload");
                Console.WriteLine("Output (optional): The output file name (default: \"[filename]_MODIFIED.uasset\" in patch package folder)");
                return;
            }
            var inputFile = File.Open(args[0], FileMode.Open);
            using (var inputReader = new StreamReader(inputFile, Encoding.UTF8))
            {
                // Parse import PLG JSON
                var inputObj = JArray.Parse(inputReader.ReadToEnd())[0]; // FModel exports JSONs as arrays, so correct that
                switch (inputObj["Type"].ToString())
                {
                    case "PlgAsset":
                        List<PlgData> plgEntries = new();
                        foreach (var plgEntry in inputObj["Properties"]["PlgData"]["PlgDatas"].Values<JObject>())
                            plgEntries.Add(PlgData.FromJsonObject(plgEntry));
                        var outPlg = new PlgAsset(args[1], args[2], args.Length == 4 ? args[3] : null, plgEntries);
                        outPlg.Inject();
                        outPlg.Serialize();
                        break;
                    case "UimAsset":
                        var uimData = FUimData.FromJsonObject(inputObj["Properties"]["UimData"].Value<JObject>());
                        var outUim = new UimAsset(args[1], args[2], args.Length == 4 ? args[3] : null, uimData);
                        outUim.Inject();
                        outUim.Serialize();
                        break;
                    default:
                        Console.WriteLine("Error: Import JSON must be a valid PLG or UIM");
                        break;
                }
            }
        }
    }
}
