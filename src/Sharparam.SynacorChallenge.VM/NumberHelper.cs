namespace Sharparam.SynacorChallenge.VM
{
    using System.Runtime.CompilerServices;

    public static class NumberHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Low(ushort number) => (byte)(number >> 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte High(ushort number) => (byte)(number & 0xFF);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (byte Low, byte High) ExtractLowHigh(ushort number) => (Low(number), High(number));
    }
}
