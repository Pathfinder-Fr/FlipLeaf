namespace FlipLeaf
{
    /// <summary>
    /// Represents the settings sets by the program at runtime (such as command line options).
    /// This settings can override some <see cref="SiteSettings"/>.
    /// </summary>
    public class RuntimeSettings
    {
        /// <summary>
        /// Gets or sets the path where the project files are stored.
        /// </summary>
        public string InputDir { get; set; }

        /// <summary>
        /// Gets or sets the path where the rendered files must be written.
        /// </summary>
        public string OutputDir { get; set; }

        public string ConfigFile { get; set; }
    }
}
