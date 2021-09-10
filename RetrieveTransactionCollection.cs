using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeProject
{
    public class RetrieveTransactionCollection : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService traceService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            EntityCollection transactions = new EntityCollection
            {
                EntityName = "practice_mtransaction"
            };

            try 
            {
                EntityReference recurringReference = null;
                if (context.InputParameters.Contains("recurringexp") && context.InputParameters["recurringexp"] != null)
                    recurringReference = (EntityReference)context.InputParameters["recurringexp"];
                else
                    throw new InvalidPluginExecutionException("RecurringReference Not Found");

                QueryExpression query = new QueryExpression("practice_mtransaction");
                query.NoLock = true;
                query.ColumnSet.AddColumns("practice_m_transaction", "practice_amount", "practice_payee", "practice_correspondant", "practice_transactiontype");
                query.Criteria.AddCondition("practice_m_transaction", ConditionOperator.Equal, recurringReference.Id );

                transactions= service.RetrieveMultiple(query);

            }
            catch(Exception e)
            {
                context.OutputParameters["error"] = e.Message;
            }
            finally
            {
                context.OutputParameters["transactions"] = transactions;
            }



        }
    }
}
