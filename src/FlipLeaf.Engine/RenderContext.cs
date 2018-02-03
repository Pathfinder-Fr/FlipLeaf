using System;
using System.Collections.Generic;
using System.Text;

namespace FlipLeaf
{
    public class RenderContext
    {
        private readonly SiteSettings _site;
        private readonly RuntimeSettings _runtime;

        public RenderContext(SiteSettings site, RuntimeSettings runtime)
        {
            _site = site;
            _runtime = runtime;

            this.OutputDir = _runtime.OutputDir ?? _site.OutputFolder;
            this.ConfigFile = _runtime.ConfigFile ?? SiteSettings.DefaultFileName;
        }

        public string InputDir => _runtime.InputDir;

        public string OutputDir { get; }

        public string ConfigFile { get; }

    }
}
