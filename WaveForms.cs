internal delegate double WaveForm(double part);
internal static class WaveForms
{
    public static double Sin(double part)
    {
        return Math.Sin(2 * Math.PI * part);
    }
}