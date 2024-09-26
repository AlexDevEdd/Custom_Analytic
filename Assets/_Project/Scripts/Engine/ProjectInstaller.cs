using Analytics;
using Zenject;

namespace Engine
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            AnalyticInstaller.Install(Container);
            SaveLoadInstaller.Install(Container);
        }
    }
}