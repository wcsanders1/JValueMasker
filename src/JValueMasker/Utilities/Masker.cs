using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JValueMasker.Utilities
{
    internal static class Masker
    {
        internal const string DefaultMask = "***";

        public static T Mask<T>(T jToken, List<string> propertiesToMask,
            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
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

        private static JProperty MaskProperty(JProperty jProperty, List<string> propertiesToMask, 
            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase,
            string mask = DefaultMask)
        {
            var property = jProperty.Name;
            var value = jProperty.Value;
            if (value is JValue && ShouldBeMasked(property, propertiesToMask, stringComparison))
            {
                return new JProperty(property, mask);
            }

            return new JProperty(property, Mask(value, propertiesToMask));
        }

        private static JObject MaskObject(JObject jObject, List<string> propertiesToMask)
        {
            var maskedJObject = new JObject();
            foreach (var obj in jObject)
            {
                var prop = new JProperty(obj.Key, obj.Value);
                maskedJObject.Add(MaskProperty(prop, propertiesToMask));
            }

            return maskedJObject;
        }

        private static JArray MaskArray(JArray jArray, List<string> propertiesToMask)
        {
            var maskedJArray = new JArray();
            foreach (var element in jArray)
            {
                maskedJArray.Add(Mask(element, propertiesToMask));
            }

            return maskedJArray;
        }

        private static bool ShouldBeMasked(string propertyName, List<string> propertiesToMask,
            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return propertiesToMask.Any(p => p.Equals(propertyName, stringComparison));
        }
    }
}
