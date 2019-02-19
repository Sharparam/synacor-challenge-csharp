namespace Sharparam.SynacorChallenge.VM
{
    public class NullInputReader : IInputReader
    {
        public ushort Read() => 0;
    }
}
