using JValueMasker.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace JValueMasker.Extensions
{
    /// <summary>
    /// Contains extension methods for JContainer
    /// </summary>
    public static class JContainerExtensions
    {
        /// <summary>
        /// Masks the values of specified properties on a JContainer.
        /// Pass in a list of the properties to mask. The values of all specified properties 
        /// will be masked, even those on nested JContainers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jContainer"></param>
        /// <param name="propsToMask"></param>
        /// <param name="stringComparison"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
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
