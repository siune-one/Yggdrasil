namespace Yggdrasil.Coroutines
{
    public interface IContinuation
    {
        void MoveNext();
        void Discard();
    }
}