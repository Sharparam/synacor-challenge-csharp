namespace Sharparam.SynacorChallenge.VM
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using JetBrains.Annotations;

    public class Program : IEnumerable<ushort>
    {
        private readonly ushort[] _data;

        public Program(ushort[] data) => _data = data;

        public int Length => _data.Length;

        public static Program FromFile([NotNull] string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));


            var bytes = File.ReadAllBytes(path);
            var data = new ushort[bytes.Length / 2];

            for (var i = 0; i < bytes.Length - 1; i += 2)
            {
                data[i / 2] = (ushort)(bytes[i] + (bytes[i + 1] << 8));
            }

            return new Program(data);
        }

        public IEnumerator<ushort> GetEnumerator() => ((IEnumerable<ushort>)_data).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
    }
}
