using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Analytics
{
    [UsedImplicitly]
    public sealed class AnalyticEventsSaveLoadInstaller : MonoInstaller
    {
        [SerializeField] 
        private SaveLoadSettings _saveLoadSettings;
        
        public override void InstallBindings()
        {
            BindSaveSystem();
        }

        private void BindSaveSystem()
        {
            Container.BindInterfacesAndSelfTo<SaveLoadAnalyticEventsService>()
                .AsSingle()
                .WithArguments(_saveLoadSettings)
                .NonLazy();
        }
    }
}