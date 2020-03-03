using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsicServer.Infrastructure
{
    public class ExtensionSettings
    {
        public IConfiguration configuration { get; private set; }

        public IHttpContextAccessor httpContextAccessor { get; set; }

        public AppSettings appSettings
        {
            get
            {
                var appSettingsSection = this.configuration.GetSection("AppSettings");
                var appSettings = appSettingsSection.Get<AppSettings>();
                return appSettings;
            }
        }

        public ExtensionSettings(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }
    }
}
