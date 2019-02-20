namespace Sharparam.SynacorChallenge.VM
{
    public class NullInputReader : IInputReader
    {
        public string ReadLine() => null;
    }
}
