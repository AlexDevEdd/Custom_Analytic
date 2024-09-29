using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Analytics
{
    [UsedImplicitly]
    public sealed class AnalyticInstaller : MonoInstaller
    {
        [SerializeField] 
        private AnalyticSettings _analyticSettings;
        
        public override void InstallBindings()
        {
            BindAnalyticSystem();
        }
        
        private void BindAnalyticSystem()
        {
            Container.BindInterfacesAndSelfTo<AnalyticEventService>()
                .AsSingle()
                .WithArguments(_analyticSettings)
                .NonLazy();
        }
    }
}