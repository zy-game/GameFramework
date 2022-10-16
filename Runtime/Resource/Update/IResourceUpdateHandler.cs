using System.Threading.Tasks;
namespace GameFramework.Resource
{
    /// <summary>
    /// ��Դ���¹ܵ�
    /// </summary>
    public interface IResourceUpdateHandler : IRefrence
    {
        /// <summary>
        /// ������Դ��д�ܵ�
        /// </summary>
        /// <param name="resourceStreamingHandler"></param>
        void SetResourceStreamingHandler(IResourceStreamingHandler resourceStreamingHandler);

        /// <summary>
        /// ��������Դ�Ƿ��и���
        /// </summary>
        /// <param name="resourceUpdateListenerHandler"></param>
        void CheckoutStreamingAssetListUpdate(IResourceUpdateListenerHandler resourceUpdateListenerHandler);

        /// <summary>
        /// ����ȸ�����Դ
        /// </summary>
        /// <param name="url"></param>
        /// <param name="resourceUpdateListenerHandler"></param>
        void ChekeoutHotfixResourceListUpdate(string url, IResourceUpdateListenerHandler resourceUpdateListenerHandler);
    }
}
