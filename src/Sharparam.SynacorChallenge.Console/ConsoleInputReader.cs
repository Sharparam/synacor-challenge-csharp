namespace Sharparam.SynacorChallenge.Console
{
    using System.Linq;

    using VM;

    public class ConsoleInputReader : IInputReader
    {
        public ushort Read() => System.Console.ReadLine().First();
    }
}
