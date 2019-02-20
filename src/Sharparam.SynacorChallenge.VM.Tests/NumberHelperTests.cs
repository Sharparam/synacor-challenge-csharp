namespace Sharparam.SynacorChallenge.VM.Tests
{
    using NUnit.Framework;

    public class NumberHelperTests
    {
        [Test]
        public void ShouldExtractLowByte()
        {
            ushort number = 0x1234;
            var (low, _) = NumberHelper.ExtractLowHigh(number);
            Assert.That(low, Is.EqualTo(0x12));
        }

        [Test]
        public void ShouldExtractHighByte()
        {
            ushort number = 0x1234;
            var (_, high) = NumberHelper.ExtractLowHigh(number);
            Assert.That(high, Is.EqualTo(0x34));
        }
    }
}
