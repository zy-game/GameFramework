using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
using GameFramework.Config;
using System.Linq;
namespace GameFramework.Resource
{
    /// <summary>
    /// 资源列表
    /// </summary>
    public sealed class BundleList : IRefrence
    {
        /// <summary>
        /// 资源列表
        /// </summary>
        public List<BundleData> bundles;

        /// <summary>
        /// 资源列表构造函数
        /// </summary>
        public BundleList()
        {
            bundles = new List<BundleData>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public void Release()
        {
            bundles.ForEach(Loader.Release);
        }

        /// <summary>
        /// 移除指定的资源包
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            BundleData bundleData = GetBundleData(name);
            if (bundleData == null)
            {
                return;
            }
            bundles.Remove(bundleData);
        }

        /// <summary>
        /// 添加资源包
        /// </summary>
        /// <param name="bundleData">资源包</param>
        public void Add(BundleData bundleData)
        {
            if (GetBundleData(bundleData.name) != null)
            {
                throw GameFrameworkException.Generate("the bundle is already exsit");
            }
            bundles.Add(bundleData);
        }

        /// <summary>
        /// 获取资源包
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BundleData GetBundleData(string name)
        {
            return bundles.AsParallel().Where(x => x.name == name).FirstOrDefault();
        }

        /// <summary>
        /// 获取包含指定资源的资源包
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public BundleData GetBundleDataWithAsset(string assetName)
        {
            return bundles.AsParallel().Where(x => x.HasAssetData(assetName)).FirstOrDefault();
        }

        public override string ToString()
        {
            return CatJson.JsonParser.ToJson(this);
        }

        public static BundleList Generate(string data)
        {
            return CatJson.JsonParser.ParseJson<BundleList>(data);
        }
    }

    /// <summary>
    /// 资源列表
    /// </summary>
    public sealed class BundleData : IRefrence
    {
        /// <summary>
        /// 包名
        /// </summary>
        public string name;

        /// <summary>
        /// 特征码
        /// </summary>
        public uint crc32;

        /// <summary>
        /// 资源版本
        /// </summary>
        public uint version;

        /// <summary>
        /// 所属对象
        /// </summary>
        public string owner;

        /// <summary>
        /// 打包时间
        /// </summary>
        public long time;

        /// <summary>
        /// 资源列表
        /// </summary>
        public List<AssetData> assets;

        public BundleData()
        {
            assets = new List<AssetData>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public void Release()
        {
            assets.ForEach(Loader.Release);
            assets.Clear();
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AssetData GetAssetData(string name)
        {
            return assets.Find(x => x.name == name);
        }

        /// <summary>
        /// 移除资源
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            AssetData assetData = GetAssetData(name);
            if (assetData == null)
            {
                return;
            }
            assets.Remove(assetData);
        }

        /// <summary>
        /// 添加资源
        /// </summary>
        /// <param name="assetData"></param>
        public void Add(AssetData assetData)
        {
            if (HasAssetData(assetData.name))
            {
                return;
            }
            assets.Add(assetData);
        }

        public bool HasAssetData(string assetName)
        {
            return assets.AsParallel().Where(x => x.name == assetName).FirstOrDefault() != default;
        }
    }

    /// <summary>
    /// 资源数据
    /// </summary>
    public sealed class AssetData : IRefrence
    {
        /// <summary>
        /// 资源名
        /// </summary>
        public string name;

        /// <summary>
        /// 资源唯一ID
        /// </summary>
        public string guid;

        public void Release()
        {
        }
    }
}
