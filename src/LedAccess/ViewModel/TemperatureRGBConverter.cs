namespace LedAccess.ViewModel
{
    using System.Windows.Media;

    public class TemperatureRGBConverter
    {
        public static Color KelvinToRGB(double temp)
        {
            float x = (float)(temp / 1000.0);
            float x2 = x * x;
            float x3 = x2 * x;
            float x4 = x3 * x;
            float x5 = x4 * x;

            float R, G, B = 0f;

            // red
            if (temp <= 6600)
                R = 1f;
            else
                R = 0.0002889f * x5 - 0.01258f * x4 + 0.2148f * x3 - 1.776f * x2 + 6.907f * x - 8.723f;

            // green
            if (temp <= 6600)
                G = -4.593e-05f * x5 + 0.001424f * x4 - 0.01489f * x3 + 0.0498f * x2 + 0.1669f * x - 0.1653f;
            else
                G = -1.308e-07f * x5 + 1.745e-05f * x4 - 0.0009116f * x3 + 0.02348f * x2 - 0.3048f * x + 2.159f;

            // blue
            if (temp <= 2000f)
                B = 0f;
            else if (temp < 6600f)
                B = 1.764e-05f * x5 + 0.0003575f * x4 - 0.01554f * x3 + 0.1549f * x2 - 0.3682f * x + 0.2386f;
            else
                B = 1f;

            return Color.FromScRgb(1f, R, G, B);
        }
    }
}
