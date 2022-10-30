using System.Threading.Tasks;
namespace GameFramework.Resource
{
    /// <summary>
    /// 资源更新管道
    /// </summary>
    public interface IResourceUpdateHandler : IRefrence
    {
        /// <summary>
        /// 设置资源读写管道
        /// </summary>
        /// <param name="resourceStreamingHandler"></param>
        void SetResourceStreamingHandler(IResourceStreamingHandler resourceStreamingHandler);

        /// <summary>
        /// 检查包内资源是否有更新
        /// </summary>
        /// <param name="resourceUpdateListenerHandler"></param>
        void CheckoutStreamingAssetListUpdate(string moduleName, IResourceUpdateListenerHandler resourceUpdateListenerHandler);

        /// <summary>
        /// 检查热更新资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="resourceUpdateListenerHandler"></param>
        void ChekeoutHotfixResourceListUpdate(string moduleName, IResourceUpdateListenerHandler resourceUpdateListenerHandler);
    }
}
