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
    [Serializable]
    public sealed class BundleList : IRefrence
    {
        /// <summary>
        /// 资源列表
        /// </summary>
        public List<BundleData> bundles;

        public int Count
        {
            get
            {
                return bundles.Count;
            }
        }

        public BundleData this[int index]
        {
            get
            {
                if (index < 0 || index >= bundles.Count)
                {
                    throw GameFrameworkException.Generate<IndexOutOfRangeException>();
                }
                return bundles[index];
            }
        }

        /// <summary>
        /// 资源列表构造函数
        /// </summary>
        public BundleList()
        {
            bundles = new List<BundleData>();
        }

        public List<BundleData> GetBundles()
        {
            return bundles;
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
        /// 移除指定的资源包
        /// </summary>
        /// <param name="name"></param>
        public void Remove(BundleData bundleData)
        {
            if (bundleData == null)
            {
                UnityEngine.Debug.Log("the bundle data cannot be null");
                return;
            }
            if (!bundles.Contains(bundleData))
            {
                BundleData temp = GetBundleData(bundleData.name);
                if (temp == null)
                {
                    UnityEngine.Debug.Log("the bundle data is not exsit to this bundle list:" + bundleData.name);
                }
                else
                {
                    UnityEngine.Debug.Log("the bundle data is not exsit to this bundle list:" + bundleData.name + "  " + temp.GetHashCode() + "  " + bundleData.GetHashCode());
                }
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
                Remove(bundleData.name);
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
            return bundles.Find(x => x.name.ToLower() == name.ToLower());
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
            if (string.IsNullOrEmpty(data))
            {
                return default;
            }
            BundleList bundle = CatJson.JsonParser.ParseJson<BundleList>(data); //Loader.Generate<BundleList>();
            //bundle.bundles = CatJson.JsonParser.ParseJson<List<BundleData>>(data);
            return bundle;
        }

        public void AddRange(BundleList bundleList)
        {
            if (bundleList == null)
            {
                throw GameFrameworkException.Generate<NullReferenceException>();
            }

            bundles.AddRange(bundleList.bundles);
        }
        public void AddRange(List<BundleData> bundleDatas)
        {
            if (bundleDatas == null)
            {
                throw GameFrameworkException.Generate<NullReferenceException>();
            }

            bundles.AddRange(bundleDatas);
        }
        public void Clear()
        {
            bundles.Clear();
        }

        public BundleData Last()
        {
            return bundles.LastOrDefault();
        }

        public BundleData First()
        {
            return bundles.FirstOrDefault();
        }
        public bool Contains(string name)
        {
            return bundles.Find(x => x.name == name) != null;
        }

        public bool Contains(BundleData bundle)
        {
            return bundles.Contains(bundle);
        }

        internal List<BundleData> GetBundleDatas(string moduleName)
        {
            return bundles.Where(x => x.module == moduleName).ToList();
        }
    }

    /// <summary>
    /// 资源列表
    /// </summary>
    [Serializable]
    public sealed class BundleData : IRefrence
    {
        /// <summary>
        /// 包名
        /// </summary>
        public string name;

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
        /// 是否在安装包内
        /// </summary>
        public bool IsApk;

        /// <summary>
        /// 资源包所属模块
        /// </summary>
        public string module;

        /// <summary>
        /// 资源列表
        /// </summary>
        public List<AssetData> assets;

        public AssetData this[int index]
        {
            get
            {
                if (index < 0 || index >= assets.Count)
                {
                    throw GameFrameworkException.Generate<IndexOutOfRangeException>();
                }
                return assets[index];
            }
        }

        public int Count
        {
            get
            {
                return assets.Count;
            }
        }

        public string[] Paths
        {
            get
            {
                string[] paths = new string[Count];
                for (int i = 0; i < Count; i++)
                {
                    paths[i] = this[i].path;
                }
                return paths;
            }
        }

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
                UnityEngine.Debug.Log("重复资源：" + assetData.path);
                return;
            }
            assets.Add(assetData);
        }

        public bool HasAssetData(string assetName)
        {
            return assets.AsParallel().Where(x => x.name == assetName).FirstOrDefault() != default;
        }

        public bool EqualsVersion(BundleData bundleData)
        {
            if (bundleData == null)
            {
                return false;
            }
            return bundleData.version > version;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override string ToString()
        {
            return CatJson.JsonParser.ToJson(this);
        }

        public AssetData Last()
        {
            return assets.Last();
        }

        public AssetData First()
        {
            return assets.First();
        }

        public bool Contains(string name)
        {
            return assets.Find(x => x.name == name) != null;
        }

        public bool Contains(AssetData assetData)
        {
            return assets.Contains(assetData);
        }
    }

    /// <summary>
    /// 资源数据
    /// </summary>
    [Serializable]
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

        /// <summary>
        /// 资源路径
        /// </summary>
        public string path;

        public void Release()
        {
        }
    }
}
