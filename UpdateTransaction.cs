using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeProject
{
    public class UpdateTransaction : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService traceService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Entity transaction = null;

            try
            {
                EntityReference transactionRef = null;
                if (context.InputParameters.Contains("trans") && context.InputParameters["trans"] != null)
                    transactionRef = (EntityReference)context.InputParameters["trans"];
                else
                    throw new Exception("Transaction Reference format error");
                transaction = service.Retrieve("practice_mtransaction", transactionRef.Id, new ColumnSet(true) );
                if (transaction == null)
                    throw new Exception("Transaction Not found error");
                transaction["practice_amount"] = 555555555;
                transaction["subject"] = "checker value from custom action pl";
                service.Update(transaction);
                context.OutputParameters["success"] = true;
            }
            catch(Exception e)
            {
                context.OutputParameters["error"] = e.Message;
            }
            finally
            {

            }


        }
    }
}
