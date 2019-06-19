using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonTransformation
{
    /// <summary>
    /// Service that transforms source JToken using registered transformers
    /// </summary>
    public class JsonTransformationService
    {
        /// <summary>
        /// Array of registered transformers
        /// </summary>
        public ICanTransformJson[] Transformers { get; set; }

        public JsonTransformationService(params ICanTransformJson[] transformers)
        {
            this.Transformers = transformers;
        }

        /// <summary>
        /// Transforms specified jtoken using registered transformers recursively.
        /// Current implementation actually MUTATES source object, so keep it in mind
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
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

    /// <summary>
    /// Knows how to transform jtoken
    /// </summary>
    public interface ICanTransformJson
    {
        /// <summary>
        /// Returns true if some transformation actually was performed by this transformer
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        bool TransformJson(TransformJsonArgs args);
    }

    /// <summary>
    /// Transformation arguments
    /// </summary>
    public class TransformJsonArgs
    {
        /// <summary>
        /// Creates instance, allows to specify parent args
        /// </summary>
        /// <param name="parent"></param>
        public TransformJsonArgs(TransformJsonArgs parent=null)
        {
            this.Parent = parent;
            this.Context = parent?.Context ?? new Dictionary<object, object>();
        }

        /// <summary>
        /// Current JToken being transformed
        /// </summary>
        public JToken Source { get; set; }

        /// <summary>
        /// Transformation path can be calculated from here
        /// </summary>
        public TransformJsonArgs Parent { get; set; }

        /// <summary>
        /// Context shared within one transformation session (service.Transform(...))
        /// </summary>
        public Dictionary<object, object> Context { get; set; }
    }
}
