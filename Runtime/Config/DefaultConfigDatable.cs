using System.IO;
using System.Collections.Generic;
using System.Text;

namespace GameFramework.Config
{
    /// <summary>
    /// 默认配置表对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    sealed class DefaultConfigDatable<T> : IConfigDatable<T> where T : IConfig
    {

        private List<T> configs = new List<T>();

        /// <summary>
        /// 配置表名称
        /// </summary>
        /// <value></value>
        public string name
        {
            get;
            private set;
        }

        /// <summary>
        /// 当前配项表数目
        /// </summary>
        /// <value></value>
        public int Count
        {
            get
            {
                return configs.Count;
            }
        }

        /// <summary>
        /// 添加一个配置项
        /// </summary>
        /// <param name="config">配置项</param>
        public void Add(T config)
        {
            if (HasConfig(config.id) || HasConfig(config.name))
            {
                throw GameFrameworkException.GenerateFormat("the config is already exsit id:{0} name:{1}", config.id, config.name);
            }
            configs.Add(config);
        }

        /// <summary>
        /// 获取指定的配置项
        /// </summary>
        /// <param name="id">配置项ID</param>
        /// <returns>配置项</returns>
        public T GetConfig(int id) => configs.Find(x => x.id == id);

        /// <summary>
        /// 获取指定的配置项
        /// </summary>
        /// <param name="name">配置项名称</param>
        /// <returns>配置项</returns>
        public T GetConfig(string name) => configs.Find(x => x.name == name);

        /// <summary>
        /// 查询是否存在指定的配置ID
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>是否存在指定的配置ID</returns>
        public bool HasConfig(int id) => GetConfig(id) != null;

        /// <summary>
        /// 查询是否存在指定的配置名称
        /// </summary>
        /// <param name="name">配置名称</param>
        /// <returns>是否存在指定的配置名称</returns>
        public bool HasConfig(string name) => GetConfig(name) != null;

        /// <summary>
        /// 加载配置表
        /// </summary>
        /// <param name="configName">配置表名称</param>
        /// <returns></returns>
        public async void Load(string configName)
        {
            Resource.ResourceManager resourceManager = Runtime.GetGameModule<Resource.ResourceManager>();
            DataStream stream = await resourceManager.ReadFileAsync(configName);
            if (stream == null || stream.position <= 0)
            {
                throw GameFrameworkException.Generate<FileNotFoundException>();
            }
            configs = CatJson.JsonParser.ParseJson<List<T>>(stream.ToString());
        }

        /// <summary>
        /// 保存配置表
        /// </summary>
        /// <returns></returns>
        public async void Save()
        {
            Resource.ResourceManager resourceManager = Runtime.GetGameModule<Resource.ResourceManager>();
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(CatJson.JsonParser.ToJson(configs));
            DataStream stream = DataStream.Generate(bytes);
            await resourceManager.WriteFileAsync(name, stream);
        }

        /// <summary>
        /// 回收配置表
        /// </summary>
        public void Release()
        {
            foreach (var item in configs)
            {
                Loader.Release(item);
            }
            configs.Clear();
        }

        /// <summary>
        /// 移除一个配置项
        /// </summary>
        /// <param name="id">配置项ID</param>
        public void Remove(int id) => Remove(GetConfig(id));

        /// <summary>
        /// 移除一个配置项
        /// </summary>
        /// <param name="name">配置项名称</param>
        public void Remove(string name) => Remove(GetConfig(name));

        /// <summary>
        /// 移除一个配置项
        /// </summary>
        /// <param name="config">配置项</param>
        public void Remove(T config)
        {
            if (config == null)
            {
                return;
            }
            configs.Remove(config);
        }

        /// <summary>
        /// 尝试获取一个配置项
        /// </summary>
        /// <param name="id">配置项ID</param>
        /// <param name="config">配置项</param>
        /// <returns>是否存在配置项</returns>
        public bool TryGetConfig(int id, out T config)
        {
            config = GetConfig(id);
            return config != null;
        }

        /// <summary>
        /// 尝试获取一个配置项
        /// </summary>
        /// <param name="name">配置项名称</param>
        /// <param name="config">配置对象</param>
        /// <returns>是否存在配置项</returns>
        public bool TryGetConfig(string name, out T config)
        {
            config = GetConfig(name);
            return config != null;
        }
    }
}
