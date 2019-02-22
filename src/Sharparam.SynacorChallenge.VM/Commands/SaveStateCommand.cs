namespace Sharparam.SynacorChallenge.VM.Commands
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    using Microsoft.Extensions.Logging;

    public sealed class SaveStateCommand : Command
    {
        public SaveStateCommand(ILogger<SaveStateCommand> log)
            : base(log, @"^\$save (?<path>.+)$")
        {
        }

        public override (bool Handled, bool AdjustPointer) Run(in Cpu cpu, in Match match)
        {
            var path = match.Groups["path"].Value.Trim();

            if (string.IsNullOrWhiteSpace(path))
            {
                Log.LogError("Path cannot be empty");
            }

            try
            {
                path = Path.ChangeExtension(path, "state");
            }
            catch (ArgumentException ex)
            {
                Log.LogError(ex, "Path contains invalid characters");
                return (true, true);
            }

            var state = cpu.CopyState();
            state.InstructionPointer--;

            try
            {
                state.SaveToDumpFile(path);
                Log.LogInformation("Save state saved to \"{Path}\"", path);
            }
            catch (ArgumentException ex)
            {
                Log.LogError(ex, "Problems with path name");
            }
            catch (IOException ex)
            {
                Log.LogError(ex, "Unable to save state file, IO problem");
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.LogError(ex, "Failed to save file due to insufficient filesystem permissions");
            }

            return (true, true);
        }
    }
}
