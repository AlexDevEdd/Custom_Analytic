using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Analytics
{
    [UsedImplicitly]
    public sealed class AnalyticsInstaller : MonoInstaller
    {
        [SerializeField] 
        private string _saveKey;

        [FormerlySerializedAs("_analyticSettings")] [SerializeField] 
        private AnalyticsSettings _analyticsSettings;

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
                .WithArguments(_analyticsSettings)
                .NonLazy();
        }
        
        private void BindAnalyticsClient()
        {
            Container.BindInterfacesAndSelfTo<AnalyticsClient>()
                .AsSingle()
                .WithArguments(_analyticsSettings)
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