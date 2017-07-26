using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Zummer.Handlers;
using Microsoft.Bot.Builder.FormFlow;
using Zummer.Services;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Zummer.Dialogs
{
    /// <summary>
    /// The top-level natural language dialog for sample.
    /// </summary>
    [Serializable]
    internal sealed class MainDialog : LuisDialog<object>
    {
        private readonly IHandlerFactory handlerFactory;

        private readonly IAzureDiagnosticService azureDiagnosticService;

        public MainDialog(ILuisService luis, IHandlerFactory handlerFactory, IAzureDiagnosticService azureDiagnosticService)
            : base(luis)
        {
            SetField.NotNull(out this.handlerFactory, nameof(handlerFactory), handlerFactory);
            SetField.NotNull(out this.azureDiagnosticService, nameof(azureDiagnosticService), azureDiagnosticService);
        }

        [LuisIntent(ZummerStrings.GreetingIntentName)]
        public async Task GreetingIntentHandlerAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await this.handlerFactory.CreateIntentHandler(ZummerStrings.GreetingIntentName).Respond(activity, result);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent(ZummerStrings.VMIntentName)]
        public async Task VMIntentHandlerAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var form = new FormDialog<Survey>(new Survey(), BuildSurveyForm, FormOptions.PromptInStart);
            context.Call(form, this.OnSurveyCompleted);
        }

        private static IForm<Survey> BuildSurveyForm()
        {
            return new FormBuilder<Survey>()
                .AddRemainingFields()
                .Build();
        }

        private async Task OnSurveyCompleted(IDialogContext context, IAwaitable<Survey> result)
        {
            try
            {
                var survey = await result;

                await context.PostAsync($"Got it... Please wait while we are running diagnosis for you.");

                // use the info to call ADS
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("ResourceUri", survey.ResourceUri);

                var adsCall = await this.azureDiagnosticService.GetDataByPollModeAsync("VMConsoleLogData", parameters);
                await context.PostAsync(JsonConvert.SerializeObject(adsCall.DiagnosticData));
            }
            catch (FormCanceledException<Survey> e)
            {
                string reply;

                if (e.InnerException == null)
                {
                    reply = "You have canceled";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {e.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }

            context.Done(string.Empty);
        }

        [LuisIntent(ZummerStrings.AzureInsightIntentName)]
        public async Task AzureInsightIntentHandlerAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await this.handlerFactory.CreateIntentHandler(ZummerStrings.AzureInsightIntentName).Respond(activity, result);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent(ZummerStrings.HelpIntentName)]
        public async Task HelpIntentHandlerAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await this.handlerFactory.CreateIntentHandler(ZummerStrings.HelpIntentName).Respond(activity, result);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent(ZummerStrings.SearchIntentName)]
        public async Task FindArticlesIntentHandlerAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await this.handlerFactory.CreateIntentHandler(ZummerStrings.SearchIntentName).Respond(activity, result);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task FallbackIntentHandlerAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(string.Format(Strings.FallbackIntentMessage));
            context.Wait(this.MessageReceived);
        }
    }
}