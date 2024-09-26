using UnityEngine;
using Zenject;

namespace SaveLoad
{
    [CreateAssetMenu(fileName = "SaveLoadSettingsInstaller", menuName = "Installers/SaveLoadSettingsInstaller")]
    public sealed class SaveLoadSettingsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private SaveLoadSettings _saveLoadSettings;
        
        public override void InstallBindings()
        {
            Container.Bind<SaveLoadSettings>()
                .FromInstance(_saveLoadSettings)
                .AsSingle()
                .NonLazy();
        }
    }
}