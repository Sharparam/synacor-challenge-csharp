namespace Sharparam.SynacorChallenge.VM.Data
{
    public readonly struct Operand
    {
        public const ushort MaxValue = 32775;

        public Operand(ushort value)
        {
            Value = value;
        }

        public ushort Value { get; }
    }
}
