namespace Sharparam.SynacorChallenge.VM
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    public class CommandManager
    {
        private readonly ILogger _log;

        private readonly IReadOnlyList<ICommand> _commands;

        public CommandManager(ILogger<CommandManager> log, IEnumerable<ICommand> commands)
        {
            _log = log;
            _commands = commands.ToList().AsReadOnly();
            _log.LogInformation("Loaded {Count} commands", _commands.Count);
        }

        public (bool Handled, bool AdjustPointer) Handle(Cpu cpu, string line)
        {
            foreach (var command in _commands)
            {
                var match = command.CommandRegex.Match(line);

                if (!match.Success)
                {
                    continue;
                }

                return command.Run(cpu, match);
            }

            return (false, true);
        }
    }
}
