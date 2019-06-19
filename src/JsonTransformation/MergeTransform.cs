using Newtonsoft.Json.Linq;
using System;

namespace JsonTransformation
{
    public class MergeTransform : ICanTransformJson
    {
        public bool TransformJson(TransformJsonArgs args)
        {
            if (!(args.Source is JObject jSource)) return false;
            var jmerge = jSource["$merge"];
            if (jmerge == null) return false;

            JObject mergeObj = null;
            if(jmerge.Type == JTokenType.String)
            {
                var mergePath = jmerge.Value<string>();
                mergeObj = args.Source.Root.SelectToken(mergePath) as JObject;
            }
            else
            {
                mergeObj = jmerge as JObject;
            }

            if (mergeObj == null)
                throw new InvalidOperationException("$merge jpath must point to a JObject");

            jSource.Remove("$merge");
            jSource.Merge(mergeObj, new JsonMergeSettings { });
            return true;
        }
    }
}
