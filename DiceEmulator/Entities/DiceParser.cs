namespace DiceEmulator.Entities;

public static class DiceParser
{
    public static List<Dice> ParseDice(string[] args)
    {
        ValidateArgumentCount(args);
        return args.Select((arg, index) => ParseDiceFromString(arg, index)).ToList();
    }

    private static void ValidateArgumentCount(string[] args)
    {
        if (args.Length < 3)
        {
            throw new ArgumentException(
                "At least 3 dice must be provided (each with 6 faces). " +
                "Example: 1,2,3,4,5,6 6,5,4,3,2,1 4,4,4,4,4,4");
        }
    }

    private static Dice ParseDiceFromString(string diceString, int index)
    {
        string[] faceValues = SplitDiceString(diceString);
        ValidateFaceCount(faceValues, index);
        int[] faces = ParseFaceValues(faceValues, diceString, index);
        return new Dice(index, faces);
    }

    private static string[] SplitDiceString(string diceString)
    {
        return diceString.Split(',');
    }

    private static void ValidateFaceCount(string[] faceValues, int diceIndex)
    {
        if (faceValues.Length != 6)
        {
            throw new ArgumentException(
                $"Dice {diceIndex + 1} must have exactly 6 faces. " +
                "Example: 1,2,3,4,5,6");
        }
    }

    private static int[] ParseFaceValues(string[] faceValues, string diceString, int diceIndex)
    {
        try
        {
            return faceValues.Select(int.Parse).ToArray();
        }
        catch (FormatException)
        {
            throw new ArgumentException(
                $"Dice {diceIndex + 1} contains non-integer values: {diceString}");
        }
    }
}