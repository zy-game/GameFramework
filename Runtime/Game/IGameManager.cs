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
        Init,
        Runing,
        Stoped,
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

    public sealed class GameManager : IGameManager
    {
        public void Release()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
