using System.Threading.Tasks;

namespace Sedio.Execution
{
    public interface IExecutionController
    {
        bool CanExecute(ExecutionContext context);

        Task Execute(ExecutionContext context);
    }
}