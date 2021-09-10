using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadhuTrain.plugin.Practice
{
    public class DeleteTransaction : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService traceService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            context.OutputParameters["success"] = false;
            context.OutputParameters["error"] = " ";
            try
            {
                EntityReference reference = null;

                if (context.InputParameters.Contains("trans") && context.InputParameters["trans"] != null)
                    reference = (EntityReference)context.InputParameters["trans"];
                context.OutputParameters["error"] += "1";
                if (reference == null)
                {
                    throw new Exception("No Transaction Found");
                }
                context.OutputParameters["error"] += ("2 "+reference.Id.ToString()+" "+reference.LogicalName);
                
                service.Delete("practice_mtransaction", reference.Id);
                
                context.OutputParameters["success"] = true;
                
                QueryExpression query = new QueryExpression("practice_mtransaction");
                query.ColumnSet.AddColumns("practice_amount", "practice_payee", "practice_correspondant");

                service.Retrieve("practice_mtransaction", reference.Id, query.ColumnSet);
                
                /*
                QueryExpression query = new QueryExpression("practice_mtransaction");
                query.ColumnSet.AddColumns("practice_amount","practice_payee","practice_correspondant");
                query.Criteria.AddCondition("id", ConditionOperator.Equal, reference.Id);
                if (service.RetrieveMultiple(query).TotalRecordCount == 0)
                    context.OutputParameters["success"] = true;
                else
                    context.OutputParameters["success"] = false;
                */
            }

            catch (Exception e)
            {
                context.OutputParameters["error"] += e.Message;
            }
            finally
            {
                
            }

            
        }
    }
}
