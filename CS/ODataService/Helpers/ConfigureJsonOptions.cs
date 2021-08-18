using System;

using DevExpress.Xpo.Helpers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection {
    public sealed class ConfigureJsonOptions : IConfigureOptions<JsonOptions>, IServiceProvider {
        readonly IHttpContextAccessor httpContextAccessor;
        readonly IServiceProvider serviceProvider;
        public ConfigureJsonOptions(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider) {
            this.httpContextAccessor = httpContextAccessor;
            this.serviceProvider = serviceProvider;
        }
        void IConfigureOptions<JsonOptions>.Configure(JsonOptions options) {
            options.JsonSerializerOptions.Converters.Add(new ChangesSetJsonConverterFactory(this));
        }

        object IServiceProvider.GetService(Type serviceType) {
            return (httpContextAccessor.HttpContext?.RequestServices ?? serviceProvider).GetService(serviceType);
        }
    }
}
