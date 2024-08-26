internal class Program
{
    private static void Main()
    {
        Console.WriteLine("Hello, World!");
        Sinner sinner = new();
        sinner.Wave().Save();
        Console.WriteLine("Success!");
    }
}