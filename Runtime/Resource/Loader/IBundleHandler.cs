using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace GameFramework.Resource
{
    public interface IBundleHandler : IRefrence
    {
        string name { get; }
        ResHandle LoadAsset(AssetData assetData);
        Task<ResHandle> LoadAssetAsync(AssetData assetData);
        bool CanUnload();
        void SubRefrence();
        void AddRefrence();
    }
}
