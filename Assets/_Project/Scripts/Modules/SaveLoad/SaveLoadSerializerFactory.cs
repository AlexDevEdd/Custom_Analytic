using JetBrains.Annotations;

namespace SaveLoad
{
    [UsedImplicitly]
    public sealed class SaveLoadSerializerFactory
    {
       // private readonly AuthService _authService;
       // private readonly GameBalance _balance;
        
        // public SaveLoadSerializerFactory(AuthService authService, GameBalance balance)
        // {
        //     _balance = balance;
        //     _authService = authService;
        // }
        
        public ISerializer CreateSerializer()
        {
            return new NewtonsoftSerializer();
            
            // switch (_balance.SaveConfig.Type)
            // {
            //     case SaveConfig.SaveType.Local:
            //         return new JsonToFileSerializer();
            //     
            //     case SaveConfig.SaveType.Cloud:
            //         return _authService.IsInitialized 
            //             ? new CloudSerializer() 
            //             : new JsonToFileSerializer();
            // }
            //
            // return new JsonToFileSerializer();
        }

        public IDataStorage CreateDataStorage()
        {
            return new StreamingAssetsDataStorage();
        }
    }
}