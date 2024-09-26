using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Utils;

namespace Analytics
{
    public class CustomAnalyticService : IDisposable
    {
        private readonly Queue<EventData> _eventsQueue = new ();
        private readonly AnalyticEventsData _eventsToSend = new ();
        
        private bool _isSending;
        private string _serverUrl;
        private float _cooldownBeforeSend;
        
        private IDisposable _cooldownDisposable;
        
        public IReadOnlyCollection<EventData> EventsQueue => _eventsQueue;

        public void Init(AnalyticSettings settings)
        {
            _serverUrl = settings.ServerUrl;
            _cooldownBeforeSend = settings.CooldownBeforeSend;
            
            _cooldownDisposable = Observable.Interval(TimeSpan.FromSeconds(_cooldownBeforeSend))
                .Subscribe(x =>
                {
                    if (!IsHasConnect())
                    {
                        Log.ColorLogDebugOnly("No Connect", ColorType.Brown, LogStyle.Warning); 
                        return;
                    }
            
                    if (_eventsQueue.Count > 0 && !_isSending) 
                        TrySendEventsAsync().Forget();
                });
        }
        
        public void TrackEvent(string type, string data)
        {
            _eventsQueue.Enqueue(new EventData(type, data));
        }

        private async UniTask SendPostRequestAsync()
        {
            var message = JsonConvert.SerializeObject(_eventsToSend);
            
            using var www = UnityWebRequest.Post(_serverUrl, message);
            await www.SendWebRequest();
                
            if (www.result == UnityWebRequest.Result.Success && www.responseCode == 200)
            {
                _eventsToSend.EventsDatas.Clear();
                _isSending = false;
                Log.ColorLogDebugOnly($"Success POST. Status code: {www.responseCode}", ColorType.Lime); 
            }
            else
            {
                FillEventsFromCache();
                _isSending = false;
                Log.ColorLogDebugOnly($"Failed to send events. Status code: {www.responseCode}", ColorType.Orange, LogStyle.Warning);
            }
        }

        private async UniTaskVoid TrySendEventsAsync()
        {
            try
            {
                _isSending = true;
                MoveEventsToSender();
                await SendPostRequestAsync();
            }
            catch (Exception ex)
            {
                FillEventsFromCache();
                _isSending = false;
                Log.ColorLogDebugOnly($"Error sending events: {ex.Message}", ColorType.Purple, LogStyle.Warning);
            }
        }
        
        private void MoveEventsToSender()
        {
            _eventsToSend.EventsDatas.AddRange(_eventsQueue);
            _eventsQueue.Clear();
        }

        private void FillEventsFromCache()
        {
            foreach (var eventsData in _eventsToSend.EventsDatas)
            {
                _eventsQueue.Enqueue(new EventData(eventsData.Type, eventsData.Data));
            } 
            
            _eventsToSend.EventsDatas.Clear();
        }

        private bool IsHasConnect()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        public void Dispose()
        {
            _cooldownDisposable?.Dispose();
        }
    }
}