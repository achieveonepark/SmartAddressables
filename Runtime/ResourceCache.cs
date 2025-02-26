using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Achieve.SmartAddressables
{
    public struct ResourceCache
    {
        /// <summary>
        /// Label for cached resources.
        /// </summary>
        public string Label;

        internal AsyncOperationHandle<IList<Object>> cachedObjects;
        internal Dictionary<string, int> orderDictionary;

        public ResourceCache(string label, AsyncOperationHandle<IList<Object>> objects, Dictionary<string, int> order)
        {
            Label = label;
            cachedObjects = objects;
            orderDictionary = order;
        }

        /// <summary>
        /// Retrieves the resource allocated in memory.
        /// Since this is not an instantiated runtime object, Instantiate should be called to create an instance.
        /// </summary>
        /// <param name="address">Address defined in the Inspector</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetObject<T>(string address) where T : Object
        {
            if (orderDictionary.TryGetValue(address, out var index))
            {
                var obj = cachedObjects.Result[index] as GameObject;

                // if (Application.isEditor)
                // {
                //     // TODO : 이거 FindShader 처리해야 함
                //     SmartAddressables.FindShader((GameObject)obj);
                // }
                
                return obj.TryGetComponent<T>(out var component) ? component : default(T);
            }
            
            return null;
        }

        public GameObject GetGameObject(string address)
        {
            if (orderDictionary.TryGetValue(address, out var index))
            {
                return cachedObjects.Result[index] as GameObject;
            }

            return null;
        }

        /// <summary>
        /// Instantiates the resource allocated in memory as a runtime object.
        /// </summary>
        /// <param name="address">Address defined in the Inspector</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(string address) where T : Object
        {
            return Object.Instantiate(GetObject<T>(address));
        }

        /// <summary>
        /// Retrieves the resource allocated in memory.
        /// Since this is not an instantiated runtime object, Instantiate should be called to create an instance.
        /// </summary>
        /// <param name="assetRef">Object assigned in the Inspector</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async UniTask<T> GetObject<T>(AssetReference assetRef) where T : Object
        {
            var loadHandle = Addressables.LoadAssetAsync<T>(SmartAddressables.GetAssetReferenceKey(assetRef));
            await loadHandle.Task;

            if (loadHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load asset: {SmartAddressables.GetAssetReferenceKey(assetRef)}");
            }
            
            return loadHandle.Result;
        }

        /// <summary>
        /// Releases all resources cached in memory.
        /// </summary>
        public void Release()
        {
            foreach (var entry in cachedObjects.Result)
            {
                SmartAddressables.Release((GameObject)entry);
            }
        }

        /// <summary>
        /// Lists all resources allocated in memory.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join(", ", cachedObjects.Result);
        }
    }
}
