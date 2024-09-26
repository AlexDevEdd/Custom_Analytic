using UnityEngine;

namespace SaveLoad
{
    [CreateAssetMenu(fileName = "SaveLoadSettings", menuName = "Configs/SaveLoadSettings")]
    public sealed class SaveLoadSettings : ScriptableObject
    {
        [field:SerializeField]
        public string SaveKey { get; private set; }

        public void SetKey(string key)
        {
            SaveKey = key;
        }
    }
}