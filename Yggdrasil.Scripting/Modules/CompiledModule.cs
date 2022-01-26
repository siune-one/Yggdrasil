namespace Yggdrasil.Scripting.Modules
{
    public class CompiledModule<T> where T : IModule
    {
        public T Module;
        public ModuleMetadata Metadata;
        public ModuleErrorType Error;
        public string ErrorMessage;
        public string Directory;
    }
}
