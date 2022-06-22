// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Sedio.Client.Infrastructure.Http
{
    /// <summary>
    /// A builder abstraction for configuring <see cref="HttpMessageHandler"/> instances.
    /// </summary>
    /// <remarks>
    /// The <see cref="HttpMessageHandlerBuilder"/> is registered in the service collection as
    /// a transient service. Callers should retrieve a new instance for each <see cref="HttpMessageHandler"/> to
    /// be created. Implementors should expect each instance to be used a single time.
    /// </remarks>
    public abstract class HttpMessageHandlerBuilder
    {
        /// <summary>
        /// Gets or sets the primary <see cref="HttpMessageHandler"/>.
        /// </summary>
        public abstract HttpMessageHandler PrimaryHandler { get; set; }

        /// <summary>
        /// Gets a list of additional <see cref="DelegatingHandler"/> instances used to configure an
        /// <see cref="HttpClient"/> pipeline.
        /// </summary>
        public abstract IList<DelegatingHandler> AdditionalHandlers { get; }

        /// <summary>
        /// Creates an <see cref="HttpMessageHandler"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="HttpMessageHandler"/> built from the <see cref="PrimaryHandler"/> and
        /// <see cref="AdditionalHandlers"/>.
        /// </returns>
        public abstract HttpMessageHandler Build();

        protected internal static HttpMessageHandler CreateHandlerPipeline(HttpMessageHandler primaryHandler, IEnumerable<DelegatingHandler> additionalHandlers)
        {
            // This is similar to https://github.com/aspnet/AspNetWebStack/blob/master/src/System.Net.Http.Formatting/HttpClientFactory.cs#L58
            // but we don't want to take that package as a dependency.

            if (primaryHandler == null)
            {
                throw new ArgumentNullException(nameof(primaryHandler));
            }

            if (additionalHandlers == null)
            {
                throw new ArgumentNullException(nameof(additionalHandlers));
            }

            var additionalHandlersList = additionalHandlers as IReadOnlyList<DelegatingHandler> ?? additionalHandlers.ToArray();

            var next = primaryHandler;
            for (var i = additionalHandlersList.Count - 1; i >= 0; i--)
            {
                var handler = additionalHandlersList[i];
                if (handler == null)
                {
                    throw new InvalidOperationException("Additional handler is null");
                }

                // Checking for this allows us to catch cases where someone has tried to re-use a handler. That really won't
                // work the way you want and it can be tricky for callers to figure out.
                if (handler.InnerHandler != null)
                {
                    throw new InvalidOperationException("Additional handler is invalid");
                }

                handler.InnerHandler = next;
                next = handler;
            }

            return next;
        }
    }
}