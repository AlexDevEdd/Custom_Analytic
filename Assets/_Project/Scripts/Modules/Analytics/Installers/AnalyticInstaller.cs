using JetBrains.Annotations;
using Zenject;

namespace Analytics
{
    [UsedImplicitly]
    public sealed class AnalyticInstaller : Installer<AnalyticInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AnalyticSystem>()
                .AsSingle()
                .NonLazy();
        }
    }
}