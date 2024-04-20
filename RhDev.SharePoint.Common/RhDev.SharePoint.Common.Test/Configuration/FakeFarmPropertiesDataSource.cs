using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration;
using RhDev.SharePoint.Common.Logging;
using System.Collections;

namespace RhDev.SharePoint.Common.Test.Configuration
{
    public class FakeFarmPropertiesDataSource : FarmPropertiesDataSource
    {
        protected override string FarmConfigPrefix => "TestSolution";

        Hashtable farmConfigInternal = new Hashtable();

        public FakeFarmPropertiesDataSource(ITraceLogger traceLogger) : base(traceLogger)
        {

        }

        public override ConfigurationValue GetValue(ConfigurationKey key)
        {
            WriteTrace($"GetValue called for key : {key}");

            return (ConfigurationValue)farmConfigInternal[key];
        }

        public override void SetValue(ConfigurationKey key, object value)
        {
            WriteTrace($"SetValue called for key : {key}, value : {value}");

            if(!farmConfigInternal.ContainsKey(key)) farmConfigInternal.Add(key, new ConfigurationValue(key, value));
        }

        public override void SaveChanges()
        {
            //save changees dont do anything
        }
    }
}
