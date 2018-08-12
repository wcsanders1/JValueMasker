using JValueMasker.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace JValueMasker.Extensions
{
    public static class JContainerExtensions
    {
        public static T MaskValues<T>(this T jContainer, List<string> propsToMask,
#if NETSTANDARD2_0
            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase,
#else
            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase,
#endif
            string mask = MaskerUtility.DefaultMask) where T : JContainer
        {
            if (jContainer == null)
            {
                return null;
            }

            if (propsToMask == null || propsToMask.Count == 0)
            {
                return jContainer;
            }

            return MaskerUtility.Mask(jContainer, propsToMask,
                stringComparison, mask);
        }
    }
}
