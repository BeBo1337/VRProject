using UnityEngine;
using UnityEngine.Pool;

namespace Framework
{
    public class Spawner<T> : MonoBehaviour where T: MonoBehaviour
    {
        [SerializeField] private T _prefab;
        [SerializeField] private int _initialPoolSize;
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private int _maxPoolSize;
    
        private IObjectPool<T> _pool;
        public static Spawner<T> BaseInstance { get; private set; }

        protected virtual void Awake()
        {
            if (BaseInstance == null)
            {
                BaseInstance = this;
                DontDestroyOnLoad(gameObject);
                
                CreatePools();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void CreatePools()
        {
            _pool = new ObjectPool<T>(CreationFunction, OnObjectGet, OnObjectRelease, OnObjectDestroy,
                defaultCapacity: _initialPoolSize,
                maxSize: _maxPoolSize);
        }

        protected virtual T CreationFunction()
        {
            var newObject = Instantiate(_prefab, _parentTransform.position, Quaternion.identity, _parentTransform);
            newObject.gameObject.SetActive(false);
            return newObject;
        }
    
        protected virtual void OnObjectGet(T obj)
        {
            obj.gameObject.SetActive(true);
        }
    
        protected virtual void OnObjectRelease(T obj)
        {
            obj.gameObject.SetActive(false);
        }
    
        protected virtual void OnObjectDestroy(T obj)
        {
            Destroy(obj.gameObject);
        }

        public virtual T Spawn()
        {
            var newObject = _pool.Get();
            return newObject;
        }
        
        public virtual void Release(T obj)
        {
            _pool.Release(obj);
        }
    }
}