using System.Globalization;

namespace Vertigo.TestCase.UI
{
    public static class AmountFormatter
    {
        public static string Format(int amount)
        {
            if (amount >= 1_000_000)
                return Compact(amount / 1_000_000f, "M");
            if (amount >= 1_000)
                return Compact(amount / 1_000f, "K");

            return amount.ToString(CultureInfo.InvariantCulture);
        }

        // 1.5K, 15K, 150K: one decimal only while it still fits.
        private static string Compact(float value, string suffix) =>
            value.ToString(value >= 100f ? "0" : "0.#", CultureInfo.InvariantCulture) + suffix;
    }
}