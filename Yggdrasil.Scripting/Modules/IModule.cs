using System.Threading.Tasks;

namespace Yggdrasil.Scripting.Modules
{
    public interface IModule
    {
        Task Load(BehaviourTreeDefinition repository);
    }
}
