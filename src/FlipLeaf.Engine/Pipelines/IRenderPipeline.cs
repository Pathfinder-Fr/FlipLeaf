using System.IO;
using System.Threading.Tasks;

namespace FlipLeaf.Pipelines
{
    public interface IRenderPipeline
    {
        bool Accept(string fileName);

        string TransformTargetPath(string path, string targetPath);

        Task RenderAsync(string path, Stream output);
    }
}
