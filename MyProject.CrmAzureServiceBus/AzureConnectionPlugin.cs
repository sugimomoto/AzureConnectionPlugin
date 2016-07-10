using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk;

namespace MyProject.CrmAzureServiceBus
{
    /// <summary>
    /// Pluginの実行コンテキストをAzure Service Bus に受け渡すPlugin
    /// </summary>
    public sealed class AzureConnectionPlugin : IPlugin
    {
        private Guid serviceEndpointId;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="config"></param>
        public AzureConnectionPlugin(string config)
        {
            if(string.IsNullOrEmpty(config) || !Guid.TryParse(config, out serviceEndpointId))
            {
                throw new InvalidPluginExecutionException("Error Service Endpoint");
            }

        }

        /// <summary>
        /// 実行メソッド
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            var cloudService = (IServiceEndpointNotificationService)serviceProvider.GetService(typeof(IServiceEndpointNotificationService));

            if (cloudService == null)
                throw new InvalidPluginExecutionException("Error Plugin");

            try
            {
                tracingService.Trace("Azure Post Start");

                var response = cloudService.Execute(new EntityReference("serviceendpoint", serviceEndpointId), context);

                if (!String.IsNullOrEmpty(response))
                    tracingService.Trace("Response = {0}", response);

                tracingService.Trace("End");                    

            }
            catch(Exception ex)
            {
                tracingService.Trace("Exception:{0}", ex.Message);
                throw;
            }
        }
    }
}
