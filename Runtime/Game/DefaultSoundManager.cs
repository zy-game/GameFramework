using System;
namespace GameFramework.Game
{
    public sealed class DefaultSoundManager : ISoundManager
    {
        public void PauseSound(string clipName)
        {
            throw new NotImplementedException();
        }

        public void PlaySound(string clipName)
        {
            throw new NotImplementedException();
        }

        public void PlaySound(string clipName, bool isLoop)
        {
            throw new NotImplementedException();
        }

        public void Release()
        {
            throw new NotImplementedException();
        }

        public void ResumeSound(string clipName)
        {
            throw new NotImplementedException();
        }

        public void SetVolumen(float volemen)
        {
            throw new NotImplementedException();
        }

        public void StopSound(string clipName)
        {
            throw new NotImplementedException();
        }

        public static ISoundManager Generate(IGameWorld world)
        {
            return default;
        }
    }
}