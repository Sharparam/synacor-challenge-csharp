namespace Sharparam.SynacorChallenge.VM.Commands
{
    using System.Text.RegularExpressions;

    using Microsoft.Extensions.Logging;

    public class AddressCommand : Command
    {
        public AddressCommand(ILogger<AddressCommand> log)
            : base(log, @"^\$addr(?:ess)?$")
        {
        }

        public override (bool Handled, bool AdjustPointer) Run(in Cpu cpu, in Match match)
        {
            var actualAddress = cpu.Pointer - 1;
            Log.LogInformation("Current address: {Address} (0x{HexAddress:X})", actualAddress, actualAddress);
            return (true, true);
        }
    }
}
