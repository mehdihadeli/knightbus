﻿using System;
using System.Collections.Generic;
using KnightBus.Azure.ServiceBus.Messages;
using KnightBus.Core;
using KnightBus.Messages;

namespace KnightBus.Azure.ServiceBus
{
    internal class ServiceBusQueueChannelFactory : ITransportChannelFactory
    {
        public ServiceBusQueueChannelFactory(IServiceBusConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ITransportConfiguration Configuration { get; set; }
        public IList<IMessageProcessorMiddleware> Middlewares { get; } = new List<IMessageProcessorMiddleware>();

        public IChannelReceiver Create(Type messageType, IEventSubscription subscription, IProcessingSettings processingSettings, IHostConfiguration configuration, IMessageProcessor processor)
        {
            var queueReaderType = typeof(ServiceBusQueueChannelReceiver<>).MakeGenericType(messageType);
            var queueReader = (IChannelReceiver)Activator.CreateInstance(queueReaderType, processingSettings, Configuration, configuration, processor);
            return queueReader;
        }

        
        public bool CanCreate(Type messageType)
        {
            return typeof(IServiceBusCommand).IsAssignableFrom(messageType);
        }
    }
}