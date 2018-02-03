using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlipLeaf
{
    public class Engine
    {
        private readonly SiteSettings _site;
        private readonly RuntimeSettings _runtime;
        private readonly RenderContext _context;
        private readonly Serilog.ILogger _log;
        private readonly IEnumerable<Pipelines.IRenderPipeline> _pipelines;
        private readonly Pipelines.IRenderPipeline _defaultPipeline;

        public Engine(RenderContext ctx, SiteSettings site, RuntimeSettings runtime, Serilog.ILogger log, IEnumerable<Pipelines.IRenderPipeline> pipelines)
        {
            _site = site;
            _runtime = runtime;
            _context = ctx;
            _log = log;
            _pipelines = pipelines;
            _defaultPipeline = new Pipelines.CopyPipeline(ctx);
        }

        public SiteSettings Site { get; }

        public void RenderAll()
        {
            RenderFolder(string.Empty);
        }

        private void RenderFolder(string directory)
        {
            var srcDir = Path.Combine(_context.InputDir, directory);
            var targetDir = Path.Combine(_context.InputDir, _context.OutputDir, directory);

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            foreach (var file in Directory.GetFiles(srcDir))
            {
                RenderFile(Path.Combine(directory, Path.GetFileName(file)), Path.Combine(targetDir, Path.GetFileName(file)));
            }

            foreach (var subDir in Directory.GetDirectories(srcDir))
            {
                var directoryName = Path.GetFileName(subDir);

                if (string.Equals(directoryName, _context.OutputDir, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.Equals(directoryName, _site.LayoutFolder, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (directory.StartsWith("."))
                {
                    continue;
                }

                RenderFolder(Path.Combine(subDir, subDir));
            }
        }

        private void RenderFile(string pagePath, string targetPath)
        {
            var path = Path.Combine(_context.InputDir, pagePath);

            // find pipeline
            var pipeline = _pipelines.FirstOrDefault(x => x.Accept(path));
            if (pipeline == null && _defaultPipeline.Accept(path))
            {
                pipeline = _defaultPipeline;
            }

            if (pipeline != null)
            {
                // transform target path
                targetPath = pipeline.TransformTargetPath(path, targetPath);

                // render
                _log.Information("Rendering {Src} to {Dest}", path, targetPath);
                pipeline.Render(path, targetPath);
            }
        }
    }
}