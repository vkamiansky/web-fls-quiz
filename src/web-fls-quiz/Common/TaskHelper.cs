using System.Threading.Tasks;

namespace WebFlsQuiz.Common
{
    public static class TaskExtensions
    {
        public static async Task<T> ExecuteWithTimeout<T>(this Task<T> operation, int timeout)
        {
            if (await Task.WhenAny(operation, Task.Delay(timeout)) == operation)
            {
                return operation.Result;
            }
            else
            {
                return default(T);
            }
        }
    }
}