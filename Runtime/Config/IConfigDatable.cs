namespace GameFramework.Config
{
    /// <summary>
    /// 配置表
    /// </summary>
    /// <typeparam name="T">配置项类型</typeparam>
    public interface IConfigDatable<T> : IRefrence where T : IConfig
    {
        /// <summary>
        /// 配置表名
        /// </summary>
        /// <value></value>
        string name { get; }
        
        /// <summary>
        /// 配置项数量
        /// </summary>
        /// <value></value>
        int Count { get; }

        /// <summary>
        /// 加载配置表
        /// </summary>
        /// <param name="configName">配置表名</param>
        /// <returns></returns>
        void Load(string configName);

        /// <summary>
        /// 保存配置表
        /// </summary>
        void Save();

        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="id">配置项ID</param>
        /// <returns>配置项</returns>
        T GetConfig(int id);

        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="name">配置项名称</param>
        /// <returns>������</returns>
        T GetConfig(string name);

        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="id">配置项ID</param>
        /// <param name="config">配置项</param>
        /// <returns>是否存在配置项</returns>
        bool TryGetConfig(int id, out T config);

        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="name">配置项名称</param>
        /// <param name="config">配置项</param>
        /// <returns>是否存在配置项</returns>
        bool TryGetConfig(string name, out T config);

        /// <summary>
        /// 是否存在配置项
        /// </summary>
        /// <param name="id">配置项ID</param>
        /// <returns>是否存在配置项</returns>
        bool HasConfig(int id);

        /// <summary>
        /// 是否存在配置项
        /// </summary>
        /// <param name="name">配置项名称</param>
        /// <returns>是否存在配置项</returns>
        bool HasConfig(string name);

        /// <summary>
        /// 添加配置项
        /// </summary>
        /// <param name="config">配置项</param>
        void Add(T config);

        /// <summary>
        /// 移除配置项
        /// </summary>
        /// <param name="id">配置项ID</param>
        void Remove(int id);

        /// <summary>
        /// 移除配置项
        /// </summary>
        /// <param name="name">配置项名称</param>
        void Remove(string name);

        /// <summary>
        /// 移除配置项
        /// </summary>
        /// <param name="config">配置项</param>
        void Remove(T config);
    }
}
