using System.Text;
using DiceEmulator.Entities;

namespace DiceEmulator.Services;

public class ProbabilityTableGenerator
{
    public static string GenerateProbabilityTable(List<Dice> dice)
    {
        var table = new StringBuilder();
        int diceCount = dice.Count;

        // Calculate column widths
        int[] columnWidths = new int[diceCount + 1];
        columnWidths[0] = "Dice \\ vs >".Length;
        for (int i = 0; i < diceCount; i++)
        {
            columnWidths[i + 1] = dice[i].ToString().Length;
        }

        // Header
        table.Append("Dice \\ vs >".PadRight(columnWidths[0]));
        for (int i = 0; i < diceCount; i++)
        {
            table.Append(" | " + dice[i].ToString().PadRight(columnWidths[i + 1]));
        }
        table.AppendLine();

        // Separator
        table.Append(new string('-', columnWidths[0]));
        for (int i = 0; i < diceCount; i++)
        {
            table.Append("-+-" + new string('-', columnWidths[i + 1]));
        }
        table.AppendLine();

        // Rows
        for (int i = 0; i < diceCount; i++)
        {
            table.Append(dice[i].ToString().PadRight(columnWidths[0]));
            for (int j = 0; j < diceCount; j++)
            {
                if (i == j)
                {
                    table.Append(" | " + new string('-', columnWidths[j + 1]));
                }
                else
                {
                    double probability = ProbabilityCalculator.CalculateWinProbability(dice[i], dice[j]);
                    table.Append($" | {probability:P1}".PadRight(columnWidths[j + 1] + 3));
                }
            }
            table.AppendLine();
        }

        return table.ToString();
    }
}