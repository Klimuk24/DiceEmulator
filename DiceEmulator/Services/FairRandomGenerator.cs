using System.Security.Cryptography;

namespace DiceEmulator.Services;

public class FairRandomGenerator
{
    private readonly byte[] _key;
    private readonly int _computerNumber;

    public FairRandomGenerator(int minValue, int maxValue)
    {
        if (minValue >= maxValue) throw new ArgumentException("minValue must be less than maxValue");

        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        _key = new byte[32]; 
        rng.GetBytes(_key);

        _computerNumber = GenerateUniformRandomNumber(minValue, maxValue, _key);
    }

    private int GenerateUniformRandomNumber(int minValue, int maxValue, byte[] key)
    {
        uint range = (uint)(maxValue - minValue + 1);

        using var hmac = new HMACSHA256(key);
        byte[] randomBytes;
        uint randomValue;
        
        do
        {
            randomBytes = new byte[4];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            randomValue = BitConverter.ToUInt32(randomBytes, 0);
        } while (randomValue > uint.MaxValue - (uint.MaxValue % range));

        return minValue + (int)(randomValue % range);
    }

    public string GetHmac() => BitConverter.ToString(new HMACSHA256(_key).ComputeHash(BitConverter.GetBytes(_computerNumber))).Replace("-", "");

    public string GetKey() => BitConverter.ToString(_key).Replace("-", "");

    public int GetComputerNumber() => _computerNumber;

    public static int CalculateFairResult(int computerNumber, int userNumber, int modulo) => (computerNumber + userNumber) % modulo;
}