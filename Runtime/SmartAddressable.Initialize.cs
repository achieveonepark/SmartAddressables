using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Achieve.SmartAddressables
{
    public static partial class SmartAddressables
    {
        private static bool _isInitialized;

        private static async UniTask<IResourceLocator> InitializeAsyncInternal()
        {
            if (_isInitialized)
            {
                return null;
            }
            
            var result = await Addressables.InitializeAsync();  
            _isInitialized = true;
            Debug.Log("[SmartAddressables] Initialized");
            return result;
        }

        private static async UniTask<IResourceLocator> InitializeAsyncInternal(string catalogPath)
        {
            if (_isInitialized)
            {
                return null;
            }

            await Addressables.InitializeAsync();
            var loadTask = Addressables.LoadContentCatalogAsync(catalogPath);  
            await loadTask.Task;

            if (loadTask.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Failed to load content catalog: {catalogPath}");
            }
            
            _isInitialized = true;
            Debug.Log($"Percent Asset initialized successfully. Catalog path: {catalogPath}");
            return loadTask.Result;
        }

        private static void ValidateInitialize()
        {
            if (_isInitialized is false)
            {
                Debug.LogError("PercentAsset module is not initialized. Please call await PAsset.Initialize() first.");
            }
        }
    }
}
