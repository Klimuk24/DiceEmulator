namespace DiceEmulator.Entities;

public static class DiceParser
{
    public static List<Dice> ParseDice(string[] args)
    {
        if (args.Length < 3)
        {
            throw new ArgumentException("At least 3 dice must be provided (each with 6 faces). Example: 1,2,3,4,5,6 6,5,4,3,2,1 4,4,4,4,4,4");
        }

        var dice = new List<Dice>();
        for (int i = 0; i < args.Length; i++)
        {
            var faceValues = args[i].Split(',');
            if (faceValues.Length != 6)
            {
                throw new ArgumentException($"Dice {i + 1} must have exactly 6 faces. Example: 1,2,3,4,5,6");
            }

            try
            {
                var faces = faceValues.Select(int.Parse).ToArray();
                dice.Add(new Dice(i, faces));
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Dice {i + 1} contains non-integer values: {args[i]}");
            }
        }

        return dice;
    }
}