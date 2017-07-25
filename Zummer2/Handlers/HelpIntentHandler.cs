using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

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
            await this.botToUser.PostAsync(Strings.AskForHelp);
        }
    }
}