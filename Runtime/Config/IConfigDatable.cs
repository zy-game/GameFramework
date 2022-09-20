namespace GameFramework.Config
{
    /// <summary>
    /// ���ñ����
    /// </summary>
    /// <typeparam name="T">���ñ�����</typeparam>
    public interface IConfigDatable<T> : IRefrence where T : IConfig
    {
        /// <summary>
        /// ���ñ�����
        /// </summary>
        /// <value></value>
        string name { get; }
        
        /// <summary>
        /// ��ǰ�������Ŀ
        /// </summary>
        /// <value></value>
        int Count { get; }

        /// <summary>
        /// �������ñ�
        /// </summary>
        /// <param name="configName">���ñ�����</param>
        /// <returns></returns>
        void Load(string configName);

        /// <summary>
        /// �������ñ�
        /// </summary>
        void Save();

        /// <summary>
        /// ��ȡָ����������
        /// </summary>
        /// <param name="id">������ID</param>
        /// <returns>������</returns>
        T GetConfig(int id);

        /// <summary>
        /// ��ȡָ����������
        /// </summary>
        /// <param name="name">����������</param>
        /// <returns>������</returns>
        T GetConfig(string name);

        /// <summary>
        /// ���Ի�ȡһ��������
        /// </summary>
        /// <param name="id">������ID</param>
        /// <param name="config">������</param>
        /// <returns>�Ƿ����������</returns>
        bool TryGetConfig(int id, out T config);

        /// <summary>
        /// ���Ի�ȡһ��������
        /// </summary>
        /// <param name="name">����������</param>
        /// <param name="config">���ö���</param>
        /// <returns>�Ƿ����������</returns>
        bool TryGetConfig(string name, out T config);

        /// <summary>
        /// ��ѯ�Ƿ����ָ��������ID
        /// </summary>
        /// <param name="id">����ID</param>
        /// <returns>�Ƿ����ָ��������ID</returns>
        bool HasConfig(int id);

        /// <summary>
        /// ��ѯ�Ƿ����ָ������������
        /// </summary>
        /// <param name="name">��������</param>
        /// <returns>�Ƿ����ָ������������</returns>
        bool HasConfig(string name);
        /// <summary>
        /// ���һ��������
        /// </summary>
        /// <param name="config">������</param>
        void Add(T config);

        /// <summary>
        /// �Ƴ�һ��������
        /// </summary>
        /// <param name="id">������ID</param>
        void Remove(int id);

        /// <summary>
        /// �Ƴ�һ��������
        /// </summary>
        /// <param name="name">����������</param>
        void Remove(string name);

        /// <summary>
        /// �Ƴ�һ��������
        /// </summary>
        /// <param name="config">������</param>
        void Remove(T config);
    }
}
