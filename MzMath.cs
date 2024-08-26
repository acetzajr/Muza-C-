internal class MzMath
{
    public static int TimeToIndex(double time, int frameRate)
    {
        return (int)(time * frameRate);
    }
    public static double IndexToTime(int index, int frameRate)
    {
        return index / (double)frameRate;
    }
}