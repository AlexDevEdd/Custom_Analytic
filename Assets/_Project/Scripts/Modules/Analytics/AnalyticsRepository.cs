using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UniRx;
using Utils;
using Zenject;

namespace Analytics
{
    [UsedImplicitly]
    public class AnalyticsRepository : IInitializable, IDisposable
    {
        private CircularArrayQueue<EventData> _events;
        
        private readonly AnalyticsClient _analyticsClient;
        private readonly AnalyticsStorage _analyticsStorage;
        private readonly float _cooldownBeforeSend;
        
        private IDisposable _cooldownDisposable;
        private CancellationTokenSource _waitFirsEventToken;
        
        public AnalyticsRepository(AnalyticsClient analyticsClient, AnalyticsStorage analyticsStorage,
            AnalyticsSettings settings)
        {
            _analyticsClient = analyticsClient;
            _analyticsStorage = analyticsStorage;
            _cooldownBeforeSend = settings.CooldownBeforeSend;
        }
        
        async void IInitializable.Initialize()
        {
            await _analyticsClient.Init();
            _events = await _analyticsStorage.LoadAsync();

            if (_events.Count!= 0) 
               TrySendEventsAsync().Forget();
            
            WaitFirsEventAsync().Forget();
        }
        
        public void TrackEvent(string type, string data)
        {
            _events.Enqueue(new EventData(type, data));
            _analyticsStorage.SaveAsync(_events).Forget();
        }

        private async UniTaskVoid WaitFirsEventAsync()
        {
            _waitFirsEventToken = new CancellationTokenSource();
            await UniTask.WaitUntil(() => _events.Count != 0, cancellationToken: _waitFirsEventToken.Token);
            InitializeCooldown();
        }
        
        private void InitializeCooldown()
        {
            _cooldownDisposable = Observable.Interval(TimeSpan.FromSeconds(_cooldownBeforeSend))
                .Subscribe(_ =>
                {
                    if (_events.Count > 0)
                        TrySendEventsAsync().Forget();
                });
        }

        private async UniTaskVoid TrySendEventsAsync()
        {
            var startIndex = _events.Head;
            var endIndex = _events.Tail;
            
            var isSuccess = await _analyticsClient.TrySendEventsAsync(_events);
            if (isSuccess) 
                UpdateEvents(_events, startIndex, endIndex);
        }

        private void UpdateEvents(CircularArrayQueue<EventData> eventDatas, in int startIndex, in int endIndex)
        {
            _events.RemoveRange(startIndex, endIndex);
            _analyticsStorage.SaveAsync(eventDatas).Forget();
        }

        void IDisposable.Dispose()
        {
            _waitFirsEventToken?.Cancel();
            _waitFirsEventToken?.Dispose();
            _cooldownDisposable?.Dispose();
        }
    }
}

