using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using Zummer.Services;

namespace Zummer.Handlers
{
    internal sealed class HelpIntentHandler : IIntentHandler
    {
        private readonly IBotToUser botToUser;

        public HelpIntentHandler(IBotToUser botToUser)
        {
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
        }

        public async Task Respond(IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var helpMessage = new ReplyMessageBuilder();
            helpMessage.AppendLine("I did not get that. This is still beta. Please be patient with me.");
            helpMessage.AppendLine("Currently we only support the following scenario: ");

            var header = new[] { "Scenario Name", "Example Phrase" };
            var tableBody = new List<string>();

            var textInfo = new CultureInfo("en-US", false).TextInfo;
            tableBody.Add("Get EA # ");
            tableBody.Add("Get me enrollment number for XXXX");
            tableBody.Add("Bing search ");
            tableBody.Add("who is tom hanks");
            helpMessage.AppendTable(2, tableBody, header);
            await this.botToUser.PostAsync(helpMessage.ToString());
        }
    }
}