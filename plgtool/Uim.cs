using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;
using FactoryType = System.Collections.Generic.Dictionary<string, System.Func<Newtonsoft.Json.Linq.JObject, string, plgtool.Uim.FUimBase>>;
namespace plgtool.Uim
{
    
    public class FUimFactory
    {
        public FactoryType AnimFactories = new();
        public FactoryType GeomFactories = new();

        public FUimFactory() {}
        public FUimFactory RegisterAnimationFactory(string name, Func<JObject, string, FUimBase> factory)
        {
            AnimFactories.Add(name, factory);
            return this;
        }
        public FUimFactory RegisterGeometryFactory(string name, Func<JObject, string, FUimBase> factory)
        {
            GeomFactories.Add(name, factory);
            return this;
        }
        private string GetVertexTypeName(JObject obj, FactoryType factories)
        {
            foreach (var factory in factories)
            {
                if (obj.TryGetValue(factory.Key, out _)) return factory.Key;
            }
            throw new Exception("Couldn't find a matching vertex type");
        }
        public string GetAnimVertexTypeName(JObject obj) => GetVertexTypeName(obj, AnimFactories);
        public string GetGeomVertexTypeName(JObject obj) => GetVertexTypeName(obj, GeomFactories);
        private List<FUimBase> MakeVertices(IEnumerable<JObject> obj, string typeName, FactoryType factories)
        {
            if (factories.TryGetValue(typeName, out var uimFactory))
            {
                List<FUimBase> vertices = new();
                foreach (var vert in obj)
                    vertices.Add(uimFactory(vert, typeName));
                return vertices;
            }
            throw new Exception($"No factory exists for type {typeName}");
        }
        public List<FUimBase> MakeAnimVertices(IEnumerable<JObject> obj, string typeName) => MakeVertices(obj, typeName, AnimFactories);
        public List<FUimBase> MakeGeomVertices(IEnumerable<JObject> obj, string typeName) => MakeVertices(obj, typeName, GeomFactories);
    }

    public abstract class FUimBase
    {
        protected string Name { get; set; }
        public FUimBase(string name) { Name = name; }
        protected StructPropertyData SerializeInner(Asset asset, params PropertyData[] fields)
        {
            var propOut = new StructPropertyData(asset.GetOrAddName(Name), asset.GetOrAddName(GetTypeName()));
            propOut.Value = fields.ToList();
            return propOut;
        }
        public abstract StructPropertyData Serialize(Asset asset);
        public string GetTypeName() => GetType().Name.Substring(1);
    }
    public class FUim2DVertex : FUimBase
    {
        public float X { get; set; }
        public float Y { get; set; }
        public FUim2DVertex(string name, float x, float y) : base(name) { X = x; Y = y; }
        public static FUim2DVertex FromJsonObject(JObject obj, string name) => 
            new FUim2DVertex(name, obj["x"].Value<float>(), obj["y"].Value<float>());
        public override StructPropertyData Serialize(Asset asset) => SerializeInner(asset, 
            Utils.MakeFloatProperty(asset, "x", X), 
            Utils.MakeFloatProperty(asset, "y", Y));
    }
    public class FUim2DVertCol : FUimBase
    {
        public float X { get; set; }
        public float Y { get; set; }
        public uint Color { get; set; }
        public FUim2DVertCol(string name, float x, float y, uint color) : base(name) { X = x; Y = y; Color = color; }
        public static FUim2DVertCol FromJsonObject(JObject obj, string name) => 
            new FUim2DVertCol(name, obj["x"].Value<float>(), obj["y"].Value<float>(), obj["color"].Value<uint>());
        public override StructPropertyData Serialize(Asset asset) => SerializeInner(asset, 
            Utils.MakeFloatProperty(asset, "x", X), 
            Utils.MakeFloatProperty(asset, "y", Y), 
            Utils.MakeUintProperty(asset, "color", Color));
    }

