using System;
using System.Collections.Generic;
using System.Text;

namespace CompileTimeCaching
{
    public class CacheBuilderOptions
    {
        /// <summary>
        /// The name to use for the type that is created as a result of the caching.
        /// </summary>
        public string GeneratedTypeName { get; set; }
        /// <summary>
        /// The namespace which contains the generated class.
        /// </summary>
        public string GeneratedNamespace { get; set; }
    }
}
