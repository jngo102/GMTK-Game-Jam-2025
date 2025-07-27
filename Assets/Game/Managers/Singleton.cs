using UnityEngine;

namespace Game.Managers
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static readonly object lockObj = new();
        
        private static T instance;
        public static T Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (instance)
                    {
                        return instance;   
                    }

                    instance = FindFirstObjectByType<T>();
                    if (!instance)
                    {
                        instance = new GameObject(typeof(T).FullName, typeof(T)).GetComponent<T>();
                    }
                    DontDestroyOnLoad(instance);
                    return instance;
                }
            }
        }

        public void OnDestroy()
        {
            lock (lockObj)
            {
                instance = null;
            }
        }
    }
}