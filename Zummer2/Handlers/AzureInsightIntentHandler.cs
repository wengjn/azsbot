using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using Zummer.Services;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Luis;
using System;

namespace Zummer.Handlers
{
    internal sealed class AzureInsightIntentHandler : IIntentHandler
    {
        private readonly IAzureDiagnosticService azureDiagnosticService;
        private readonly IBotToUser botToUser;

        public AzureInsightIntentHandler(IBotToUser botToUser, IAzureDiagnosticService azureDiagnosticService)
        {
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
            SetField.NotNull(out this.azureDiagnosticService, nameof(azureDiagnosticService), azureDiagnosticService);
        }

        public async Task Respond(IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            EntityRecommendation entityRecommendation;

            await this.botToUser.PostAsync(Strings.AzureInsight);

            var query = result.TryFindEntity(ZummerStrings.SubId, out entityRecommendation)
                ? entityRecommendation.Entity
                : result.Query;

            var ls = query.Split(' ');
            var subId = string.Empty;
            
            for(int i=0; i<ls.Length; i++)
            {
                if (ls[i] == "for")
                {
                    subId = ls[i + 1];
                    break;
                }
            }

            // call ads with a hardcode requestId
            // var adsCall = await this.azureDiagnosticService.GetDiagnosticResultWith("7ae3d58d-2180-4139-a8a2-dcfb8de666e6");
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            var test = Guid.Parse(subId).ToString();
            parameters.Add("SubscriptionId", test);

            var adsCall = await this.azureDiagnosticService.GetDataByPollModeAsync("AzureInsightData", parameters);
            await this.botToUser.PostAsync(JsonConvert.SerializeObject(adsCall.DiagnosticData));
        }
    }
}