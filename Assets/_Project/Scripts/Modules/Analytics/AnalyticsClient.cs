using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UniRx;
using UnityEngine.Networking;
using Utils;
using Zenject;

namespace Analytics
{
    [UsedImplicitly]
    public sealed class AnalyticsClient : IInitializable, IDisposable
    {
        private readonly string _serverUrl;
        private readonly bool _isAvailable;
        private readonly float _pingInterval;
        private readonly int _requestTimeout;
        private bool _isConnected;
        
        private CompositeDisposable _disposable;
        
        public AnalyticsClient(AnalyticSettings settings)
        {
            _isAvailable = settings.IsEnable && !settings.ServerUrl.IsNullOrEmpty();
            
            _serverUrl = settings.ServerUrl;
            _pingInterval = settings.PingInterval;
            _requestTimeout = settings.RequestTimeout;
        }
        
        void IInitializable.Initialize()
        {
            // if (_isAvailable)
            //     InitializePingCheck();
            //TODO: remove flag
            _isConnected = true;
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
                _isConnected = await PerformPingAsync();
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
                    return true;
                
                Log.ColorLogDebugOnly($"Ping failed: {www.error}", ColorType.Red, LogStyle.Warning);
                return false;
            }
            catch (UnityWebRequestException ex)
            {
                Log.ColorLogDebugOnly($"UnityWebRequest error: {ex.Message}", ColorType.Red, LogStyle.Warning);
                return false;
            }
        }

        private async UniTask<bool> SendPostRequestAsync(Array data)
        {
            var message = JsonConvert.SerializeObject(new {events = data});
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

        public async UniTask<(bool isSuccess, int startIndex, int endIndex)> 
            TrySendEventsAsync(Array events)
        {
            if (!_isAvailable || !_isConnected)
                return (false, 0, 0);
            
            try
            {
                var isSuccess = await SendPostRequestAsync(events);
                
                return (isSuccess, 0, events.Length);
            }
            catch (Exception ex)
            {
                Log.ColorLogDebugOnly($"Error sending events: {ex.Message}", ColorType.Purple, LogStyle.Warning);
                return (false, 0, 0);
            }
        }

        void IDisposable.Dispose()
        {
            _disposable?.Dispose();
        }
    }
}