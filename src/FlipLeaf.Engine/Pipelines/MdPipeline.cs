using System;
using System.IO;

namespace FlipLeaf.Pipelines
{
    public class MdPipeline : IRenderPipeline
    {
        private readonly Parsers.FluidParser _fluid;
        private readonly Parsers.YamlParser _yaml;
        private readonly Parsers.MarkdownParser _md;
        private readonly RenderContext _ctx;

        public MdPipeline(RenderContext ctx, Parsers.FluidParser fluid, Parsers.YamlParser yaml, Parsers.MarkdownParser md)
        {
            _fluid = fluid;
            _yaml = yaml;
            _md = md;
            _ctx = ctx;
        }

        public bool Accept(string fileName) => fileName.EndsWith(".md", StringComparison.OrdinalIgnoreCase);

        public string TransformTargetPath(string path, string outPath) => Path.ChangeExtension(outPath, ".html");

        public void Render(string path, string outPath)
        {
            var content = File.ReadAllText(path);

            // 1) yaml
            if (!_yaml.ParseHeader(ref content, out var pageContext))
            {
                return;
            }

            // 2) liquid
            content = _fluid.ParseContent(content, pageContext, out var templateContext);

            // 3) markdown
            if (!_md.Parse(ref content))
            {
                return;
            }

            // 4) layout
            content = _fluid.ApplyLayout(content, templateContext);
            
            File.WriteAllText(outPath, content);
        }
    }
}