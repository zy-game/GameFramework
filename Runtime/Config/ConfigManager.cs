using System.Collections.Generic;

namespace GameFramework.Config
{
    /// <summary>
    /// ���ñ������
    /// </summary>
    public sealed class ConfigManager : IConfigManager
    {
        private Dictionary<string, IRefrence> configs;
        private const string CONFIG_FILE_EXTENSION = ".cfg";

        public ConfigManager()
        {
            configs = new Dictionary<string, IRefrence>();
        }

        /// <summary>
        /// �����������ñ�
        /// </summary>
        public void Clear()
        {
            foreach (var item in configs.Values)
            {
                Loader.Release(item);
            }
        }

        /// <summary>
        /// ��ȡָ�������ñ�
        /// </summary>
        /// <param name="name">���ñ�����</param>
        /// <typeparam name="T">���ñ�</typeparam>
        /// <returns></returns>
        public IConfigDatable<T> GetConfigTable<T>(string name) where T : IConfig
        {
            if (configs.TryGetValue(name, out IRefrence table))
            {
                return (IConfigDatable<T>)table;
            }
            return default;
        }

        /// <summary>
        /// ��ѯ�Ƿ��Ѿ�����ָ�������ñ�
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasLoadConfig(string name) => configs.ContainsKey(name);

        /// <summary>
        /// ����ָ�������ñ�
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IConfigDatable<T> LoadConfig<T>(string name) where T : IConfig
        {
            IConfigDatable<T> configTable = Loader.Generate<IConfigDatable<T>>();
            configTable.Load(name);
            return configTable;
        }

        /// <summary>
        /// �������ñ�
        /// </summary>
        public void Release()
        {
            Clear();
        }

        /// <summary>
        /// ж��ָ�������ñ�
        /// </summary>
        /// <param name="name"></param>
        public void UnloadConfig(string name)
        {
            if (HasLoadConfig(name))
            {
                Loader.Release(configs[name]);
                configs.Remove(name);
            }
        }

        /// <summary>
        /// ��ѯ
        /// </summary>
        public void Update()
        {

        }
    }
}
