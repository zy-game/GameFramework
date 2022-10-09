using System;
namespace GameFramework.Game
{
    public interface ISoundManager : IRefrence
    {
        void PlaySound(string clipName);
        void PlaySound(string clipName, bool isLoop);
        void StopSound(string clipName);
        void PauseSound(string clipName);
        void ResumeSound(string clipName);
        void SetVolumen(float volemen);
    }
}