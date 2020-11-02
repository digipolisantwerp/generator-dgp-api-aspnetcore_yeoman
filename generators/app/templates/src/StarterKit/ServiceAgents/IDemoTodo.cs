using System.Threading.Tasks;

namespace StarterKit.ServiceAgents
{
    public interface IDemoTodo
    {
        Task<string> GetTodosAsStringAsync();
    }
}