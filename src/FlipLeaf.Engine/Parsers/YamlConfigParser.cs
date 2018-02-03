using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlipLeaf.Parsers
{
    public class YamlConfigParser
    {
        public SiteSettings ParseConfig(string path)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            using (var file = File.OpenRead(path))
            using (var reader = new StreamReader(file))
            {
                var parser = new Parser(reader);
                parser.Expect<StreamStart>();

                return deserializer.Deserialize<SiteSettings>(parser);
            }
        }
    }
}