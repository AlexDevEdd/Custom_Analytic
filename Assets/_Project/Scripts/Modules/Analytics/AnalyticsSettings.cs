using UnityEngine;

namespace Analytics
{
    [CreateAssetMenu(fileName = "AnalyticSettings", menuName = "Configs/AnalyticSettings")]
    public sealed class AnalyticsSettings : ScriptableObject
    {
        [field: SerializeField]
        public bool IsEnable { get; private set; }
        
        [field:SerializeField]
        public string ServerUrl { get; private set; }
        
        [field:SerializeField]
        public int BufferSize { get; private set; }
        
        [field:SerializeField]
        public float PingInterval { get; private set; }
        
        [field:SerializeField]
        public float CooldownBeforeSend { get; private set; }
        
        [field:SerializeField]
        public int RequestTimeout { get; private set; }
    }
}