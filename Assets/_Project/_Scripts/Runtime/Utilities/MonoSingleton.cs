using UnityEngine;

namespace PingPingProduction.ProjectAnomaly.Utilities {
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
        private static readonly object _lock = new();

        public static bool IsInitialize => _instance != null;

        private static T _instance = default;

        public static T Instance {
            get {
                if (IsInitialize) return _instance;

                lock (_lock) {
                    _instance = FindFirstObjectByType<T>();
                    if (IsInitialize)
                        return _instance;

                    var go = new GameObject($"{typeof(T).Name} Instance");
                    _instance = go.AddComponent<T>();
                    return _instance;
                }
            }
        }

        [SerializeField] protected bool IsPersistent = true;

        protected virtual void Awake() {
            if (IsInitialize && _instance != this) {
                Destroy(this);
                return;
            }

            _instance = this as T;

            if (IsPersistent)
                DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy() {
            _instance = null;
        }
    }
}