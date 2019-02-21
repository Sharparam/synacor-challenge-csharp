namespace Sharparam.SynacorChallenge.VM
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    public class CommandManager
    {
        private readonly ILogger _log;

        private readonly Cpu _cpu;

        private readonly IReadOnlyList<ICommand> _commands;

        public CommandManager(ILogger<CommandManager> log, Cpu cpu, IEnumerable<ICommand> commands)
        {
            _log = log;
            _cpu = cpu;
            _commands = commands.ToList().AsReadOnly();
        }

        public (bool Handled, bool AdjustPointer) Handle(string line)
        {
            foreach (var command in _commands)
            {
                var match = command.CommandRegex.Match(line);

                if (!match.Success)
                {
                    continue;
                }

                return command.Run(_cpu, match);
            }

            return (false, true);
        }
    }
}
