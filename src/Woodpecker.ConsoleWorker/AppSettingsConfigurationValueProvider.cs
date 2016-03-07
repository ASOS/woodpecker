using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeHive.Configuration;

namespace Woodpecker.ConsoleWorker
{
    class AppSettingsConfigurationValueProvider : IConfigurationValueProvider
    {
        public string GetValue(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
    }
}
