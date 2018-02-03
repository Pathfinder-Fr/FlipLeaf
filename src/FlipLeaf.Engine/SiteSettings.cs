namespace FlipLeaf
{
    public class SiteSettings
    {
        public const string DefaultFileName = "_config.yml";
        private const string DefaultLayoutsFolder = "_layouts";
        private const string DefaultOutputFolder = "_site";

        public string Title { get; set; }

        public string LayoutFolder { get; set; } = DefaultLayoutsFolder;

        public string OutputFolder { get; set; } = DefaultOutputFolder;
    }
}
