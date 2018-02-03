namespace FlipLeaf.Parsers
{
    public class MarkdownParser
    {
        public bool Parse(ref string source)
        {
            source = Markdig.Markdown.ToHtml(source);

            return true;
        }
    }
}