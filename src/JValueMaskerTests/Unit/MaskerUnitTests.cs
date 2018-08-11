using JValueMasker.Utilities;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xunit;


namespace JValueMaskerTests.Unit
{
    [Trait("Category", "Unit")]
    public class MaskerUnitTests
    {
        public class Mask
        {
            [Fact]
            public void ReturnsMaskedValue_WhenValueShouldBeMasked()
            {
                const string passwordProp = "password";
                const string passwordVal = "badpassword123";

                var propsToMask = new List<string>
                {
                    "password"
                };

                var jProp = new JProperty(passwordProp, passwordVal);
                var result = Masker.MaskProperty(jProp, propsToMask);

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(passwordProp, result.Name);
                Assert.Equal(Masker.DefaultMask, result.Value);
            }

            [Fact]
            public void ReturnsOriginalValue_WhenValueShouldNotBeMasked()
            {
                const string nameProp = "name";
                const string nameVal = "Sam";

                var propsToMask = new List<string>
                {
                    "password"
                };

                var jProp = new JProperty(nameProp, nameVal);
                var result = Masker.MaskProperty(jProp, propsToMask);

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(nameProp, result.Name);
                Assert.Equal(nameVal, result.Value);
            }
        }
    }
}