    public class FUim2DVertUV : FUimBase
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float U { get; set; }
        public float V { get; set; }
        public FUim2DVertUV(string name, float x, float y, float u, float v) : base(name) { X = x; Y = y; U = u; V = v; }
        public static FUim2DVertUV FromJsonObject(JObject obj, string name) =>
            new FUim2DVertUV(name, obj["x"].Value<float>(), obj["y"].Value<float>(), 
                obj["u0"].Value<float>(), obj["v0"].Value<float>());
        public override StructPropertyData Serialize(Asset asset) => SerializeInner(asset,
            Utils.MakeFloatProperty(asset, "x", X),
            Utils.MakeFloatProperty(asset, "y", Y),
            Utils.MakeFloatProperty(asset, "u0", U),
            Utils.MakeFloatProperty(asset, "v0", V));
    }

    public class FUim2DVertColUV : FUimBase
    {
        public float X { get; set; }
        public float Y { get; set; }
        public uint Color { get; set; }
        public float U { get; set; }
        public float V { get; set; }
        public FUim2DVertColUV(string name, float x, float y, uint color, float u, float v) : base(name) { X = x; Y = y; Color = color;  U = u; V = v; }
        public static FUim2DVertColUV FromJsonObject(JObject obj, string name) =>
            new FUim2DVertColUV(name, obj["x"].Value<float>(), obj["y"].Value<float>(),
                obj["color"].Value<uint>(), obj["u0"].Value<float>(), obj["v0"].Value<float>());
        public override StructPropertyData Serialize(Asset asset) => SerializeInner(asset,
            Utils.MakeFloatProperty(asset, "x", X),
            Utils.MakeFloatProperty(asset, "y", Y),
            Utils.MakeUintProperty(asset, "color", Color),
            Utils.MakeFloatProperty(asset, "u0", U),
            Utils.MakeFloatProperty(asset, "v0", V));
    }

    public class FUim3DVertex : FUimBase
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public FUim3DVertex(string name, float x, float y, float z) : base(name) { X = x; Y = y; Z = z; }
        public static FUim3DVertex FromJsonObject(JObject obj, string name) =>
            new FUim3DVertex(name, obj["x"].Value<float>(), obj["y"].Value<float>(), obj["z"].Value<float>());
        public override StructPropertyData Serialize(Asset asset) => SerializeInner(asset,
            Utils.MakeFloatProperty(asset, "x", X),
            Utils.MakeFloatProperty(asset, "y", Y));
    }
    public class FUim3DVertCol : FUimBase
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public uint Color { get; set; }
        public FUim3DVertCol(string name, float x, float y, float z, uint color) : base(name) { X = x; Y = y; Z = z; Color = color; }
        public static FUim3DVertCol FromJsonObject(JObject obj, string name) =>
            new FUim3DVertCol(name, obj["x"].Value<float>(), obj["y"].Value<float>(),
                obj["z"].Value<float>(), obj["color"].Value<uint>());
        public override StructPropertyData Serialize(Asset asset) => SerializeInner(asset,
            Utils.MakeFloatProperty(asset, "x", X),
            Utils.MakeFloatProperty(asset, "y", Y),
            Utils.MakeFloatProperty(asset, "z", Z),
            Utils.MakeUintProperty(asset, "color", Color));
    }

    public class FUim3DVertUV : FUimBase
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float U { get; set; }
        public float V { get; set; }
        public FUim3DVertUV(string name, float x, float y, float z, float u, float v) : base(name) { X = x; Y = y; Z = z;  U = u; V = v; }
        public static FUim3DVertUV FromJsonObject(JObject obj, string name) =>
            new FUim3DVertUV(name, obj["x"].Value<float>(), obj["y"].Value<float>(),
                obj["z"].Value<float>(), obj["u0"].Value<float>(), obj["v0"].Value<float>());
        public override StructPropertyData Serialize(Asset asset) => SerializeInner(asset,
            Utils.MakeFloatProperty(asset, "x", X),
            Utils.MakeFloatProperty(asset, "y", Y),
            Utils.MakeFloatProperty(asset, "z", Z),
            Utils.MakeFloatProperty(asset, "u0", U),
            Utils.MakeFloatProperty(asset, "v0", V));
    }

    public class FUim3DVertColUV : FUimBase
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public uint Color { get; set; }
        public float U { get; set; }
        public float V { get; set; }
        public FUim3DVertColUV(string name, float x, float y, float z, uint color, float u, float v) : base(name) { X = x; Y = y; Z = z; Color = color; U = u; V = v; }
        public static FUim3DVertColUV FromJsonObject(JObject obj, string name) =>
            new FUim3DVertColUV(name, obj["x"].Value<float>(), obj["y"].Value<float>(), obj["z"].Value<float>(),
                obj["color"].Value<uint>(), obj["u0"].Value<float>(), obj["v0"].Value<float>());
        public override StructPropertyData Serialize(Asset asset) => SerializeInner(asset,
            Utils.MakeFloatProperty(asset, "x", X),
            Utils.MakeFloatProperty(asset, "y", Y),
            Utils.MakeFloatProperty(asset, "z", Z),
            Utils.MakeUintProperty(asset, "color", Color),
            Utils.MakeFloatProperty(asset, "u0", U),
            Utils.MakeFloatProperty(asset, "v0", V));
    }

    public class FUimData
    {
        public uint? FrameSkip { get; set; }
        public int? FrameNum { get; set; }
        public int? VertexNum { get; set; }
        public int? PolygonNum { get; set; }
        public int? Coordinate { get; set; }
        public int? IndexNum { get; set; }
        public int? GeomFormat { get; set; }
        public int? AnimFormat { get; set; }

        public List<FUimBase> Geometry { get; set; }
        public List<FUimBase> Animation { get; set; }
        public List<ushort> Indices { get; set; }

        protected string AnimFactoryName { get; set; }
        protected string GeomFactoryName { get; set; }

        public FUimData(
            uint? frameSkip, int? frameNum, int? vertexNum, int? polygonNum, int? coordinate, int? indexNum, int? geomFormat, int? animFormat, 
            List<FUimBase> geometry, List<FUimBase> animation, List<ushort> indices, string animFactoryName, string geomFactoryName)
        {
            FrameSkip = frameSkip;
            FrameNum = frameNum;
            VertexNum = vertexNum;
            PolygonNum = polygonNum;
            Coordinate = coordinate;
            IndexNum = indexNum;
            GeomFormat = geomFormat;
            AnimFormat = animFormat;
            Geometry = geometry;
            Animation = animation;
            Indices = indices;
            AnimFactoryName = animFactoryName;
            GeomFactoryName = geomFactoryName;
        }
        public static FUimData FromJsonObject(JObject obj)
        {
            var factory = new FUimFactory()
                .RegisterGeometryFactory ("p2DGeomVertex",      FUim2DVertex.FromJsonObject)
                .RegisterAnimationFactory("p2DAnimVertex",      FUim2DVertex.FromJsonObject)
                .RegisterGeometryFactory ("p2DGeomVertCol",     FUim2DVertCol.FromJsonObject)
                .RegisterAnimationFactory("p2DAnimVertCol",     FUim2DVertCol.FromJsonObject)
                .RegisterGeometryFactory ("p2DGeomVertUV",      FUim2DVertUV.FromJsonObject)
                .RegisterAnimationFactory("p2DAnimVertUV",      FUim2DVertUV.FromJsonObject)
                .RegisterGeometryFactory ("p2DGeomVertColUV",   FUim2DVertColUV.FromJsonObject)
                .RegisterAnimationFactory("p2DAnimVertColUV",   FUim2DVertColUV.FromJsonObject)
                .RegisterGeometryFactory ("p3DGeomVertex",      FUim2DVertex.FromJsonObject)
                .RegisterAnimationFactory("p3DAnimVertex",      FUim2DVertex.FromJsonObject)
                .RegisterGeometryFactory ("p3DGeomVertCol",     FUim2DVertCol.FromJsonObject)
                .RegisterAnimationFactory("p3DAnimVertCol",     FUim2DVertCol.FromJsonObject)
                .RegisterGeometryFactory ("p3DGeomVertUV",      FUim2DVertUV.FromJsonObject)
                .RegisterAnimationFactory("p3DAnimVertUV",      FUim2DVertUV.FromJsonObject)
                .RegisterGeometryFactory ("p3DGeomVertColUV",   FUim2DVertColUV.FromJsonObject)
                .RegisterAnimationFactory("p3DAnimVertColUV",   FUim2DVertColUV.FromJsonObject)
            ;
            var geomFactoryName = factory.GetGeomVertexTypeName(obj);
            var animFactoryName = factory.GetAnimVertexTypeName(obj);
            List<FUimBase> geometry = factory.MakeGeomVertices(obj[geomFactoryName].Values<JObject>(), geomFactoryName);
            List<FUimBase> anims = factory.MakeAnimVertices(obj[animFactoryName].Values<JObject>(), animFactoryName);
            List<ushort> indices = [.. obj["Indices"].Values<ushort>()];
            Console.WriteLine($"{geometry.Count} {geomFactoryName}, {anims.Count} {animFactoryName}, {indices.Count} indices");
            return new FUimData(
                Utils.GetValueMaybeNull<uint>(obj, "frameSkip"), Utils.GetValueMaybeNull<int>(obj, "frameNum"), Utils.GetValueMaybeNull<int>(obj, "vertexNum"),
                Utils.GetValueMaybeNull<int>(obj, "polygonNum"), Utils.GetValueMaybeNull<int>(obj, "indexNum"), Utils.GetValueMaybeNull<int>(obj, "coordinate"),
                Utils.GetValueMaybeNull<int>(obj, "geomFormat"), Utils.GetValueMaybeNull<int>(obj, "animFormat"), geometry, anims, indices, geomFactoryName, animFactoryName);
        }


        public ArrayPropertyData MakeGeometry(Asset asset) => MakeVertices(asset, GeomFactoryName, Geometry);
        public ArrayPropertyData MakeAnimation(Asset asset) => MakeVertices(asset, AnimFactoryName, Animation);
        public ArrayPropertyData MakeVertices(Asset asset, string name, List<FUimBase> vertices)
        {
            var propOut = new ArrayPropertyData(asset.GetOrAddName(name));
            propOut.ArrayType = asset.GetOrAddName("StructProperty");
            propOut.Value = GC.AllocateUninitializedArray<PropertyData>(vertices.Count);
            for (int i = 0; i < Geometry.Count; i++) propOut.Value[i] = vertices[i].Serialize(asset);
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
        public StructPropertyData Serialize(Asset asset)
        {
            var newUim = new StructPropertyData(asset.GetOrAddName("UimData"), asset.GetOrAddName("UimData"));
            newUim.Value = new();
            if (FrameSkip != null) newUim.Value.Add(Utils.MakeUintProperty(asset, "frameSkip", FrameSkip.Value));
            if (FrameNum != null) newUim.Value.Add(Utils.MakeIntProperty(asset, "frameNum", FrameNum.Value));
            if (VertexNum != null) newUim.Value.Add(Utils.MakeIntProperty(asset, "vertexNum", VertexNum.Value));
            if (PolygonNum != null) newUim.Value.Add(Utils.MakeIntProperty(asset, "polygonNum", PolygonNum.Value));
            if (IndexNum != null) newUim.Value.Add(Utils.MakeIntProperty(asset, "indexNum", IndexNum.Value));
            if (Coordinate != null) newUim.Value.Add(Utils.MakeIntProperty(asset, "coordinate", Coordinate.Value));
            if (GeomFormat != null) newUim.Value.Add(Utils.MakeIntProperty(asset, "geomFormat", GeomFormat.Value));
            if (AnimFormat != null) newUim.Value.Add(Utils.MakeIntProperty(asset, "animFormat", AnimFormat.Value));
            newUim.Value.Add(MakeGeometry(asset));
            newUim.Value.Add(MakeAnimation(asset));
            newUim.Value.Add(MakeIndices(asset));
            return newUim;
        }
    }
}
