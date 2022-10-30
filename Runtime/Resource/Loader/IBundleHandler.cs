using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace GameFramework.Resource
{
    public interface IBundleHandler : IRefrence
    {
        string name { get; }
        ResHandle LoadAsset<T>(AssetData assetData) where T : UnityEngine.Object;
        Task<ResHandle> LoadAssetAsync<T>(AssetData assetData) where T : UnityEngine.Object;
        bool CanUnload();
        void SubRefrence();
        void AddRefrence();
    }
}
