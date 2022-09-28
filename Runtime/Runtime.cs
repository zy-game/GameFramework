using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFramework
{

    public sealed class AppConfig
    {
        public const string HOTFIX_FILE_LIST_NAME = "fileList.ini";
    }
    /// <summary>
    /// 游戏运行时
    /// </summary>
    public sealed class Runtime
    {
        sealed class INTERNAL_MonoBehaviour : MonoBehaviour
        {
            private void Update()
            {
                Runtime.Update();
            }

            private void OnApplicationQuit()
            {
                Runtime.Shutdown();
            }
        }
        private static INTERNAL_MonoBehaviour runtime;
        private static List<IGameModule> modules = new List<IGameModule>();

        public static int MaxDownloadCount = 5;
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            runtime = new GameObject("Runtime").AddComponent<INTERNAL_MonoBehaviour>();
            GameObject.DontDestroyOnLoad(runtime.gameObject);
        }

        public static void StartCoroutine(IEnumerator ie)
        {
            runtime.StartCoroutine(ie);
        }

        public static void StartCoroutine(AsyncOperation ie)
        {
            runtime.StartCoroutine(INTERNAL_RunningIEnumerator(ie));
        }
        static IEnumerator INTERNAL_RunningIEnumerator(AsyncOperation ie)
        {
            yield return ie;
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
            IGameModule module = modules.AsParallel().Where(x => x.GetType() == type).FirstOrDefault();
            if (module == null)
            {
                module = (IGameModule)Loader.Generate(type);
                modules.Add(module);
            }
            return module;
        }

        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        public static void UnloadModule<T>() where T : IGameModule
        {
            UnloadModule(typeof(T));
        }

        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <param name="type">模块类型</param>
        public static void UnloadModule(Type type)
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
