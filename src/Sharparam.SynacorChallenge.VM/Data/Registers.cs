namespace Sharparam.SynacorChallenge.VM.Data
{
    using System;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    [Serializable]
    public class Registers
    {
        public const int Length = 8;

        [JsonProperty("registers")]
        private readonly ushort[] _registers;

        public Registers() => _registers = new ushort[Length];

        public Registers(ushort[] data)
        {
            if (data.Length != Length)
            {
                throw new ArgumentException($"Registers array must be of size {Length}", nameof(data));
            }

            _registers = new ushort[Length];
            data.CopyTo(_registers, 0);
        }

        public ushort this[int index]
        {
            get => _registers[ResolveIndex(index)];
            set => _registers[ResolveIndex(index)] = value;
        }

        public Registers Copy() => new Registers(_registers);

        public void Reset()
        {
            for (var i = 0; i < Length; i++)
            {
                _registers[i] = 0;
            }
        }

        private static int ResolveIndex(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Index cannot be negative");

            if (value < Length)
                return value;

            if (value >= Constants.Register1 && value <= Constants.Register8)
                return value - Constants.Register1;

            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"Register index must be within [0,{Length}) or [{Constants.Register1},{Constants.Register8}]");
        }
    }
}
