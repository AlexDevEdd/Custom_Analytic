using JetBrains.Annotations;
using SaveLoad;
using Zenject;

namespace Engine
{
    [UsedImplicitly]
    public sealed class SaveLoadInstaller : Installer<SaveLoadInstaller>
    {
        public override void InstallBindings()
        {
            BindSaveSystem();
            BindGameRepository();
            BindSaveLoadSerializerFactory();
            BindAnalyticsEventsSaveLoader();
        }

        private void BindSaveSystem()
        {
            Container.BindInterfacesAndSelfTo<SaveLoadSystem>()
                .AsSingle()
                .NonLazy();
        }

        private void BindGameRepository()
        {
            Container.Bind<GameDataStorage>()
                .AsSingle()
                .NonLazy();
        }
        private void BindSaveLoadSerializerFactory()
        {
            Container.Bind<SaveLoadSerializerFactory>()
                .AsSingle()
                .NonLazy();
        }
        
        private void BindAnalyticsEventsSaveLoader()
        {
            Container.BindInterfacesAndSelfTo<AnalyticsEventsSaveLoader>()
                .AsSingle()
                .NonLazy();
        }
    }
}