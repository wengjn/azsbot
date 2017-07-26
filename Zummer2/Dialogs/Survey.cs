
namespace Zummer.Dialogs
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class Survey
    {
        [Prompt("What's the ResourceUri?")]
        public string ResourceUri { get; set; }
    }
}