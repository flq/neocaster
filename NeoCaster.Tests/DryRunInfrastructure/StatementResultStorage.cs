using System;
using System.IO;
using Neo4j.Driver.V1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeoCaster.Tests.DryRunInfrastructure
{
    /// <summary>
    /// Construct an <see cref="IStatementResult"/> instance
    /// (implemented by <see cref="DryStatementResult"/>) based on an incoming stream that is written to
    /// by using the exposed TextWriter at <see cref="Sink"/>.
     /// </summary>
    public class StatementResultStorage
    {
        private MemoryStream _memoryStream;

        /// <summary>
        /// Use this to write into the stream that is used to produce the statement result when calling
        /// <see cref="ProduceDryResult"/>. This stream expects a valid json array.
        /// You can obtain one by calling <see cref="RenderStatementResultExtension.Render"/>
        /// </summary>
        public TextWriter Sink
        {
            get
            {
                _memoryStream = new MemoryStream();
                return new StreamWriter(_memoryStream);
            }
        }

        /// <summary>
        /// Build a <see cref="IStatementResult"/> instance
        /// (implemented by <see cref="DryStatementResult"/>) from the provided data entered through the
        /// <see cref="Sink"/>
        /// </summary>
        /// <returns>A <see cref="DryStatementResult"/> instance</returns>
        public IStatementResult ProduceDryResult()
        {
            return new DryStatementResult(DryStatementResultSkeleton);
        }

        /// <summary>
        /// Use this in preparation to create a new embedded resource by storing the captured StatementResult.  
        /// The default path is your desktop.
        /// </summary>
        // ReSharper disable once UnusedMember.Global - This is used sporadically to extract a statementresult from a running neo4j
        public void SaveRepresentation(string path = null)
        {
            path = Path.Combine(path ?? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "new.json");
            using (var writeTarget = new JsonTextWriter(new StreamWriter(File.OpenWrite(path))))
              DryStatementResultSkeleton.WriteTo(writeTarget);
        }

        private JArray DryStatementResultSkeleton
        {
            get
            {
                if (_memoryStream == null || _memoryStream.Length == 0)
                    return new JArray();
                _memoryStream.Seek(0, SeekOrigin.Begin);
                return GetContentsFromStream(_memoryStream);
            }
        }

        public static JArray GetContentsFromStream(Stream stream)
        {
            return new JsonSerializer().Deserialize<JArray>(
                new JsonTextReader(new StreamReader(stream)));
        }
    }
}