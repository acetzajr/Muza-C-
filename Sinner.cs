internal class Sinner
(
    double frequency = 360,
    double duration = 1,
    double amplitude = 1,
    int channels = 2,
    int frameRate = 48_000
)
{
    public double Frequency { get; set; } = frequency;
    public double Duration { get; set; } = duration;
    public double Amplitude { get; set; } = amplitude;
    public int Channels { get; set; } = channels;
    public int FrameRate { get; set; } = frameRate;
    public Wave Wave
    {
        get
        {
            Wave wave = Wave.WithDuration(Duration, Channels, FrameRate);
            for (int frame = 0; frame < wave.Frames; frame++)
            {
                double time = MzMath.IndexToTime(frame, wave.FrameRate);
                double part = time * Frequency % 1.0;
                double sample = WaveForms.Sin(part) * Amplitude;
                for (int channel = 0; channel < wave.Channels; channel++)
                {
                    wave[channel, frame] = sample;
                }
            }
            return wave;
        }
    }
}