﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// A transmission sender for <c>Azure</c> <c>EventHub</c>.
    /// </summary>
    public class EventHubTransmissionSender
        : ITransmissionSender<EventData>
    {
        private readonly EventHubClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubTransmissionSender"/> class.
        /// </summary>
        /// <param name="client">A <c>Azure</c> <c>EventHub</c> client instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="client"/> must not be <c>null</c>.
        /// </exception>
        public EventHubTransmissionSender(EventHubClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <inheritdoc />
        public Task SendAsync(IEnumerable<EventData> batch, CancellationToken cancellationToken)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            if (!batch.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(batch), ExceptionMessages.CollectionIsEmpty);
            }
            return _client.SendAsync(batch);
        }
    }
}