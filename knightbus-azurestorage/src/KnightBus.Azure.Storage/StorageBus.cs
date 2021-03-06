using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using KnightBus.Azure.Storage.Messages;
using KnightBus.Core;
using KnightBus.Messages;

namespace KnightBus.Azure.Storage
{
    public interface IStorageBus
    {
        Task SendAsync<T>(T command) where T : class, IStorageQueueCommand;
        Task ScheduleAsync<T>(T command, TimeSpan delay) where T : class, IStorageQueueCommand;
    }

    public class StorageBus : IStorageBus
    {
        private readonly IStorageBusConfiguration _options;
        private readonly ConcurrentDictionary<Type, IStorageQueueClient> _queueClients = new ConcurrentDictionary<Type, IStorageQueueClient>();
        private IMessageAttachmentProvider _attachmentProvider;

        public StorageBus(IStorageBusConfiguration options)
        {
            _options = options;
        }

        public void EnableAttachments(IMessageAttachmentProvider attachmentProvider)
        {
            _attachmentProvider = attachmentProvider;
        }

        private IStorageQueueClient GetClient<T>() where T : class, ICommand
        {
            return _queueClients.GetOrAdd(typeof(T), type => new StorageQueueClient(_options, _attachmentProvider, AutoMessageMapper.GetQueueName<T>()));
        }

        private Task SendAsync<T>(T command, TimeSpan? delay) where T : class, IStorageQueueCommand
        {
            return GetClient<T>().SendAsync(command, delay);
        }

        public Task SendAsync<T>(T command) where T : class, IStorageQueueCommand
        {
            return SendAsync(command, null);
        }

        public Task ScheduleAsync<T>(T command, TimeSpan delay) where T : class, IStorageQueueCommand
        {
            return SendAsync(command, delay);
        }
    }
}