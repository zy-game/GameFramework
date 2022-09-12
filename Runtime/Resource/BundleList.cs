using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
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
            bundles.ForEach(Creater.Release);
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
                return;
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
            return bundles.Find(x => x.name == name);
        }

        /// <summary>
        /// 获取包含指定资源的资源包
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public BundleData GetBundleDataWithAsset(string assetName)
        {
            return bundles.Find(x => x.GetAssetData(assetName) != null);
        }

        public override string ToString()
        {
            return CatJson.JsonParser.ToJson(bundles);
        }

        public static BundleList Generate(string data)
        {
            BundleList list = Creater.Generate<BundleList>();
            list.bundles = CatJson.JsonParser.ParseJson<List<BundleData>>(data);
            return list;
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
        /// 资源地址
        /// </summary>
        public string url;

        /// <summary>
        /// 特征码
        /// </summary>
        public uint crc32;

        /// <summary>
        /// 所属对象
        /// </summary>
        public string owner;

        /// <summary>
        /// 资源列表
        /// </summary>
        public List<AssetData> assets;

        /// <summary>
        /// 回收
        /// </summary>
        public void Release()
        {
            assets.ForEach(Creater.Release);
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
            if (GetAssetData(assetData.name) != null)
            {
                return;
            }
            assets.Add(assetData);
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
