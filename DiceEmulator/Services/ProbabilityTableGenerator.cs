using System.Text;
using DiceEmulator.Entities;

namespace DiceEmulator.Services;

public class ProbabilityTableGenerator
{
    public static string GenerateProbabilityTable(List<Dice> dice)
    {
        var table = new StringBuilder();
        int[] columnWidths = CalculateColumnWidths(dice);
        AppendHeader(table, dice, columnWidths);
        AppendSeparator(table, columnWidths);
        AppendProbabilityRows(table, dice, columnWidths);
        return table.ToString();
    }

    private static int[] CalculateColumnWidths(List<Dice> dice)
    {
        var widths = new int[dice.Count + 1];
        widths[0] = "Dice \\ vs >".Length;
        for (int i = 0; i < dice.Count; i++)
        {
            widths[i + 1] = dice[i].ToString().Length;
        }
        return widths;
    }

    private static void AppendHeader(StringBuilder table, List<Dice> dice, int[] columnWidths)
    {
        table.Append("Dice \\ vs >".PadRight(columnWidths[0]));
        for (int i = 0; i < dice.Count; i++)
        {
            table.Append($" | {dice[i].ToString().PadRight(columnWidths[i + 1])}");
        }
        table.AppendLine();
    }

    private static void AppendSeparator(StringBuilder table, int[] columnWidths)
    {
        table.Append(new string('-', columnWidths[0]));
        for (int i = 0; i < columnWidths.Length - 1; i++)
        {
            table.Append($"-+-{new string('-', columnWidths[i + 1])}");
        }
        table.AppendLine();
    }

    private static void AppendProbabilityRows(StringBuilder table, List<Dice> dice, int[] columnWidths)
    {
        for (int i = 0; i < dice.Count; i++)
        {
            AppendRow(table, dice, columnWidths, i);
        }
    }

    private static void AppendRow(StringBuilder table, List<Dice> dice, int[] columnWidths, int rowIndex)
    {
        table.Append(dice[rowIndex].ToString().PadRight(columnWidths[0]));
        for (int j = 0; j < dice.Count; j++)
        {
            AppendCell(table, dice, columnWidths, rowIndex, j);
        }
        table.AppendLine();
    }

    private static void AppendCell(StringBuilder table, List<Dice> dice, int[] columnWidths, int i, int j)
    {
        if (i == j)
        {
            table.Append($" | {new string('-', columnWidths[j + 1])}");
        }
        else
        {
            double probability = ProbabilityCalculator.CalculateWinProbability(dice[i], dice[j]);
            table.Append($" | {probability:P1}".PadRight(columnWidths[j + 1] + 3));
        }
    }
}