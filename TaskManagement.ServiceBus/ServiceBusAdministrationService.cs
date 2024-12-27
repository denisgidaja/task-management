using Azure.Messaging.ServiceBus.Administration;
using TaskManagement.Contracts;

namespace TaskManagement.ServiceBus
{
    public class ServiceBusAdministrationService
    {
        public ServiceBusAdministrationService()
        {
        }

        public static async Task<ServiceBusAdministrationClient> CreateAsync(string connectionString, string topicName)
        {

            var adminClient = new ServiceBusAdministrationClient(connectionString);
            // Define SQL filters
            var updateFilter = new SqlRuleFilter($"OperationType = '{OperationConstants.Update}'");

            // Add rules to subscriptions
            var rules = adminClient.GetRulesAsync(topicName, SubscriptionConstants.CreateSubscription);

            await AddRulesIfNotExist(topicName, adminClient);

            return adminClient;
        }
        private static async Task<bool> RuleExists(string topicName, ServiceBusAdministrationClient adminClient, string subscription, string ruleName)
        {
            var creationRules = adminClient.GetRulesAsync(topicName, subscription);
            await foreach (var rule in creationRules)
            {
                // Check if the rule exists
                if (rule.Name.Equals(ruleName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static async Task AddRulesIfNotExist(string topicName, ServiceBusAdministrationClient adminClient)
        {
            if(!await RuleExists(topicName, adminClient, SubscriptionConstants.CreateSubscription, RulesConstants.CreateRule))
                    await adminClient.CreateRuleAsync(topicName, SubscriptionConstants.CreateSubscription, new CreateRuleOptions(RulesConstants.CreateRule, new SqlRuleFilter($"OperationType = '{OperationConstants.Create}'")));
             
            if(!await RuleExists(topicName, adminClient, SubscriptionConstants.UpdateSubscription, RulesConstants.UpdateRule))
                    await adminClient.CreateRuleAsync(topicName, SubscriptionConstants.UpdateSubscription, new CreateRuleOptions(RulesConstants.UpdateRule, new SqlRuleFilter($"OperationType = '{OperationConstants.Update}'")));
            
            if(!await RuleExists(topicName, adminClient, SubscriptionConstants.FeedbackCreateSubscription, RulesConstants.FeedbackCreateRule))
                    await adminClient.CreateRuleAsync(topicName, SubscriptionConstants.FeedbackCreateSubscription, new CreateRuleOptions(RulesConstants.FeedbackCreateRule, new SqlRuleFilter($"OperationType = '{OperationConstants.FeedbackCreated}'")));
             
            if(!await RuleExists(topicName, adminClient, SubscriptionConstants.FeedbackUpdateSubscription, RulesConstants.FeedbackUpdateRule))
                    await adminClient.CreateRuleAsync(topicName, SubscriptionConstants.FeedbackUpdateSubscription, new CreateRuleOptions(RulesConstants.FeedbackUpdateRule, new SqlRuleFilter($"OperationType = '{OperationConstants.FeedbackUpdated}'")));
          
        }
    }
}
