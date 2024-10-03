using System;
using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

namespace Analytics
{
    [UsedImplicitly]
    public sealed class AnalyticsStorage
    {
        private readonly string _saveKey;
        private bool _isInProgressNow;

        public AnalyticsStorage(string saveKey)
        {
            _saveKey = saveKey;
        }

        public async UniTask Save(IEnumerable eventData)
        {
            var serializedData = JsonConvert.SerializeObject(new {events = eventData});
            
            if(!serializedData.IsNullOrEmpty())
                await WriteAsync(_saveKey, serializedData);
        }

        public async UniTask<EventsDataContainer> Load()
        {
            var jsonData = await ReadAsync(_saveKey);
            if (jsonData.IsNullOrEmpty())
                return new EventsDataContainer();
            
            var eventsData = JsonConvert.DeserializeObject<EventsDataContainer>(jsonData);
            return eventsData;
        }

        private async UniTask<string> ReadAsync(string key)
        {
            var path = BuildPath(key);
            if (!File.Exists(path))
            {
                Log.ColorLogDebugOnly($"First save for {key} haven't been yet", ColorType.Orange, LogStyle.Warning);
                return default;
            }

            try
            {
                return await File.ReadAllTextAsync(path);
            }
            catch (Exception e)
            {
                Log.ColorLog(e.ToString(), ColorType.Red, LogStyle.Error);
                return default;
            }
        }

        private async UniTask WriteAsync(string key, string data)
        {
            if(_isInProgressNow) return;
            
            var path = BuildPath(key);
            
            try
            {
                _isInProgressNow = true;
                if (File.Exists(path)) 
                    File.Delete(path);
                
                await File.WriteAllTextAsync(path, data);
                _isInProgressNow = false;
            }
            catch (Exception e)
            {
                _isInProgressNow = false;
                throw new ArgumentException($"Something went wrong : {e}");
            }
        }
        
        private string BuildPath(string key)
            => Path.Combine(Application.persistentDataPath, key);
    }
}