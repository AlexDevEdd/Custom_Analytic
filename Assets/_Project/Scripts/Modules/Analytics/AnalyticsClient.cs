using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UniRx;
using UnityEngine.Networking;
using Utils;

namespace Analytics
{
    [UsedImplicitly]
    public sealed class AnalyticsClient : IDisposable
    {
        private readonly string _serverUrl;
        private readonly bool _isAvailable;
        private readonly float _pingInterval;
        private readonly int _requestTimeout;
        private bool _isConnected;
        
        private CompositeDisposable _disposable;
        
        public AnalyticsClient(AnalyticsSettings settings)
        {
            _isAvailable = settings.IsEnable && !settings.ServerUrl.IsNullOrEmpty();
            
            _serverUrl = settings.ServerUrl;
            _pingInterval = settings.PingInterval;
            _requestTimeout = settings.RequestTimeout;
        }
        
        public async UniTask Init()
        {
            if (_isAvailable)
            {
                await PerformPingAsync();
                InitializePingCheck();
            }
        }

        public async UniTask<bool> TrySendEventsAsync(CircularArrayQueue<EventData> events)
        {
            if (!_isAvailable || !_isConnected)
                return false;
            
            try
            {
                return await SendPostRequestAsync(events);
            }
            catch (Exception ex)
            {
                Log.ColorLogDebugOnly($"Error sending events: {ex.Message}", ColorType.Purple, LogStyle.Warning);
                return false;
            }
        }

        private async UniTask<bool> SendPostRequestAsync(CircularArrayQueue<EventData> eventDatas)
        {
            var arraySegment = new ArraySegment<EventData>(eventDatas.Items, eventDatas.Head, eventDatas.Tail);
            
            var message = JsonConvert.SerializeObject(new {events = arraySegment});
            if (message.IsNullOrEmpty())
                return false;

            using var www = UnityWebRequest.Post(_serverUrl, message);
            www.SetRequestHeader("Content-Type", "application/json");
           
            await www.SendWebRequest();
                
            if (www.result == UnityWebRequest.Result.Success && www.responseCode == 200)
            {
                Log.ColorLogDebugOnly($"Success POST. Status code: {www.responseCode}", ColorType.Lime);
                return true;
            }
            
            Log.ColorLogDebugOnly($"Failed to send events. Status code: {www.responseCode}", ColorType.Orange, LogStyle.Warning);
            return false;
        }

        private void InitializePingCheck()
        {
            _disposable = new CompositeDisposable();
            Observable.Interval(TimeSpan.FromSeconds(_pingInterval))
                .Subscribe(OnCheckPingAsync)
                .AddTo(_disposable);
        }

        private async void OnCheckPingAsync(long x)
        {
            try
            { 
                await PerformPingAsync();
            }
            catch (Exception ex)
            {
                Log.ColorLogDebugOnly($"Error during ping: {ex.Message}", ColorType.Brown, LogStyle.Warning);
            }
        }

        private async UniTask<bool> PerformPingAsync()
        {
            using var www = UnityWebRequest.Head(_serverUrl);
            www.timeout = _requestTimeout;

            try
            {
                await www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    _isConnected = true;
                    return true;
                }
                
                Log.ColorLogDebugOnly($"Ping failed: {www.error}", ColorType.Red, LogStyle.Warning);
                _isConnected = false;
                return false;
            }
            catch (UnityWebRequestException ex)
            {
                Log.ColorLogDebugOnly($"UnityWebRequest error: {ex.Message}", ColorType.Red, LogStyle.Warning);
                _isConnected = false;
                return false;
            }
        }

        void IDisposable.Dispose()
        {
            _disposable?.Dispose();
        }
    }
}