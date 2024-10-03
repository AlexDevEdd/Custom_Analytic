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
        private readonly CircularLinkedList<EventData> _events = new ();
        
        private readonly AnalyticsClient _analyticsClient;
        private readonly AnalyticsStorage _analyticsStorage;
        
        private readonly float _cooldownBeforeSend;
        private IDisposable _cooldownDisposable;
        private CancellationTokenSource _waitFirsEventToken;
        
        public AnalyticsRepository(AnalyticsClient analyticsClient, AnalyticsStorage analyticsStorage,
            AnalyticSettings settings)
        {
            _analyticsClient = analyticsClient;
            _analyticsStorage = analyticsStorage;
            _cooldownBeforeSend = settings.CooldownBeforeSend;
        }
        
        async void IInitializable.Initialize()
        {
           WaitFirsEventAsync().Forget();
           
           var data = await _analyticsStorage.Load();
           if (data.EventsDatas?.Length != 0)
           {
                 await _analyticsClient.TrySendEventsAsync(data.EventsDatas);
                 _analyticsStorage.Save(_events).Forget();
           }
        }
        
        public void TrackEvent(string type, string data)
        {
            _events.Add(new EventData(type, data));
            _analyticsStorage.Save(_events).Forget();
        }

        private async UniTaskVoid WaitFirsEventAsync()
        {
            _waitFirsEventToken = new CancellationTokenSource();
            await UniTask.WaitUntil(() => _events.Count != 0, cancellationToken: _waitFirsEventToken.Token);
            _waitFirsEventToken?.Dispose();
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
           var result = await _analyticsClient.TrySendEventsAsync(_events.ToArray());
           if (result.isSuccess)
           {
               _events.Delete(result.startIndex, result.endIndex);
               _analyticsStorage.Save(_events).Forget();
           }
        }

        void IDisposable.Dispose()
        {
            _waitFirsEventToken?.Cancel();
            _waitFirsEventToken?.Dispose();
            _cooldownDisposable?.Dispose();
        }
    }
}

