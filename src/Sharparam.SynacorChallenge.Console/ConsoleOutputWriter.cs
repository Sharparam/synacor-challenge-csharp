namespace Sharparam.SynacorChallenge.Console
{
    using VM;

    public class ConsoleOutputWriter : IOutputWriter
    {
        public void Write(char value)
        {
            System.Console.Write(value);
        }
    }
}
