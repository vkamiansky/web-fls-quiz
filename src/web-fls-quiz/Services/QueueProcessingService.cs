using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFlsQuiz.Data;
using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Services
{
    public class QueueProcessingService : IHostedService, IDisposable
    {
        private readonly CachedDataStorage _dataStorage;

        private Timer _timer;

        public QueueProcessingService(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage as CachedDataStorage;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_dataStorage != null)
                _timer = new Timer(HandleResultsQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void HandleResultsQueue(object state)
        {
            if (_dataStorage != null)
            {
                while (await _dataStorage.TryInsertResult()) ;
            }
        }
    }
}
