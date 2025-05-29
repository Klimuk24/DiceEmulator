using DiceEmulator.Entities;
using DiceEmulator.Services;

namespace DiceEmulator.Game;

public class DiceGame
{
    private readonly List<Dice> _dice;
        private Dice _computerDice;
        private Dice _playerDice;
        private bool _computerMovesFirst;

        public DiceGame(List<Dice> dice)
        {
            _dice = dice;
        }

        public void Play()
        {
            Console.WriteLine("Welcome to the Non-Transitive Dice Game!");
            Console.WriteLine("Available dice:");
            for (int i = 0; i < _dice.Count; i++)
            {
                Console.WriteLine($"{i} - {_dice[i]}");
            }
            Console.WriteLine();

            DetermineFirstMove();
            SelectDice();
            PlayRound();
        }

        private void DetermineFirstMove()
        {
            Console.WriteLine("Let's determine who makes the first move.");
            var fairRandom = new FairRandomGenerator(0, 1);
            Console.WriteLine($"I selected a random value in the range 0..1 (HMAC={fairRandom.GetHmac()}).");
            Console.WriteLine("Try to guess my selection.");
            Console.WriteLine("0 - 0");
            Console.WriteLine("1 - 1");
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help");
            Console.Write("Your selection: ");

            string input = Console.ReadLine()?.Trim().ToUpper();
            if (input == "X") Environment.Exit(0);
            if (input == "?")
            {
                ShowHelp();
                DetermineFirstMove();
                return;
            }

            if (!int.TryParse(input, out int userChoice) || (userChoice != 0 && userChoice != 1))
            {
                Console.WriteLine("Invalid input. Please enter 0, 1, X or ?.");
                DetermineFirstMove();
                return;
            }

            int computerNumber = fairRandom.GetComputerNumber();
            int result = FairRandomGenerator.CalculateFairResult(computerNumber, userChoice, 2);

            Console.WriteLine($"My selection: {computerNumber} (KEY={fairRandom.GetKey()}).");
            Console.WriteLine($"The fair number generation result is {computerNumber} + {userChoice} = {result} (mod 2).");

            _computerMovesFirst = result == 1;
            Console.WriteLine($"{( _computerMovesFirst ? "I" : "You")} make the first move.");
        }

        private void SelectDice()
        {
            if (_computerMovesFirst)
            {
                _computerDice = _dice[new Random().Next(_dice.Count)];
                Console.WriteLine($"I choose the {_computerDice} dice.");
                PlayerSelectDice();
            }
            else
            {
                PlayerSelectDice();
                ComputerSelectDice();
            }
        }

        private void PlayerSelectDice()
        {
            Console.WriteLine("Choose your dice:");
            for (int i = 0; i < _dice.Count; i++)
            {
                if (_computerDice == null || _dice[i].Id != _computerDice.Id)
                {
                    Console.WriteLine($"{i} - {_dice[i]}");
                }
            }
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help");
            Console.Write("Your selection: ");

            string input = Console.ReadLine()?.Trim().ToUpper();
            if (input == "X") Environment.Exit(0);
            if (input == "?")
            {
                ShowHelp();
                PlayerSelectDice();
                return;
            }

            if (!int.TryParse(input, out int choice) || choice < 0 || choice >= _dice.Count || 
                (_computerDice != null && _dice[choice].Id == _computerDice.Id))
            {
                Console.WriteLine("Invalid selection. Please choose an available dice.");
                PlayerSelectDice();
                return;
            }

            _playerDice = _dice[choice];
            Console.WriteLine($"You choose the {_playerDice} dice.");
        }

        private void ComputerSelectDice()
        {
            var availableDice = _dice.Where(d => d.Id != _playerDice.Id).ToList();
            _computerDice = availableDice[new Random().Next(availableDice.Count)];
            Console.WriteLine($"I choose the {_computerDice} dice.");
        }

        private void PlayRound()
        {
            int computerRoll = RollDice(_computerDice, "my");
            int playerRoll = RollDice(_playerDice, "your");

            Console.WriteLine();
            if (playerRoll > computerRoll)
            {
                Console.WriteLine($"You win ({playerRoll} > {computerRoll})!");
            }
            else if (computerRoll > playerRoll)
            {
                Console.WriteLine($"I win ({computerRoll} > {playerRoll})!");
            }
            else
            {
                Console.WriteLine($"It's a tie ({playerRoll} = {computerRoll})!");
            }
        }

        private int RollDice(Dice dice, string owner)
        {
            Console.WriteLine($"It's time for {owner} roll.");
            var fairRandom = new FairRandomGenerator(0, 5);
            Console.WriteLine($"I selected a random value in the range 0..5 (HMAC={fairRandom.GetHmac()}).");
            Console.WriteLine("Add your number modulo 6.");
            Console.WriteLine("0 - 0");
            Console.WriteLine("1 - 1");
            Console.WriteLine("2 - 2");
            Console.WriteLine("3 - 3");
            Console.WriteLine("4 - 4");
            Console.WriteLine("5 - 5");
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help");
            Console.Write("Your selection: ");

            string input = Console.ReadLine()?.Trim().ToUpper();
            if (input == "X") Environment.Exit(0);
            if (input == "?")
            {
                ShowHelp();
                return RollDice(dice, owner);
            }

            if (!int.TryParse(input, out int userChoice) || userChoice < 0 || userChoice > 5)
            {
                Console.WriteLine("Invalid input. Please enter a number between 0 and 5.");
                return RollDice(dice, owner);
            }

            int computerNumber = fairRandom.GetComputerNumber();
            int result = FairRandomGenerator.CalculateFairResult(computerNumber, userChoice, 6);

            Console.WriteLine($"My number is {computerNumber} (KEY={fairRandom.GetKey()}).");
            Console.WriteLine($"The fair number generation result is {computerNumber} + {userChoice} = {result} (mod 6).");

            int rollResult = dice.Faces[result];
            Console.WriteLine($"{owner} roll result is {rollResult}.");
            return rollResult;
        }

        private void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("HELP:");
            Console.WriteLine("This is a non-transitive dice game where different dice can beat each other in a non-transitive way.");
            Console.WriteLine("The game uses cryptographically secure fair random number generation to ensure neither player can cheat.");
            Console.WriteLine();
            Console.WriteLine("Here are the probabilities of each dice beating another:");
            Console.WriteLine(ProbabilityTableGenerator.GenerateProbabilityTable(_dice));
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.WriteLine();
        }
}