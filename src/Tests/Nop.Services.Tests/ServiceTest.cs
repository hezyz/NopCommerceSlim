using System.Collections.Generic;
using Nop.Core.Plugins;
using NUnit.Framework;

namespace Nop.Services.Tests
{
    [TestFixture]
    public abstract class ServiceTest
    {
        [SetUp]
        public void SetUp()
        {
            //init plugins
            InitPlugins();
        }

        private void InitPlugins()
        {
            var plugins = new List<PluginDescriptor>();
            PluginManager.ReferencedPlugins = plugins;
        }
    }
}
