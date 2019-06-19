using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace JsonTransformation
{
    public class LoadFileTransform : ICanTransformJson
    {
        public bool TransformJson(TransformJsonArgs args)
        {
            if (!(args.Source is JObject jobj)) return false;
            var filePath = jobj["$file"]?.Value<string>();
            if (filePath == null) return false;
            jobj.Remove("$file");
            using (var fr = File.OpenText(filePath))
                using(var jtr = new JsonTextReader(fr))
            {
                var jfilecontent = JObject.ReadFrom(jtr);
                jobj.Merge(jfilecontent);
            }
            return true;
        }
    }
}
