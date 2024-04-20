using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.Test;
using RhDev.Zakaznik.Reseni.Common.DataAccess.SharePoint.Configuration.Objects;

namespace $ext_safeprojectname$.Common.Test
{
    [TestClass]
    public class ConfigurationTest : UnitTestOf<GlobalConfiguration>
    {
        private ConfigurationKey ck = new ConfigurationKey("module", "key");
        protected override void Setup()
        {
            base.Setup();

            var ds = Mocker.Get<IConfigurationDataSource>();

            ds.GetValue(Arg.Any<ConfigurationKey>()).ReturnsForAnyArgs(new ConfigurationValue(ck, "server"));
        }

        [TestMethod]
        public void CheckConfgurationReturnsDataAsExpected()
        {
            var sut = SUT;
            var ds = Mocker.Get<IConfigurationDataSource>();
                        
            var server = sut.EmptyJobServer;

            server.Should().Be("server");
        }
    }
}
