using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.PropertyTypes.Objects;

namespace plgtool
{
    public class Utils
    {
        public static IntPropertyData MakeIntProperty(Asset asset, string name, int value)
        {
            var propOut = new IntPropertyData(asset.GetOrAddName(name));
            propOut.Value = value;
            return propOut;
        }
        public static UInt32PropertyData MakeUintProperty(Asset asset, string name, uint value)
        {
            var propOut = new UInt32PropertyData(asset.GetOrAddName(name));
            propOut.Value = value;
            return propOut;
        }
        public static FloatPropertyData MakeFloatProperty(Asset asset, string name, float value)
        {
            var propOut = new FloatPropertyData(asset.GetOrAddName(name));
            propOut.Value = value;
            return propOut;
        }
        public static NamePropertyData MakeNameProperty(Asset asset, string propName, string valueName)
        {
            var propOut = new NamePropertyData(asset.GetOrAddName(propName));
            propOut.Value = asset.GetOrAddName(valueName);
            return propOut;
        }

        public static T? GetValueMaybeNull<T>(JObject parent, string field) where T : struct
        {
            if (parent.TryGetValue(field, out var value)) return value.Value<T>();
            return null;
        }
    }
}
