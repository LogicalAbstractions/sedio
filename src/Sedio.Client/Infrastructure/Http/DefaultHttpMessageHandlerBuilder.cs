// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Sedio.Client.Infrastructure.Http
{
    class DefaultHttpMessageHandlerBuilder : HttpMessageHandlerBuilder
    {
        public override HttpMessageHandler PrimaryHandler { get; set; } = new HttpClientHandler();

        public override IList<DelegatingHandler> AdditionalHandlers { get; } = new List<DelegatingHandler>();

        public override HttpMessageHandler Build()
        {
            if (PrimaryHandler == null)
            {
                throw new InvalidOperationException("No primary handler");
            }
            
            return CreateHandlerPipeline(PrimaryHandler, AdditionalHandlers);
        }
    }
}
