using UnityEngine;

namespace Analytics
{
    [CreateAssetMenu(fileName = "SaveLoadSettings", menuName = "Configs/SaveLoadSettings")]
    public sealed class SaveLoadSettings : ScriptableObject
    {
        [field:SerializeField]
        public string SaveKey { get; private set; }
    }
}