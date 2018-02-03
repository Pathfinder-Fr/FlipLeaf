namespace FlipLeaf.Pipelines
{
    public interface IRenderPipeline
    {
        bool Accept(string fileName);

        string TransformTargetPath(string path, string targetPath);

        void Render(string path, string targetPath);
    }
}
