using System.IO;
using System.Collections.Generic;
using System.Text;

namespace GameFramework.Config
{
    /// <summary>
    /// Ĭ�����ñ����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    sealed class DefaultConfigDatable<T> : IConfigDatable<T> where T : IConfig
    {

        private List<T> configs = new List<T>();

        /// <summary>
        /// ���ñ�����
        /// </summary>
        /// <value></value>
        public string name
        {
            get;
            private set;
        }

        /// <summary>
        /// ��ǰ�������Ŀ
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
        /// ���һ��������
        /// </summary>
        /// <param name="config">������</param>
        public void Add(T config)
        {
            if (HasConfig(config.id) || HasConfig(config.name))
            {
                throw GameFrameworkException.GenerateFormat("the config is already exsit id:{0} name:{1}", config.id, config.name);
            }
            configs.Add(config);
        }

        /// <summary>
        /// ��ȡָ����������
        /// </summary>
        /// <param name="id">������ID</param>
        /// <returns>������</returns>
        public T GetConfig(int id) => configs.Find(x => x.id == id);

        /// <summary>
        /// ��ȡָ����������
        /// </summary>
        /// <param name="name">����������</param>
        /// <returns>������</returns>
        public T GetConfig(string name) => configs.Find(x => x.name == name);

        /// <summary>
        /// ��ѯ�Ƿ����ָ��������ID
        /// </summary>
        /// <param name="id">����ID</param>
        /// <returns>�Ƿ����ָ��������ID</returns>
        public bool HasConfig(int id) => GetConfig(id) != null;

        /// <summary>
        /// ��ѯ�Ƿ����ָ������������
        /// </summary>
        /// <param name="name">��������</param>
        /// <returns>�Ƿ����ָ������������</returns>
        public bool HasConfig(string name) => GetConfig(name) != null;

        /// <summary>
        /// �������ñ�
        /// </summary>
        /// <param name="configName">���ñ�����</param>
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
        /// �������ñ�
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
        /// �������ñ�
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
        /// �Ƴ�һ��������
        /// </summary>
        /// <param name="id">������ID</param>
        public void Remove(int id) => Remove(GetConfig(id));

        /// <summary>
        /// �Ƴ�һ��������
        /// </summary>
        /// <param name="name">����������</param>
        public void Remove(string name) => Remove(GetConfig(name));

        /// <summary>
        /// �Ƴ�һ��������
        /// </summary>
        /// <param name="config">������</param>
        public void Remove(T config)
        {
            if (config == null)
            {
                return;
            }
            configs.Remove(config);
        }

        /// <summary>
        /// ���Ի�ȡһ��������
        /// </summary>
        /// <param name="id">������ID</param>
        /// <param name="config">������</param>
        /// <returns>�Ƿ����������</returns>
        public bool TryGetConfig(int id, out T config)
        {
            config = GetConfig(id);
            return config != null;
        }

        /// <summary>
        /// ���Ի�ȡһ��������
        /// </summary>
        /// <param name="name">����������</param>
        /// <param name="config">���ö���</param>
        /// <returns>�Ƿ����������</returns>
        public bool TryGetConfig(string name, out T config)
        {
            config = GetConfig(name);
            return config != null;
        }
    }
}
