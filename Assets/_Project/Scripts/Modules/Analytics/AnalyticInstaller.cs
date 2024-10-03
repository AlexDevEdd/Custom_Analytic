using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Analytics
{
    [UsedImplicitly]
    public sealed class AnalyticInstaller : MonoInstaller
    {
        [SerializeField] 
        private string _saveKey;

        [SerializeField] 
        private AnalyticSettings _analyticSettings;

        public override void InstallBindings()
        {
            BindAnalyticsRepository();
            BindAnalyticsClient();
            BindAnalyticsStorage();
        }
        
        private void BindAnalyticsRepository()
        {
            Container.BindInterfacesAndSelfTo<AnalyticsRepository>()
                .AsSingle()
                .WithArguments(_analyticSettings)
                .NonLazy();
        }
        
        private void BindAnalyticsClient()
        {
            Container.BindInterfacesAndSelfTo<AnalyticsClient>()
                .AsSingle()
                .WithArguments(_analyticSettings)
                .NonLazy();
        }
        
        private void BindAnalyticsStorage()
        {
            Container.BindInterfacesAndSelfTo<AnalyticsStorage>()
                .AsSingle()
                .WithArguments(_saveKey)
                .NonLazy();
        }
    }
}