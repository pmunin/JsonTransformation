using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Xunit;

namespace JsonTransformation.Test
{
    public class MergeTests
    {
        [Fact]
        public void Merge1()
        {
            var svc = new JsonTransformationService(new MergeTransform());
            var jsrc = File.ReadAllText("test.merge.1.json");

            var srcRoot = JObject.Parse(jsrc);
            var job1 = srcRoot.SelectToken("$.jobs.job1");

            var res = svc.Transform(job1);

            Assert.True(res["job1Prop1"]?.Value<int>() == 1235);
            Assert.True(res["tp2"]?.Value<string>() == "Hello world");
        }
    }
}
