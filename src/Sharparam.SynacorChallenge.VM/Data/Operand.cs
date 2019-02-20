namespace Sharparam.SynacorChallenge.VM.Data
{
    using System;

    [Serializable]
    public readonly struct Operand
    {
        public const ushort MaxValue = 32775;

        public Operand(ushort value) => Value = value;

        public ushort Value { get; }
    }
}
