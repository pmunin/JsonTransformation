using System;
using Xunit;

namespace JsonTransformation.Test
{
    public class JsonTransformationServiceTests
    {
        [Fact]
        public void Test1()
        {
            var instance = new JsonTransformationService();
            Assert.Throws<ArgumentNullException>(() =>
            {
                instance.Transform(null);
            });
        }
    }
}
