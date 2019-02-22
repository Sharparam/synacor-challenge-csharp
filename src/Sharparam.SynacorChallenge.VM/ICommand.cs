namespace Sharparam.SynacorChallenge.VM
{
    using System.Text.RegularExpressions;

    public interface ICommand
    {
        Regex CommandRegex { get; }

        (bool Handled, bool AdjustPointer) Run(in Cpu cpu, in Match match);
    }
}
