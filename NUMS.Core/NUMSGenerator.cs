using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Utilities;
using ECPoint = Org.BouncyCastle.Math.EC.ECPoint;

namespace NUMS.Core;

public static class NumsGenerator
{
    private static readonly X9ECParameters _curve;
    private static readonly ECDomainParameters _domainParams;
    private static readonly BigInteger _n; // curve order

    static NumsGenerator()
    {
        _curve = ECNamedCurveTable.GetByName("secp256k1");
        _domainParams = new ECDomainParameters(
            _curve.Curve, _curve.G, _curve.N, _curve.H, _curve.GetSeed());
        _n = _curve.N;
    }

    public static (ECPoint numsPoint, byte[] r) FromString(string input)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentException("Input string cannot be null or empty");

        // Get the original generator point G in uncompressed format
        byte[] gPointEncoded = _domainParams.G.GetEncoded(false);

        using var sha256 = SHA256.Create();

        // Get H point by hashing G
        byte[] hash = sha256.ComputeHash(gPointEncoded);
        BigInteger xCoord = new BigInteger(1, hash);

        // Generate H point from x coordinate
        ECPoint h = _curve.Curve.DecodePoint(
            Arrays.Concatenate([0x02],
            BigIntegers.AsUnsignedByteArray(32, xCoord)));

        // Generate scalar from input string
        byte[] inputHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        BigInteger r = new BigInteger(1, inputHash).Mod(_n);

        // Calculate rG and H + rG
        ECPoint rG = _domainParams.G.Multiply(r);
        ECPoint numsPoint = h.Add(rG).Normalize();

        return (numsPoint, inputHash);
    }

    public static bool VerifyPoint(BigInteger x, byte[] scalarR)
    {
        BigInteger r = new BigInteger(1, scalarR).Mod(_n);

        // Reconstruct H point
        byte[] gPointEncoded = _domainParams.G.GetEncoded(false);
        using var sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(gPointEncoded);
        BigInteger xCoord = new BigInteger(1, hash);

        // Generate H point
        ECPoint h = _curve.Curve.DecodePoint(
            Arrays.Concatenate([0x02],
                BigIntegers.AsUnsignedByteArray(32, xCoord)));

        // Calculate expected point
        ECPoint rG = _domainParams.G.Multiply(r);
        ECPoint expectedPoint = h.Add(rG).Normalize();

        // Compare only x-coordinates
        return x.Equals(expectedPoint.AffineXCoord.ToBigInteger());
    }
}