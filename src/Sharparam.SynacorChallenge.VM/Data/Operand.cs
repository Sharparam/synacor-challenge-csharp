namespace Sharparam.SynacorChallenge.VM.Data
{
    using System;

    [Serializable]
    public readonly struct Operand : IEquatable<Operand>, IEquatable<ushort>, IComparable<Operand>, IComparable<ushort>
    {
        public const ushort MaxValue = 32775;

        public Operand(ushort value)
            : this() =>
            Value = value;

        public ushort Value { get; }

        public static implicit operator Operand(ushort value) => new Operand(value);

        public static implicit operator ushort(Operand operand) => operand.Value;

        public static bool operator ==(Operand left, Operand right) => left.Equals(right);

        public static bool operator !=(Operand left, Operand right) => !(left == right);

        public static bool operator ==(Operand left, ushort right) => left.Equals(right);

        public static bool operator !=(Operand left, ushort right) => !(left == right);

        public static bool operator ==(ushort left, Operand right) => left.Equals(right);

        public static bool operator !=(ushort left, Operand right) => !(left == right);

        public static bool operator <(Operand left, Operand right) => left.CompareTo(right) < 0;

        public static bool operator >(Operand left, Operand right) => left.CompareTo(right) > 0;

        public static bool operator <(Operand left, ushort right) => left.CompareTo(right) < 0;

        public static bool operator >(Operand left, ushort right) => left.CompareTo(right) > 0;

        public static bool operator <(ushort left, Operand right) => left.CompareTo(right) < 0;

        public static bool operator >(ushort left, Operand right) => left.CompareTo(right) > 0;

        public static bool operator <=(Operand left, Operand right) => left.CompareTo(right) <= 0;

        public static bool operator >=(Operand left, Operand right) => left.CompareTo(right) >= 0;

        public static bool operator <=(Operand left, ushort right) => left.CompareTo(right) <= 0;

        public static bool operator >=(Operand left, ushort right) => left.CompareTo(right) >= 0;

        public static bool operator <=(ushort left, Operand right) => left.CompareTo(right) <= 0;

        public static bool operator >=(ushort left, Operand right) => left.CompareTo(right) >= 0;

        public bool Equals(Operand other) => Value == other.Value;

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
                    return obj is Operand other && Equals(other);
            }
        }

        public override int GetHashCode() => Value.GetHashCode();

        public int CompareTo(Operand other) => Value.CompareTo(other.Value);

        public int CompareTo(ushort other) => Value.CompareTo(other);
    }
}
