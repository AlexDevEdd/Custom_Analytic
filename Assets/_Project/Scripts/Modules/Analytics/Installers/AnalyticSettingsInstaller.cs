using UnityEngine;
using Zenject;

namespace Analytics
{
    [CreateAssetMenu(fileName = "AnalyticSettingsInstaller", menuName = "Installers/AnalyticSettingsInstaller")]
    public sealed class AnalyticSettingsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private AnalyticSettings _analyticSettings;
        
        public override void InstallBindings()
        {
            Container.Bind<AnalyticSettings>()
                .FromInstance(_analyticSettings)
                .AsSingle()
                .NonLazy();
        }
    }
}