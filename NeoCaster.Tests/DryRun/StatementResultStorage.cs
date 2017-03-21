using System.IO;
using Neo4j.Driver.V1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeoCaster.Tests.DryRun
{
    public class StatementResultStorage
    {
        private MemoryStream _memoryStream;

        public TextWriter Sink
        {
            get
            {
                _memoryStream = new MemoryStream();
                return new StreamWriter(_memoryStream);
            }
        }

        public IStatementResult ProduceDryResult()
        {
            return new DryStatementResult(DryStatementResultSkeleton);
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