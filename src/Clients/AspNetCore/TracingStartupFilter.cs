using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Thor.Core;
using Thor.Core.Session.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Hosting.AspNetCore
{
    /// <summary>
    /// A <see cref="IStartupFilter"/> to initialize tracing.
    /// </summary>
    public class TracingStartupFilter
        : IStartupFilter
    {
        private readonly IOptions<TracingConfiguration> _configurationAccessor;
        private readonly IAttachmentTransmissionInitializer _initializer;
        private readonly ITelemetrySession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="TracingStartupFilter"/> class.
        /// </summary>
        /// <param name="applicationLifetime">A application lifetime object instance.</param>
        /// <param name="configurationAccessor">A configuration accessor instance.</param>
        /// <param name="initializer">An attachment transmission initializer.</param>
        /// <param name="session">An optional telemetry event session.</param>
        public TracingStartupFilter(
            IApplicationLifetime applicationLifetime,
            IOptions<TracingConfiguration> configurationAccessor,
            IAttachmentTransmissionInitializer initializer,
            ITelemetrySession session)
        {
            if (applicationLifetime == null)
            {
                throw new ArgumentNullException(nameof(applicationLifetime));
            }

            _configurationAccessor = configurationAccessor ??
                throw new ArgumentNullException(nameof(configurationAccessor));
            _initializer = initializer ??
                throw new ArgumentNullException(nameof(initializer));
            _session = session ??
                throw new ArgumentNullException(nameof(initializer));

            Start();
            applicationLifetime.ApplicationStopping.Register(Stop);
        }

        private void Start()
        {
            _initializer.Initialize();
            Application.Start(_configurationAccessor.Value.ApplicationId);
        }

        private void Stop()
        {
            Application.Stop();
            _session.Dispose();
        }

        /// <inheritdoc />
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder
                    .ApplicationServices
                    .GetRequiredService<DiagnosticListener>()
                    .SubscribeWithAdapter(new HostingDiagnosticsListener(_configurationAccessor.Value.SkipRequestFilter));

                next(builder);
            };
        }
    }
}
