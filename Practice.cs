using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;




namespace PracticeProject
{
    public class Practice : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService traceService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            
            Guid recExp = Guid.Empty;
            Guid transaction = Guid.Empty;
            context.OutputParameters["id"] = "";
            try
            {
                string name = string.Empty;
                Double amount = 0;
                Guid transType = Guid.Empty;

                if (context.InputParameters.Contains("name"))
                {
                    name = context.InputParameters["name"].ToString();
                }
                else
                {
                    throw new InvalidPluginExecutionException("REC NAME expected");
                }
                if (context.InputParameters.Contains("amount"))
                {
                    amount = (double)context.InputParameters["amount"];
                }
                else
                {
                    throw new InvalidPluginExecutionException("amount expected");
                }
    
                Entity rec = new Entity("practice_mrecurringexpenditure");
                rec["practice_name"] = name;
                rec.Id = service.Create(rec);
                recExp = rec.Id;

                Entity trans = new Entity("practice_mtransaction");
                trans["practice_amount"] = amount;
                trans["practice_m_transaction"] = new EntityReference("practice_mrecurringexpenditure",recExp);
                trans.Id = (service.Create(trans));
                transaction = trans.Id;
            }
            catch (Exception exception)
            {
                context.OutputParameters["success"] = false;
                context.OutputParameters["id"] = (""+exception.Message);
            }
            finally 
            {
                context.OutputParameters["id"] += (" \n RecurringExp: "+recExp.ToString()+"\n Transaction : "+transaction.ToString());
            }


         
        }
    }
}
