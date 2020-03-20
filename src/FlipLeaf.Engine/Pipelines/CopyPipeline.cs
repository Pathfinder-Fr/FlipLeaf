using System;
using System.IO;
using System.Threading.Tasks;

namespace FlipLeaf.Pipelines
{
    public class CopyPipeline : IRenderPipeline
    {
        private readonly RenderContext _ctx;

        public CopyPipeline(RenderContext ctx)
        {
            _ctx = ctx;
        }

        public bool Accept(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            var extension = Path.GetExtension(fileName);

            if (fileName.Length == extension.Length)
            {
                // ignore all "." files (.git, .xxx)
                return false;
            }

            if (fileName.Equals(_ctx.ConfigFile, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public string TransformTargetPath(string path, string outPath) => outPath;

        public Task RenderAsync(string path, string outPath)
        {
            File.Copy(path, outPath, true);
            return Task.CompletedTask;
        }
    }
}
