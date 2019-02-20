namespace Sharparam.SynacorChallenge.VM.Data
{
    using System;

    [Serializable]
    public readonly struct Literal : IEquatable<Literal>, IEquatable<ushort>, IComparable<Literal>, IComparable<ushort>
    {
        public const ushort MaxValue = 32767;

        public const ushort Divisor = 32768;

        public Literal(ushort value)
            : this()
        {
            if (value > MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Value = value;
        }

        public static explicit operator Literal(ushort value) => new Literal(value);

        public static implicit operator ushort(Literal literal) => literal.Value;

        public static bool operator ==(Literal left, Literal right) => left.Equals(right);

        public static bool operator !=(Literal left, Literal right) => !(left == right);

        public static bool operator <(Literal left, Literal right) => left.CompareTo(right) < 0;

        public static bool operator >(Literal left, Literal right) => left.CompareTo(right) > 0;

        public static bool operator <=(Literal left, Literal right) => left.CompareTo(right) <= 0;

        public static bool operator >=(Literal left, Literal right) => left.CompareTo(right) >= 0;

        public static Literal operator +(Literal left, Literal right) => (Literal)(ushort)((left.Value + right.Value) % Divisor);

        public static Literal operator *(Literal left, Literal right) =>
            (Literal)(ushort)((left.Value * right.Value) % Divisor);

        public static Literal operator %(Literal left, Literal right) => (Literal)(ushort)(left.Value % right.Value);

        public static Literal operator &(Literal left, Literal right) => (Literal)(ushort)(left.Value & right.Value);

        public static Literal operator |(Literal left, Literal right) => (Literal)(ushort)(left.Value | right.Value);

        public static Literal operator ^(Literal left, Literal right) => (Literal)(ushort)(left.Value ^ right.Value);

        public static Literal operator ~(Literal operand) => (Literal)(ushort)(~operand.Value & ((1 << 15) - 1));

        public ushort Value { get; }

        public bool Equals(Literal other) => Value == other.Value;

        public bool Equals(ushort other) => Value == other;

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;

                case ushort @ushort:
                    return Equals(@ushort);

                default:
                    return obj is Literal other && Equals(other);
            }
        }

        public override int GetHashCode() => Value.GetHashCode();

        public int CompareTo(Literal other) => Value.CompareTo(other.Value);

        public int CompareTo(ushort other) => Value.CompareTo(other);
    }
}
