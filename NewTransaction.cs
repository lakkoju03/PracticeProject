using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeProject
{
    public class NewTransaction : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService traceService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Guid transaction = Guid.Empty;
            bool success = false;
            try
            {

                int amount = 0;
                string correspondant = string.Empty ;
                string recurring = string.Empty;
                EntityReference recurringReference = null;
                string note = string.Empty;
                string payee = string.Empty;
                EntityCollection transactionType = null;
                OptionSetValue picklist = null;

                //Access input parameters

                //amount
                if (context.InputParameters.Contains("amount") && context.InputParameters["amount"].GetType() == typeof(int))
                    amount = (int)context.InputParameters["amount"];
                else
                    throw new InvalidPluginExecutionException("Amount needed in int type");

                //correspondant ID
                if (context.InputParameters.Contains("correspondant") && !context.InputParameters["correspondant"].ToString().Equals("")
                    && !(context.InputParameters["correspondant"].ToString().Replace(" ", "").Length == 0))
                    correspondant = context.InputParameters["correspondant"].ToString();
                else
                    throw new Exception("Correspondant ID not sent");

                //recurring ID
                if (context.InputParameters.Contains("recurring") && !context.InputParameters["recurring"].ToString().Equals("")
                    && !(context.InputParameters["recurring"].ToString().Replace(" ", "").Length == 0))
                    recurring = (string)context.InputParameters["recurring"];
               else
                    throw new Exception("Recurring ID not sent");

                //Recurring EntityReference
                if (context.InputParameters.Contains("recurringReference") && context.InputParameters["recurringReference"] != null)
                    recurringReference = (EntityReference)context.InputParameters["recurringReference"];
                else
                    throw new Exception("Recurring Reference not available/found");
                //Note
                if(context.InputParameters.Contains("note") && context.InputParameters["note"] != null && context.InputParameters["note"].ToString().Replace(" ","").Length != 0 )
                     note = context.InputParameters["note"].ToString();

                //Payee
                if (context.InputParameters.Contains("payee") && context.InputParameters["payee"] != null && context.InputParameters["payee"].ToString().Replace(" ", "").Length != 0)
                    payee = context.InputParameters["payee"].ToString();
                else
                    throw new Exception("Payee ID not found");
                
                //TransactionTypeCollection (Entity collection)
                if(context.InputParameters.Contains("transactiontypesCollection") && context.InputParameters["transactiontypesCollection"] != null)
                    transactionType = (EntityCollection)context.InputParameters["transactiontypesCollection"];
                
                //Picklist for TransactionType choice set
                if(context.InputParameters.Contains("picklist") && context.InputParameters["picklist"] != null )
                    picklist = (OptionSetValue)context.InputParameters["picklist"];

                //Creating the transaction Object

                Entity createdTransaction = new Entity("practice_mtransaction");

                createdTransaction["practice_amount"] = amount;
                
                //Correspondant check
                Guid corres = Guid.Empty;
                if(!Guid.TryParse(correspondant, out corres) || corres == null)
                    throw new InvalidPluginExecutionException("Unsupported ID for Correspondant ID");
                else 
                    createdTransaction["practice_correspondant"] = new EntityReference("systemuser", corres);
                
                //RecurringReference Check
                Guid recRef = Guid.Empty;
                if (!Guid.TryParse(recurring, out recRef))
                    throw new InvalidPluginExecutionException("Unsupported ID for RecurringExpenditure");
                if (recRef == null)
                    throw new InvalidPluginExecutionException("RecurringExp Reference is NULL");
                if (recRef != recurringReference.Id)
                    throw new Exception("Recurring Referece and RecurringReference generated doesnt match");
                else
                    createdTransaction["practice_m_transaction"] = recurringReference;

                createdTransaction["practice_note"] = note;
                
                //Payee check
                Guid payeeGuid = Guid.Empty;
                if(!Guid.TryParse(payee, out payeeGuid))
                    throw new Exception("Payee Guid Unsupported Format");
                if (payeeGuid == null)
                    throw new Exception("Payee Not Found / null");
                else
                    createdTransaction["practice_payee"] = new EntityReference("systemuser", payeeGuid);

                createdTransaction["practice_transactiontype"] = picklist;

                //context.OutputParameters["transaction"] += "success until getting data to ACTION";

                createdTransaction.Id = service.Create(createdTransaction);

                transaction = createdTransaction.Id;

                success = true;
            }
            catch (Exception e)
            {
                context.OutputParameters["transaction"] += ("  " + e.Message);
                success = false;
            }
            finally
            {
                context.OutputParameters["transaction"] = transaction.ToString();
                context.OutputParameters["success"] = success;
            }

        }
    }
}
