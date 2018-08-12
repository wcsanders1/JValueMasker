using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace JValueMasker.Utilities
{
    internal static class Masker
    {
        internal const string DefaultMask = "***";

        public static T Mask<T>(T jToken, List<string> propertiesToMask)
            where T : JToken
        {
            if (jToken == null)
            {
                return null;
            }

            if (jToken is JObject)
            {
                var obj = jToken as JObject;
                return MaskObject(obj, propertiesToMask) as T;
            }

            if (jToken is JArray)
            {
                var arr = jToken as JArray;
                return MaskArray(arr, propertiesToMask) as T;
            }

            if (jToken is JProperty)
            {
                var prop = jToken as JProperty;
                return MaskProperty(prop, propertiesToMask) as T;
            }

            return jToken;
        }

        private static JProperty MaskProperty(JProperty jProperty, 
                                     List<string> propertiesToMask, 
                                     string mask = DefaultMask)
        {
            var property = jProperty.Name;
            var value = jProperty.Value;
            if (value is JValue && ShouldBeMasked(property, propertiesToMask))
            {
                return new JProperty(property, mask);
            }

            return new JProperty(property, Mask(value, propertiesToMask));
        }

        private static JObject MaskObject(JObject jObject, List<string> propertiesToMask)
        {
            var maskedJObject = new JObject();
            foreach (var j in jObject)
            {
                var prop = j.Value as JProperty;
                maskedJObject.Add(j.Key, MaskProperty(prop, propertiesToMask));
            }

            return maskedJObject;
        }

        private static JArray MaskArray(JArray jArray, List<string> propertiesToMask)
        {
            var maskedJArray = new JArray();
            foreach (var j in jArray)
            {
                maskedJArray.Add(Mask(j, propertiesToMask));
            }

            return maskedJArray;
        }

        private static bool ShouldBeMasked(string propertyName, List<string> propertiesToMask)
        {
            return propertiesToMask.Contains(propertyName);
        }
    }
}
