using JValueMasker.Extensions;
using JValueMasker.Utilities;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xunit;

namespace JValueMaskerTests.Unit
{
    [Trait("Category", "Unit")]
    public class JContainerExtensionsUnitTests
    {
        public class MaskValues
        {
            private const string PasswordProp = "password";
            private static readonly List<string> PropsToMask = new List<string>
            {
                PasswordProp
            };

            [Fact]
            public void ReturnsNull_WhenProvidedNullContainer()
            {
                var jContainer = (JContainer)null;

                var result = jContainer.MaskValues(PropsToMask);

                Assert.Null(result);
            }

            [Fact]
            public void ReturnsOriginalJContainer_WhenProvidedNullPropsToMask()
            {
                var jProperty = new JProperty("name", "Fred");

                var result = jProperty.MaskValues(null);

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(jProperty, result);
            }

            [Fact]
            public void ReturnsOriginalJContainer_WhenProvidedEmptyPropsToMask()
            {
                var jProperty = new JProperty("name", "Fred");

                var result = jProperty.MaskValues(new List<string>());

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(jProperty, result);
            }

            [Fact]
            public void ReturnsJPropertyWithMaskedValue_WhenProvidedJPropertyWithPropertyToMask()
            {
                var jProperty = new JProperty(PasswordProp, "badPassword");

                var result = jProperty.MaskValues(PropsToMask);

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(PasswordProp, result.Name);
                Assert.Equal(MaskerUtility.DefaultMask, result.Value);
            }

            [Fact]
            public void ReturnsJObjectWithMaskedValue_WhenProvidedJObjectWithPropertyToMask()
            {
                var jObject = new JObject(
                                  new JProperty(PasswordProp, "badPassword"));

                var result = jObject.MaskValues(PropsToMask);
                var maskedPassword = result[PasswordProp].Value<string>();

                Assert.NotNull(result);
                Assert.IsType<JObject>(result);
                Assert.Equal(MaskerUtility.DefaultMask, maskedPassword);
            }

            [Fact]
            public void ReturnsJArrayWithMaskedValue_WhenProvidedJArrayWithPropertyToMask()
            {
                var jArray = new JArray(
                             new JObject(
                                 new JProperty(PasswordProp, "badPassword")));

                var result = jArray.MaskValues(PropsToMask);
                var maskedPassword = result[0][PasswordProp].Value<string>();

                Assert.NotNull(result);
                Assert.IsType<JArray>(result);
                Assert.Equal(MaskerUtility.DefaultMask, maskedPassword);
            }
        }
    }
}
