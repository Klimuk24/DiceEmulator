using System.Security.Cryptography;

namespace DiceEmulator.Services;

public class RandomNumberGenerator
{
    private readonly byte[] _key;
    private readonly int _computerNumber;

    public RandomNumberGenerator(int minValue, int maxValue)
    {
        ValidateRange(minValue, maxValue);
        _key = GenerateCryptographicKey();
        _computerNumber = GenerateUniformRandomNumber(minValue, maxValue, _key);
    }

    private static void ValidateRange(int minValue, int maxValue)
    {
        if (minValue >= maxValue)
            throw new ArgumentException("minValue must be less than maxValue");
    }

    private static byte[] GenerateCryptographicKey()
    {
        var key = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(key);
        return key;
    }

    private int GenerateUniformRandomNumber(int minValue, int maxValue, byte[] key)
    {
        uint range = CalculateRange(minValue, maxValue);
        uint randomValue = GenerateUniformRandomValue(range, key);
        return minValue + (int)(randomValue % range);
    }

    private static uint CalculateRange(int minValue, int maxValue) 
        => (uint)(maxValue - minValue + 1);

    private uint GenerateUniformRandomValue(uint range, byte[] key)
    {
        using var hmac = new HMACSHA256(key);
        uint randomValue;
        do
        {
            randomValue = GenerateRandomUInt32();
        } while (IsBiasedValue(randomValue, range));
        return randomValue;
    }

    private static uint GenerateRandomUInt32()
    {
        var randomBytes = new byte[4];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return BitConverter.ToUInt32(randomBytes, 0);
    }
    
    public static int CalculateFairResult(int computerNumber, int userNumber, int modulo) 
        => (computerNumber + userNumber) % modulo;
    
    public string GetHmac() 
        => BitConverter.ToString(new HMACSHA256(_key).ComputeHash(BitConverter.GetBytes(_computerNumber)))
            .Replace("-", "");

    private static bool IsBiasedValue(uint value, uint range) => value > uint.MaxValue - (uint.MaxValue % range);
    
    public string GetKey() => BitConverter.ToString(_key).Replace("-", "");

    public int GetComputerNumber() => _computerNumber;
}