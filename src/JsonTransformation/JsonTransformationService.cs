using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonTransformation
{
    public class JsonTransformationService
    {
        public ICanTransformJson[] Transformers { get; set; }

        public JsonTransformationService(params ICanTransformJson[] transformers)
        {
            this.Transformers = transformers;
        }

        public JToken Transform(JToken source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var transformers = (Transformers ?? Array.Empty<ICanTransformJson>());
            var root = source.Root;


            void TransformDfs(TransformJsonArgs args)
            {
                //first we go deeper and then try to tranform current source, if something was transformed, we go deep again, since children might have changed
                do
                {
                    if (args.Source == null||args.Source.Root!=root) break;
                    if (args.Source is JObject jobj)
                    {
                        foreach (var jprop in jobj.Properties())
                        {
                            var childArgs = new TransformJsonArgs(args) { Source = jprop.Value};
                            TransformDfs(childArgs);
                        }
                    }

                } while (transformers.Select(t => {

                    if (args.Source == null||args.Source.Root!=root) return (t, transformed:false);
                    else return (t, transformed:t.TransformJson(args));
                }).ToArray().Any(t => t.transformed));
            }

            var rootArgs = new TransformJsonArgs { Source = source };
            TransformDfs(rootArgs);
            return rootArgs.Source;
        }
    }

    public interface ICanTransformJson
    {
        bool TransformJson(TransformJsonArgs args);
    }

    public class TransformJsonArgs
    {
        public TransformJsonArgs(TransformJsonArgs parent=null)
        {
            this.Parent = parent;
            this.Context = parent?.Context ?? new Dictionary<object, object>();
        }

        public JToken Source { get; set; }
        //public JToken Target { get; set; }

        public TransformJsonArgs Parent { get; set; }

        public Dictionary<object, object> Context { get; set; }
    }
}
