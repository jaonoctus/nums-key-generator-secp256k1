using Cocona;
using NUMS.Core;
using Org.BouncyCastle.Math;

CoconaApp.Run<NumsCommands>(args);

public class NumsCommands
{
    [Command("generate")]
    public void Generate(
        [Option("input", Description = "Input string to generate NUMS point from")] string input = "unspendable")
    {
        var (numsPoint, r) = NumsGenerator.FromString(input);
        var xCoord = numsPoint.XCoord;
        Console.WriteLine($"NUMS PK: {BitConverter.ToString(xCoord.ToBigInteger().ToByteArrayUnsigned()).Replace("-", "")}");
        Console.WriteLine($"R: {BitConverter.ToString(r).Replace("-", "")}");
    }

    [Command("verify")]
    public void Verify(
        [Option("pk", Description = "Public key in hex format")] string x,
        [Option("r", Description = "Scalar R in hex format")] string r)
    {
        try
        {
            BigInteger xCoord = new BigInteger(x, 16);

            byte[] scalarR = new byte[r.Length / 2];
            for (int i = 0; i < scalarR.Length; i++)
            {
                scalarR[i] = Convert.ToByte(r.Substring(i * 2, 2), 16);
            }

            bool isValid = NumsGenerator.VerifyPoint(xCoord, scalarR);
            Console.WriteLine($"Verification result: {(isValid ? "\u2705 Nothing Up My Sleeve Public Key" : "Invalid")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during verification: {ex.Message}");
            Environment.Exit(1);
        }
    }
}