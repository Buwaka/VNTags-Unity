#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VNTags.Utility
{
    public delegate void AssetChanged();

    public class AssetWatcher : AssetPostprocessor
    {
        private static          string[]                         _allAssets;
        private static readonly Dictionary<string, AssetChanged> _assetEventLibrary = new();


        private static void OnPostprocessAllAssets(string[] importedAssets,
                                                   string[] deletedAssets,
                                                   string[] movedAssets,
                                                   string[] movedFromAssetPaths)
        {
            if (_allAssets == null)
            {
                return;
            }

            string[] created  = importedAssets.Except(_allAssets).ToArray();
            string[] modified = importedAssets.Except(created).ToArray();

            foreach (string asset in modified)
            {
                if (_assetEventLibrary.ContainsKey(asset))
                {
                    _assetEventLibrary[asset].Invoke();
                }
            }
        }

        public static void WatchAsset(Object asset, AssetChanged action)
        {
            if (_allAssets == null)
            {
                _allAssets = AssetDatabase.GetAllAssetPaths();
            }

            string path = AssetDatabase.GetAssetPath(asset);

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("AssetWatcher: WatchAsset: asset not found, " + asset.name);
                return;
            }

            if (!_assetEventLibrary.ContainsKey(path))
            {
                _assetEventLibrary.Add(path, null);
            }

            _assetEventLibrary[path] += action;
        }

        public static void UnwatchAsset(Object asset, AssetChanged action)
        {
            string path = AssetDatabase.GetAssetPath(asset);

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("AssetWatcher: WatchAsset: asset not found, " + asset.name);
                return;
            }

            if (_assetEventLibrary.ContainsKey(path))
            {
                _assetEventLibrary[path] -= action;
            }
        }
    }
}
#endif