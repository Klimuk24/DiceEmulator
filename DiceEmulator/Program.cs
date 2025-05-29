using DiceEmulator.Entities;
using DiceEmulator.Game;

namespace DiceEmulator;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var dice = DiceParser.ParseDice(args);
            var game = new DiceGame(dice);
            game.Play();
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Usage: NonTransitiveDice.exe <dice1> <dice2> <dice3> [dice4...]");
            Console.WriteLine("Each dice should be 6 comma-separated integers, e.g. 1,2,3,4,5,6");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}