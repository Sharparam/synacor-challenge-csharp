namespace Sharparam.SynacorChallenge.VM.Commands
{
    using System.Text.RegularExpressions;

    using JetBrains.Annotations;

    using Microsoft.Extensions.Logging;

    public abstract class Command : ICommand
    {
        public Command(ILogger log, [RegexPattern] string pattern)
        {
            Log = log;
            CommandRegex = new Regex(
                pattern,
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        protected ILogger Log { get; }

        public Regex CommandRegex { get; }

        public abstract (bool Handled, bool AdjustPointer) Run(Cpu cpu, Match match);
    }
}
