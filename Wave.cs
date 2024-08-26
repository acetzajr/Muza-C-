using System.Buffers.Binary;

internal class Wave
{
    public static Wave WithDuration
    (double duration = 1, int channels = 2, int frameRate = 48_000)
    {
        int frames = MzMath.TimeToIndex(duration, frameRate);
        return new(new double[frames * channels], frames, channels, frameRate);
    }
    public int FrameRate { get; }
    public int ChannelsCount { get; }
    public int FramesCount { get; }
    public int SamplesCount { get; }
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
        return frame * ChannelsCount + channel;
    }
    private Wave
    (double[] samples, int frames, int channels = 2, int frameRate = 48_000)
    {
        this.samples = samples;
        FramesCount = frames;
        ChannelsCount = channels;
        FrameRate = frameRate;
        SamplesCount = frames * channels;
    }
    private readonly double[] samples;
    public Wave Save(string path = "out/wave.wav")
    {
        //File.Delete(path);
        int bytesPerSample = 8;
        short bytesPerBlock = (short)(ChannelsCount * bytesPerSample);
        int dataSize = SamplesCount * bytesPerSample;

        byte[] bytes4 = [0x52, 0x49, 0x46, 0x46];
        byte[] bytes2 = new byte[2];

        using
        (FileStream fileStream = new(path, FileMode.Create, FileAccess.Write))
        {

            // [Master RIFF chunk]
            // FileTypeBlocID  (4 bytes) : Identifier « RIFF »  (0x52, 0x49, 0x46, 0x46)
            fileStream.Write(bytes4, 0, 4);
            // FileSize        (4 bytes) : Overall file size minus 8 bytes
            // (4 + 4 + 4) + (4 + 4 + 2 + 2 + 4 + 4 + 2 + 2) + (4 + 4) + dataSize - 8
            int fileSize = 36 + dataSize;
            BinaryPrimitives.WriteInt32LittleEndian(bytes4, fileSize);
            fileStream.Write(bytes4, 0, 4);
            // FileFormatID    (4 bytes) : Format = « WAVE »  (0x57, 0x41, 0x56, 0x45)
            bytes4 = [0x57, 0x41, 0x56, 0x45];
            fileStream.Write(bytes4, 0, 4);

            // [Chunk describing the data format]
            // FormatBlocID    (4 bytes) : Identifier « fmt␣ »  (0x66, 0x6D, 0x74, 0x20)
            bytes4 = [0x66, 0x6D, 0x74, 0x20];
            fileStream.Write(bytes4, 0, 4);
            // BlocSize        (4 bytes) : Chunk size minus 8 bytes, which is 16 bytes here  (0x10)
            int blockSize = 16;
            BinaryPrimitives.WriteInt32LittleEndian(bytes4, blockSize);
            fileStream.Write(bytes4, 0, 4);
            // AudioFormat     (2 bytes) : Audio format (1: PCM integer, 3: IEEE 754 float)
            short format = 3;
            BinaryPrimitives.WriteInt16LittleEndian(bytes2, format);
            fileStream.Write(bytes2, 0, 2);
            // NbrChannels     (2 bytes) : Number of channels
            short channels = (short)ChannelsCount;
            BinaryPrimitives.WriteInt16LittleEndian(bytes2, channels);
            fileStream.Write(bytes2, 0, 2);
            // Frequence       (4 bytes) : Sample rate (in hertz)
            int frameRate = FrameRate;
            BinaryPrimitives.WriteInt32LittleEndian(bytes4, frameRate);
            fileStream.Write(bytes4, 0, 4);
            // BytePerSec      (4 bytes) : Number of bytes to read per second (Frequence * BytePerBloc).
            int bytesPerSec = FrameRate * bytesPerBlock;
            BinaryPrimitives.WriteInt32LittleEndian(bytes4, bytesPerSec);
            fileStream.Write(bytes4, 0, 4);
            // BytePerBloc     (2 bytes) : Number of bytes per block (NbrChannels * BitsPerSample / 8).
            BinaryPrimitives.WriteInt16LittleEndian(bytes2, bytesPerBlock);
            fileStream.Write(bytes2, 0, 2);
            // BitsPerSample   (2 bytes) : Number of bits per sample
            short bitsPerSample = (short)(bytesPerSample * 8);
            BinaryPrimitives.WriteInt16LittleEndian(bytes2, bitsPerSample);
            fileStream.Write(bytes2, 0, 2);

            // [Chunk containing the sampled data]
            // DataBlocID      (4 bytes) : Identifier « data »  (0x64, 0x61, 0x74, 0x61)
            bytes4 = [0x64, 0x61, 0x74, 0x61];
            fileStream.Write(bytes4, 0, 4);
            // DataSize        (4 bytes) : SampledData size
            BinaryPrimitives.WriteInt32LittleEndian(bytes4, dataSize);
            fileStream.Write(bytes4, 0, 4);
            // SampledData
            byte[] sampleBytes = new byte[bytesPerSample];
            for (int sample = 0; sample < SamplesCount; sample++)
            {
                BinaryPrimitives.WriteDoubleLittleEndian
                (sampleBytes, samples[sample]);
                fileStream.Write(sampleBytes, 0, bytesPerSample);
            }
        }
        return this;
    }

}