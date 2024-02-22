using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction
{
    public static class Utils
    {
        public static string GetConfigSetting(IConfiguration config, string keyName)
        {
            if (!string.IsNullOrEmpty(keyName))
                return config[keyName];
            else
                return null;
        }
    }
}
