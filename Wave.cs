internal class Wave
{
    public static Wave WithDuration
    (double duration = 1, int channels = 2, int frameRate = 48_000)
    {
        int frames = MzMath.TimeToIndex(duration, frameRate);
        return new(new double[frames * channels], frames, channels, frameRate);
    }
    public int FrameRate { get; }
    public int Channels { get; }
    public int Frames { get; }
    public double this[int index]
    {
        get => samples[index];
        set => samples[index] = value;
    }
    public double this[int channel, int frame]
    {
        get => samples[Index(channel, frame)];
        set => samples[Index(channel, frame)] = value;
    }
    public int Index(int channel, int frame)
    {
        return frame * Channels + channel;
    }
    private Wave
    (double[] samples, int frames, int channels = 2, int frameRate = 48_000)
    {
        this.samples = samples;
        Frames = frames;
        Channels = channels;
        FrameRate = frameRate;
    }
    private readonly double[] samples;

}