﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// A transmission sender for <c>Azure</c> <c>BLOB</c> <c>Storage</c>.
    /// </summary>
    public class BlobStorageTransmissionSender
        : ITransmissionSender<AttachmentDescriptor>
    {
        private readonly IBlobContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageTransmissionSender"/> class.
        /// </summary>
        /// <param name="container">A <c>BLOB</c> <c>Storage</c> container instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="container"/> must not be <c>null</c>.
        /// </exception>
        public BlobStorageTransmissionSender(IBlobContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <inheritdoc/>
        public async Task SendAsync(IEnumerable<AttachmentDescriptor> batch, CancellationToken cancellationToken)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            if (!batch.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(batch), ExceptionMessages.CollectionIsEmpty);
            }

            try
            {
                foreach (AttachmentDescriptor descriptor in batch)
                {
                    await _container.UploadAsync(descriptor, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                // todo: log via event provider
            }
        }
    }
}