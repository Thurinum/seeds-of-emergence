using System.Collections.Generic;
using UnityEngine;

namespace GodBox.GameplayTags
{
    public class GameplayTagManager : MonoBehaviour
    {
        private static GameplayTagManager _instance;
        private Dictionary<GameplayTag, HashSet<GameObject>> _tagMap = new Dictionary<GameplayTag, HashSet<GameObject>>();

        public static GameplayTagManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("GameplayTagManager");
                    _instance = go.AddComponent<GameplayTagManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        public void Register(GameplayTag tag, GameObject obj)
        {
            if (tag == null || obj == null) return;

            if (!_tagMap.ContainsKey(tag))
            {
                _tagMap[tag] = new HashSet<GameObject>();
            }
            _tagMap[tag].Add(obj);
        }

        public void Unregister(GameplayTag tag, GameObject obj)
        {
            if (tag == null || obj == null) return;
            if (_tagMap.ContainsKey(tag))
            {
                _tagMap[tag].Remove(obj);
                if (_tagMap[tag].Count == 0)
                {
                    _tagMap.Remove(tag);
                }
            }
        }

        public HashSet<GameObject> GetObjectsWithTag(GameplayTag tag)
        {
            if (tag != null && _tagMap.ContainsKey(tag))
            {
                return _tagMap[tag];
            }
            return new HashSet<GameObject>();
        }
    }
}
