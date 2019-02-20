namespace Sharparam.SynacorChallenge.VM
{
    using System;
    using System.Runtime.CompilerServices;

    public static class NumberHelper
    {
        public const ushort ModuloDivisor = 32768;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Low(ushort number) => (byte)(number >> 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte High(ushort number) => (byte)(number & 0xFF);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (byte Low, byte High) ExtractLowHigh(ushort number) => (Low(number), High(number));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Equal(ushort a, ushort b) => (ushort)(a == b ? 1 : 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GreaterThan(ushort a, ushort b) => (ushort)(a > b ? 1 : 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Add(ushort a, ushort b) => (ushort)((a + b) % ModuloDivisor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Multiply(ushort a, ushort b) => (ushort)((a * b) % ModuloDivisor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Mod(ushort a, ushort b) => (ushort)(a % b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort And(ushort a, ushort b) => (ushort)(a & b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Or(ushort a, ushort b) => (ushort)(a | b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Not(ushort a, ushort b) => (ushort)(a ^ b);
    }
}
