namespace GameFramework.Resource
{
    sealed class DefaultResourceUpdateListenerHandle : IResourceUpdateListenerHandler
    {
        private GameFrameworkAction<float> progresCallback;
        private GameFrameworkAction<ResourceUpdateState> compoleted;
        public void Completed(ResourceUpdateState state)
        {
            if (compoleted == null)
            {
                return;
            }
            compoleted(state);
        }

        public void Progres(float progres)
        {
            if (progresCallback == null)
            {
                return;
            }
            progresCallback(progres);
        }

        public void Release()
        {
            compoleted = null;
            progresCallback = null;
        }



        public static DefaultResourceUpdateListenerHandle Generate(GameFrameworkAction<float> progresCallback, GameFrameworkAction<ResourceUpdateState> compoleted)
        {
            DefaultResourceUpdateListenerHandle defaultResourceUpdateListenerHandle = Loader.Generate<DefaultResourceUpdateListenerHandle>();
            defaultResourceUpdateListenerHandle.progresCallback = progresCallback;
            defaultResourceUpdateListenerHandle.compoleted = compoleted;
            return defaultResourceUpdateListenerHandle;
        }
    }
}
