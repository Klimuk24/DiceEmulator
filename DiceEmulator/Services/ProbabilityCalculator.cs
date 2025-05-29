using DiceEmulator.Entities;

namespace DiceEmulator.Services;

public class ProbabilityCalculator
{
    public static double CalculateWinProbability(Dice dice1, Dice dice2)
    {
        int wins = 0;
        int total = 0;

        foreach (var face1 in dice1.Faces)
        {
            foreach (var face2 in dice2.Faces)
            {
                if (face1 > face2) wins++;
                total++;
            }
        }

        return (double)wins / total;
    }
}