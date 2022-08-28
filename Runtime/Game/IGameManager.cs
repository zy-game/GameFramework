using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework.Game
{
    public enum GameState : byte
    {
        None,
        Initialize,
        Runing,
        Stoping,
        Destory,
    }
    public interface IGame : IRefrence
    {
        bool activeSelf { get; }
        int EntityCount { get; }
        GameState State { get; }
        void LoadScript<T>() where T : IGameScript;
        void UnloadScript<T>() where T : IGameScript;
        void Update();
        GameEntity GenerateGameEntity();
        GameEntity GetGameEntity(int id);
        void RemoveGameEntity(int id);
    }
    public interface IComponent : IRefrence
    {

    }
    public interface IGameScript : IRefrence
    {

    }
    public sealed class GameEntity : IRefrence
    {
        public void Release()
        {
        }
    }
    public interface IGameManager : IGameModule
    {
    }
}
