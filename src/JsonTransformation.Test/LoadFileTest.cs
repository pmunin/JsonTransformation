using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Xunit;

namespace JsonTransformation.Test
{
    public class LoadFileTests
    {
        [Fact]
        public void LoadFile()
        {
            var svc = new JsonTransformationService(new LoadFileTransform() );

            var srcText = File.ReadAllText("test.loadFile.1.json");
            var src = JObject.Parse(srcText);

            var res = svc.Transform(src);
            Assert.True(res["linkedFile"]?["myFile2Prop1"]?.Value<int>() == 123);
            Assert.True(res["linkedFile"]?["nestedFile3"]?["myFile3Prop1"]?.Value<string>() == "File 3");
        }
    }
}
