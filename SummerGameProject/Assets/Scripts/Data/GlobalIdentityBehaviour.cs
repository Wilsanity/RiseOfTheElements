using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Kibo.Data
{
    public abstract class GlobalIdentityBehaviour<GlobalIdentityT, DataT> : MonoBehaviour where GlobalIdentityT : GlobalIdentityBehaviour<GlobalIdentityT, DataT> where DataT : IGlobalIdentityData
    {
        public static UnityEvent<GlobalIdentityT> Spawned { get; } = new();
        public static UnityEvent<GlobalIdentityT> Destroyed { get; } = new();

        private readonly static Dictionary<string, GlobalIdentityT> loadedIdentities = new();

        public string GUID => guid;

        [SerializeField, HideInInspector] private string guid;

        #region Unity Messages
        protected virtual void Awake()
        {
            Assert.IsTrue(!string.IsNullOrEmpty(GUID), $"No {nameof(GUID)} given to {name}");
        }

        protected virtual void Start()
        {
            if (!loadedIdentities.ContainsKey(GUID))
            {
                loadedIdentities.Add(GUID, (GlobalIdentityT)this);
            }
            else
            {
#if LOG
                Debug.Log($"Multiple {nameof(GlobalIdentityBehaviour<GlobalIdentityT, DataT>)}s loaded with GUID '{GUID}'");
#endif
                Destroy(gameObject);
                return;
            }

            Spawned.Invoke(this as GlobalIdentityT);
        }

        protected virtual void OnDestroy()
        {
            if (loadedIdentities.TryGetValue(GUID, out GlobalIdentityT globalIdentity) && globalIdentity == this)
            {
                loadedIdentities.Remove(GUID);
            }
            else return;

            Destroyed.Invoke(this as GlobalIdentityT);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (GUID == null) GenerateGUID();
        }
#endif
        #endregion

#if UNITY_EDITOR
        #region Editor
        public void GenerateGUID()
        {
            guid = Guid.NewGuid().ToString();
        }
        #endregion
#endif

        #region File IO
        public abstract void SaveTo(ref DataT globalIdentityType);

        public abstract void LoadFrom(DataT globalIdentityType);
        #endregion

        #region Identity Access
        public static GlobalIdentityT Get(string guid)
        {
            return loadedIdentities[guid];
        }

        public static IEnumerable<GlobalIdentityT> GetAll()
        {
            return loadedIdentities.Values;
        }

        public static IEnumerable<GlobalIdentityT> GetAll(Func<GlobalIdentityT, bool> predicate)
        {
            return loadedIdentities.Values.Where(predicate);
        }
        #endregion
    }
}