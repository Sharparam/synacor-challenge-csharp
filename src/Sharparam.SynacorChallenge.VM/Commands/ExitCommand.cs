namespace Sharparam.SynacorChallenge.VM.Commands
{
    using System.Text.RegularExpressions;

    using Microsoft.Extensions.Logging;

    public class ExitCommand : Command
    {
        public ExitCommand(ILogger<ExitCommand> log)
            : base(log, @"^\$(?:exit|q(?:uit)?|)$")
        {
        }

        public override (bool Handled, bool AdjustPointer) Run(in Cpu cpu, in Match match)
        {
            cpu.Halt();
            return (true, true);
        }
    }
}
