using Unity.Cinemachine;
using UnityEngine;

namespace _src.Scripts.CinemachineLink.CinemachineLink.Data
{
    [RequireComponent(typeof(CinemachineCamera))]
    [DisallowMultipleComponent]
    public class CinemachineLinkerSingleton : MonoBehaviour
    {
        private static CinemachineLinkerSingleton _singleton;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        private Transform cinemachineTargetTransform;

        public static Transform Transform => _singleton.cinemachineTargetTransform;

        private void Awake()
        {
            cinemachineTargetTransform ??= new GameObject(nameof(cinemachineTargetTransform)).transform;
            cinemachineCamera.Follow = cinemachineTargetTransform;
            _singleton = this;
        }

        private void Reset() => cinemachineCamera = GetComponent<CinemachineCamera>();
    }
}