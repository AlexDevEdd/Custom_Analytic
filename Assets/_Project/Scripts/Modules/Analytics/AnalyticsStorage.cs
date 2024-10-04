using System;
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
        
        public AnalyticsStorage(string saveKey)
        {
            _saveKey = saveKey;
        }

        public async UniTask SaveAsync(CircularArrayQueue<EventData> eventData)
        {
            var arraySegment = eventData.Tail > 0 
                ? new ArraySegment<EventData>(eventData.Items, eventData.Head, eventData.Tail)
                : new ArraySegment<EventData>(eventData.Items, 0, 0);
            
            var serializedData = JsonConvert.SerializeObject(arraySegment);
            if(!serializedData.IsNullOrEmpty())
                await WriteAsync(_saveKey, serializedData);
        }

        public async UniTask<CircularArrayQueue<EventData>> LoadAsync()
        {
            var jsonData = await ReadAsync(_saveKey);
            if (jsonData.IsNullOrEmpty())
                return new CircularArrayQueue<EventData>();
            
            var eventsData = JsonConvert.DeserializeObject<EventData[]>(jsonData);
            var data = new CircularArrayQueue<EventData>();
            for (var i = 0; i < eventsData.Length; i++)
            {
                data.Enqueue(new EventData(eventsData[i].Type, eventsData[i].Data));
            }
            return data;
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
            var path = BuildPath(key);
            
            try
            {
                if (File.Exists(path)) 
                    File.Delete(path);
                
                await File.WriteAllTextAsync(path, data);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Something went wrong : {e}");
            }
        }
        
        private string BuildPath(string key)
            => Path.Combine(Application.persistentDataPath, key);
    }
}