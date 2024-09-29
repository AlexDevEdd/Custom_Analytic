using System;
using System.Collections.Generic;
using Analytics.Data;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Utils;

namespace Analytics
{
    [UsedImplicitly]
    public class AnalyticEventService : IDisposable
    {
        private readonly AnalyticEventsData _eventsToSend = new ();
        private Queue<EventData> _unsentEvents;
        private ISerializer _serializer;
        
        private bool _isSending;
        private string _serverUrl;
        private float _cooldownBeforeSend;
        
        private IDisposable _cooldownDisposable;

        public bool IsAvailable { get; }
        public IReadOnlyCollection<EventData> UnsentEvents => _unsentEvents;
        
        public AnalyticEventService(AnalyticSettings settings)
        {
            IsAvailable = settings.IsEnable && !settings.ServerUrl.IsNullOrEmpty();

            if (!IsAvailable)
            {
                Log.ColorLogDebugOnly($"Analytic settings isAvailable: {settings.IsEnable} or ServerUrl null or empty",
                    ColorType.Red, LogStyle.Warning); 
                return;
            }
            
            Init(settings);
        }
        
        public void TrackEvent(string type, string data)
        {
            if (!IsAvailable)
                return;
            
            _unsentEvents.Enqueue(new EventData(type, data));
        }
        
        private void Init(AnalyticSettings settings)
        {
            _serializer = new NewtonsoftSerializer();
            _unsentEvents = new Queue<EventData>(settings.BufferSize);
            _cooldownBeforeSend = settings.CooldownBeforeSend;
            _serverUrl = settings.ServerUrl;
            
            _cooldownDisposable = Observable.Interval(TimeSpan.FromSeconds(_cooldownBeforeSend))
                .Subscribe(x =>
                {
                    if (!IsHasConnect())
                    {
                        Log.ColorLogDebugOnly("No Connect", ColorType.Brown, LogStyle.Warning); 
                        return;
                    }
            
                    if (_unsentEvents.Count > 0 && !_isSending) 
                        TrySendEventsAsync().Forget();
                });
        }
        
        private async UniTask SendPostRequestAsync()
        {
            if (_serializer.TrySerialize(_eventsToSend, out var message))
            {
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
            _eventsToSend.EventsDatas.AddRange(_unsentEvents);
            _unsentEvents.Clear();
        }

        private void FillEventsFromCache()
        {
            foreach (var eventsData in _eventsToSend.EventsDatas)
            {
                _unsentEvents.Enqueue(new EventData(eventsData.Type, eventsData.Data));
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