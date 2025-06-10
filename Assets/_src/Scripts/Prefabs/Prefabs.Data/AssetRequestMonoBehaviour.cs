﻿using System;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _src.Scripts.Prefabs.Prefabs.Data
{
    public class AssetRequestMonoBehaviour : MonoBehaviour
    {
        public static AssetRequestMonoBehaviour Instance;

        [SerializeField] private sbyte mainCharacterIndex;
        [SerializeField] private AssetReferenceGameObjectSoGroup bodies;
        [SerializeField] private List<IndexGameObjectMap> caches;
        private bool _pending;

        private void OnEnable()
        {
            Instance = this;
            _pending = false;
        }

        public bool TryRequest(sbyte index, in LocalToWorld localToWorld, out GameObject obj)
        {
            if (index < 0) index = mainCharacterIndex;
            if (TryGetFromCache(index, out obj)) return true;
            return TryInstantiateNew(index, localToWorld);
        }

        private bool TryInstantiateNew(sbyte index, LocalToWorld localToWorld)
        {
            if (_pending) return false;
            _pending = true;
            var assetReferenceGameObject = bodies.assets[mainCharacterIndex].assetReferenceGameObject;
            var asyncOperationHandle = Addressables.InstantiateAsync(
                assetReferenceGameObject,
                localToWorld.Position,
                localToWorld.Rotation
            );
            asyncOperationHandle.Completed += handle => AsyncOperationHandleOnCompleted(index, handle);
            return asyncOperationHandle.IsDone;
        }

        private void AsyncOperationHandleOnCompleted(sbyte index, AsyncOperationHandle<GameObject> obj)
        {
            caches.Add(new IndexGameObjectMap()
            {
                index = index,
                gameObject = obj.Result,
                inUse = false
            });
            _pending = false;
        }

        private void OnDisable()
        {
            foreach (var cache in caches) Addressables.ReleaseInstance(cache.gameObject);
        }


        private bool TryGetFromCache(sbyte index, out GameObject obj)
        {
            obj = null;
            foreach (var indexGameObjectMap in caches)
            {
                if (indexGameObjectMap.index == index && indexGameObjectMap.inUse == false)
                {
                    obj = indexGameObjectMap.gameObject;
                    indexGameObjectMap.inUse = true;
                    return true;
                }
            }

            return false;
        }

        [Serializable]
        private class IndexGameObjectMap
        {
            public bool inUse;
            public sbyte index;
            public GameObject gameObject;
        }
    }
}