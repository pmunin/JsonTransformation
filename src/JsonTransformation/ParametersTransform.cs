using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace JsonTransformation
{
    public class ParametersTransform : ICanTransformJson
    {
        public bool TransformJson(TransformJsonArgs args)
        {
            if (!(args.Source is JObject jSource)) return false;
            if (jSource["$setParameters"] is JObject jSetParameters) return SetParameters(args, jSetParameters);
            else if (jSource["$useParameter"] is JToken jUseParameter) return UseParameter(args, jUseParameter);

            return false;
        }

        bool UseParameter(TransformJsonArgs args, JToken jUseParameter)
        {
            var parName = jUseParameter.Value<string>();
            if (args.Context.TryGetValue("paramByName", out object paramByNameObj) && paramByNameObj is Dictionary<string, JToken> paramByName)
            {
                if (paramByName.TryGetValue(parName, out var parValue))
                {
                    var newNode = parValue.DeepClone();
                    args.Source.Replace(newNode);
                    args.Source = newNode;
                    return true;
                }
            }
            return false;
        }

        bool SetParameters(TransformJsonArgs args, JObject jSetParameters)
        {
            if (!(args.Context.TryGetValue("paramByName", out object paramByNameObj) && paramByNameObj is Dictionary<string, JToken> paramByName))
            {
                paramByName = new Dictionary<string, JToken>();
                args.Context["paramByName"] = paramByName;
            }

            foreach (var jProp in jSetParameters.Properties())
            {
                paramByName[jProp.Name] = jProp.Value;
            }

            (args.Source as JObject).Remove("$setParameters");

            //args.Source.Remove();
            //args.Source = null;

            return false;
        }
    }
}
