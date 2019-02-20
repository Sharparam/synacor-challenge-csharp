namespace Sharparam.SynacorChallenge.Console
{
    using System.Linq;

    using VM;

    public class ConsoleInputReader : IInputReader
    {
        public string ReadLine() => System.Console.ReadLine();
    }
}
