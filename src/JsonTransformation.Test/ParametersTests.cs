using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Xunit;

namespace JsonTransformation.Test
{
    public class ParametersTests
    {
        [Fact]
        public void TestParameters()
        {
            var svc = new JsonTransformationService( 
                new MergeTransform(), 
                new ParametersTransform()
            );

            var srcText = File.ReadAllText("test.parameters.1.json");
            var srcRoot = JObject.Parse(srcText);

            var job1 = srcRoot.SelectToken("$.jobs.job1");

            var res = svc.Transform(job1);
            Assert.True(job1["name"]?.Value<string>() == "My Job 1");
        }
    }
}
