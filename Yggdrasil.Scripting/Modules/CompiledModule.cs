namespace Yggdrasil.Scripting.Modules
{
    public class CompiledModule<T> where T : IModule
    {
        public T Module;
        public ModuleInfo Info;
    }
}
