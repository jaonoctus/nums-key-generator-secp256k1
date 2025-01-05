# NUMS secp256k1 Keys Generator and Verifier

This C# program provides functionality to generate and verify NUMS (Nothing Up My Sleeve) public keys using Cocona for CLI interactions.

## Usage

### Generate NUMS Public Key

```bash
dotnet run --project NUMS.Cli generate --input "unspendable"
```

### Verify NUMS Public Key

```bash
dotnet run --project NUMS.Cli verify --pk 08335B42143DD67DA2EC8CB8B9108777C351B47993BBA2A537A04BF72EB7396A --r 811C0DB9302E9EC042B3DEDB0C72361F08C866AEB26F38D4D959B205EF0A04B9
```
