using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;
using Newtonsoft.Json.Linq;

namespace plgtool.Imports
{
    public class PlgVertex
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public PlgVertex(float X, float Y, float Z) { this.X = X; this.Y = Y; this.Z = Z; }
        public static PlgVertex FromJsonObject(JObject obj) => new PlgVertex(obj["X"].Value<float>(), obj["Y"].Value<float>(), obj["Z"].Value<float>());

        public VectorPropertyData Serialize(Asset asset)
        {
            var propOut = new VectorPropertyData(asset.GetOrAddName("Vertices"));
            propOut.Value = new(X, Y, Z);
            return propOut;
        }
    }

    public class PlgData
    {
        public List<PlgVertex> Vertices { get; set; }
        public List<ushort> Indices { get; set; }
        public List<uint> Colors { get; set; }
        public string Name { get; set; }
        public float MinX { get; set; }
        public float MinY { get; set; }
        public float MaxX { get; set; }
        public float MaxY { get; set; }

        public PlgData(List<PlgVertex> vertices, List<ushort> indices, List<uint> colors,
            string name, float minX, float minY, float maxX, float maxY)
        {
            Vertices = vertices;
            Indices = indices;
            Colors = colors;
            Name = name;
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        public static PlgData FromJsonObject(JObject obj)
        {
            List<PlgVertex> vertices = new();
            List<ushort> indices = new();
            List<uint> colors = new();
            foreach (var vertex in obj["Vertices"].Values<JObject>()) vertices.Add(PlgVertex.FromJsonObject(vertex));
            foreach (var index in obj["Indices"].Values<ushort>()) indices.Add(index);
            foreach (var color in obj["Colors"].Values<uint>()) colors.Add(color);
            var plgName = obj["Name"].Value<string>();
            Console.WriteLine($"Imported from JSON \"{plgName}\" ({vertices.Count} vertices, {indices.Count} indices)");
            return new PlgData(vertices, indices, colors, plgName, obj["MinX"].Value<float>(), 
                obj["MinY"].Value<float>(), obj["MaxX"].Value<float>(), obj["MaxY"].Value<float>());
        }
        public ArrayPropertyData MakeVertices(Asset asset)
        {
            var propOut = new ArrayPropertyData(asset.GetOrAddName("Vertices"));
            propOut.ArrayType = asset.GetOrAddName("StructProperty");
            propOut.Value = GC.AllocateUninitializedArray<PropertyData>(Vertices.Count);
            for (int i = 0; i < Vertices.Count; i++)
            {
                var currValue = new VectorPropertyData(asset.GetOrAddName("Vertices"));
                currValue.Value = new FVector(Vertices[i].X, Vertices[i].Y, Vertices[i].Z);
                propOut.Value[i] = currValue;
            }
            return propOut;
        }
        public ArrayPropertyData MakeIndices(Asset asset)
        {
            var propOut = new ArrayPropertyData(asset.GetOrAddName("Indices"));
            propOut.ArrayType = asset.GetOrAddName("UInt16Property");
            propOut.Value = GC.AllocateUninitializedArray<PropertyData>(Indices.Count);
            for (int i = 0; i < Indices.Count; i++)
            {
                var currValue = new UInt16PropertyData();
                currValue.Value = Indices[i];
                propOut.Value[i] = currValue;
            }
            return propOut;
        }
        public ArrayPropertyData MakeColors(Asset asset)
        {
            var propOut = new ArrayPropertyData(asset.GetOrAddName("Colors"));
            propOut.ArrayType = asset.GetOrAddName("UInt32Property");
            propOut.Value = GC.AllocateUninitializedArray<PropertyData>(Vertices.Count);
            for (int i = 0; i < Vertices.Count; i++)
            {
                var currValue = new UInt32PropertyData();
                currValue.Value = Colors[i];
                propOut.Value[i] = currValue;
            }
            return propOut;
        }

        public UInt32PropertyData MakeUintProperty(Asset asset, string name, uint value)
        {
            var propOut = new UInt32PropertyData(asset.GetOrAddName(name));
            propOut.Value = value;
            return propOut;
        }
        public FloatPropertyData MakeFloatProperty(Asset asset, string name, float value)
        {
            var propOut = new FloatPropertyData(asset.GetOrAddName(name));
            propOut.Value = value;
            return propOut;
        }
        public NamePropertyData MakeNameProperty(Asset asset, string propName, string valueName)
        {
            var propOut = new NamePropertyData(asset.GetOrAddName(propName));
            propOut.Value = asset.GetOrAddName(valueName);
            return propOut;
        }
        public StructPropertyData Serialize(Asset asset)
        {
            var newPlg = new StructPropertyData(asset.GetOrAddName("PlgDatas"), asset.GetOrAddName("PlgPrimitiveData"));
            newPlg.Value = new() {
                MakeVertices(asset),
                MakeIndices(asset),
                MakeColors(asset),
                MakeNameProperty(asset, "Name", Name),
                MakeFloatProperty(asset, "MinX", MinX),
                MakeFloatProperty(asset, "MinY", MinY),
                MakeFloatProperty(asset, "MaxX", MaxX),
                MakeFloatProperty(asset, "MaxY", MaxY),
            };
            return newPlg;
        }
    }
}
