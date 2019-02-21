namespace Sharparam.SynacorChallenge.VM.Commands
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    using Data;

    using Microsoft.Extensions.Logging;

    public sealed class LoadStateCommand : Command
    {
        public LoadStateCommand(ILogger<LoadStateCommand> log)
            : base(log, @"^load (?<path>.+)$")
        {
        }

        public override (bool Handled, bool AdjustPointer) Run(Cpu cpu, Match match)
        {
            var path = match.Groups["path"].Value.Trim();

            if (string.IsNullOrWhiteSpace(path))
            {
                Log.LogError("Path cannot be empty");
            }

            State state;

            try
            {
                state = State.FromDumpFile(path);
            }
            catch (ArgumentException ex)
            {
                Log.LogError(ex, "Problems with path name");
                return (true, true);
            }
            catch (IOException ex)
            {
                Log.LogError(ex, "Unable to save state file, IO problem");
                return (true, true);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.LogError(ex, "Failed to save file due to insufficient filesystem permissions");
                return (true, true);
            }

            cpu.LoadState(state);
            Log.LogInformation("Loaded save state from \"{Path}\"", path);

            return (true, false);
        }
    }
}
