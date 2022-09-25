using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFramework
{
    /// <summary>
    /// 游戏运行时
    /// </summary>
    public sealed class Runtime
    {
        private static List<IGameModule> modules = new List<IGameModule>();

        public const string HOTFIX_FILE_LIST_NAME = "hotfixFileList.ini";
        public const string BASIC_FILE_LIST_NAME = "basicFileList.ini";

        public static void Initialize()
        {
            LoadGameModule<Events.EventManager>();
            LoadGameModule<Resource.ResourceManager>();
            LoadGameModule<Config.ConfigManager>();
            LoadGameModule<Datable.DatableManager>();
            LoadGameModule<Game.GameManager>();
            LoadGameModule<Network.NetworkManager>();
        }

        /// <summary>
        /// 关闭运行时
        /// </summary>
        public static void Shutdown()
        {
            modules.ForEach(Loader.Release);
            modules.Clear();
        }

        /// <summary>
        /// 加载游戏模块
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>游戏模块</returns>
        public static T LoadGameModule<T>() where T : IGameModule
        {
            return (T)LoadGameModule(typeof(T));
        }

        /// <summary>
        /// 加载游戏模块
        /// </summary>
        /// <param name="gameModuleType">模块类型</param>
        /// <returns>游戏模块</returns>
        public static IGameModule LoadGameModule(Type gameModuleType)
        {
            IGameModule gameModule = GetGameModule(gameModuleType);
            if (gameModule != null)
            {
                return gameModule;
            }
            gameModule = (IGameModule)Loader.Generate(gameModuleType);
            modules.Add(gameModule);
            return gameModule;
        }

        /// <summary>
        /// 获取游戏模块
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>游戏模块</returns>
        public static T GetGameModule<T>() where T : IGameModule
        {
            return (T)GetGameModule(typeof(T));
        }

        /// <summary>
        /// 获取游戏模块
        /// </summary>
        /// <param name="type">模块类型</param>
        /// <returns>游戏模块</returns>
        public static IGameModule GetGameModule(Type type)
        {
            type.EnsureObjectRefrenceType<IGameModule>();
            IGameModule module = default;
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].GetType() == type)
                {
                    module = modules[i];
                    break;
                }
            }
            return module;
        }

        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        public static void ShutdowmModule<T>() where T : IGameModule
        {
            ShutdowmModule(typeof(T));
        }

        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <param name="type">模块类型</param>
        public static void ShutdowmModule(Type type)
        {
            IGameModule gameModule = GetGameModule(type);
            if (gameModule == null)
            {
                return;
            }
            Loader.Release(gameModule);
            modules.Remove(gameModule);
        }

        /// <summary>
        /// 轮询游戏模块
        /// </summary>
        public static void Update()
        {
            for (int i = modules.Count - 1; i >= 0; i--)
            {
                SafeRun(modules[i].Update);
            }
        }

        private static void SafeRun(GameFrameworkAction action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                throw GameFrameworkException.Generate(e);
            }
        }
    }
}
