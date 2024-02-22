using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorTests
{
    internal class TestConfig : IConfiguration
    {
        public string? this[string key] 
        {
            get { return TestUtils.nrRetries.ToString(); }
            set => throw new NotImplementedException("TestConfig.this[key].set not implemented"); 
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException("TestConfig.GetChildren not implemented");
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException("TestConfig:GetReloadToken not implemented");
        }

        public IConfigurationSection GetSection(string key)
        {
            throw new NotImplementedException("TestConfig.GetSection not implemented");
        }
    }
}
