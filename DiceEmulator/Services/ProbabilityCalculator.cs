using DiceEmulator.Entities;

namespace DiceEmulator.Services;

public class ProbabilityCalculator
{
    public static double CalculateWinProbability(Dice dice1, Dice dice2)
    {
        int wins = CountWinningCases(dice1, dice2);
        int total = CalculateTotalComparisons(dice1, dice2);
        return (double)wins / total;
    }

    private static int CountWinningCases(Dice dice1, Dice dice2)
    {
        int wins = 0;
        foreach (var face1 in dice1.Faces)
        {
            foreach (var face2 in dice2.Faces)
            {
                if (face1 > face2) wins++;
            }
        }
        return wins;
    }

    private static int CalculateTotalComparisons(Dice dice1, Dice dice2)
    {
        return dice1.Faces.Length * dice2.Faces.Length;
    }
}