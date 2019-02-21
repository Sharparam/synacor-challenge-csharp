namespace Sharparam.SynacorChallenge.VM.Data
{
    using System;
    using System.ComponentModel.Design.Serialization;

    using Newtonsoft.Json;

    [Serializable]
    public class Memory
    {
        private const ushort MemorySize = 0x7FFF;

        [JsonProperty("data")]
        private readonly ushort[] _data;

        public Memory() => _data = new ushort[MemorySize];

        [JsonConstructor]
        public Memory(ushort[] data)
        {
            if (data.Length != MemorySize)
            {
                throw new ArgumentException($"Memory array must be of size {MemorySize}", nameof(data));
            }

            _data = new ushort[MemorySize];
            data.CopyTo(_data, 0);
        }

        public ushort this[int index]
        {
            get => _data[ResolveIndex(index)];
            set => _data[ResolveIndex(index)] = value;
        }

        public Memory Copy() => new Memory(_data);

        public void Clear()
        {
            for (var i = 0; i < MemorySize; i++)
            {
                _data[i] = 0;
            }
        }

        private static int ResolveIndex(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Index cannot be negative");
            }

            if (value >= MemorySize)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Index must be less than {MemorySize}");
            }

            return value;
        }
    }
}
