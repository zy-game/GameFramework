namespace GameFramework.Resource
{
    public interface IResourceUpdateListenerHandler : IRefrence
    {
        void Progres(float progres);
        void Completed(ResourceUpdateState state);
    }
}
