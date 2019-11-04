using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IImageService
    {
        IOperationResult LoadIfNeeded(StandardImage image);
    }
}
