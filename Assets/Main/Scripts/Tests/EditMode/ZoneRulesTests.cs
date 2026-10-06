using NUnit.Framework;
using Vertigo.TestCase.Core;

namespace Vertigo.TestCase.Tests
{
    public sealed class ZoneRulesTests
    {
        private readonly ZoneRules rules = new ZoneRules(); // defaults: 60 zones, safe every 5, super every 30

        [TestCase(1, ZoneType.Safe)]
        [TestCase(2, ZoneType.Normal)]
        [TestCase(5, ZoneType.Safe)]
        [TestCase(10, ZoneType.Safe)]
        [TestCase(29, ZoneType.Normal)]
        [TestCase(30, ZoneType.Super)]
        [TestCase(55, ZoneType.Safe)]
        [TestCase(60, ZoneType.Super)]
        public void GetZoneType_ReturnsExpectedType(int zone, ZoneType expected)
        {
            Assert.AreEqual(expected, rules.GetZoneType(zone));
        }

        [TestCase(0)]
        [TestCase(61)]
        public void GetZoneType_OutOfRange_Throws(int zone)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => rules.GetZoneType(zone));
        }

        [Test]
        public void FindNextZone_FindsNextSuperOrNone()
        {
            Assert.AreEqual(30, rules.FindNextZone(1, ZoneType.Super));
            Assert.AreEqual(60, rules.FindNextZone(30, ZoneType.Super));
            Assert.AreEqual(-1, rules.FindNextZone(60, ZoneType.Super));
        }
    }
}
