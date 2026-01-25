namespace SUIM.StrideEngine.Sample;

using Stride.Engine;

/// <summary>
/// Entry point for the SUIM sample game.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        using (var game = new SUIMSampleGame())
        {
            game.Run();
        }
    }
}
