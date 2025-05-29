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
            ShowWelcomeMessage();
            DetermineFirstMove();
            SelectDice();
            PlayRound();
        }

        private void ShowWelcomeMessage()
        {
            Console.WriteLine("Welcome to the Non-Transitive Dice Game!\nAvailable dice: ");
            for (int i = 0; i < _dice.Count; i++)
            {
                Console.WriteLine($"{i} - {_dice[i]}");
            }
        }

        private void ShowSelectionMessage()
        {
            Console.WriteLine("0 - 0");
            Console.WriteLine("1 - 1");
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help");
            Console.Write("Your selection:\n"); 
        }
        
        private void DetermineFirstMove()
        {
            DisplayFirstMoveIntroduction();
            var fairRandom = CreateRandomGenerator();
            DisplayHmac(fairRandom);
            PromptForUserGuess();
            string input = GetUserInput();
            HandleSpecialInputs(input, fairRandom);
            int userChoice = ParseUserChoice(input);
            int computerNumber = fairRandom.GetComputerNumber();
            DisplayResults(fairRandom, userChoice, computerNumber);
            DetermineFirstMoveResult(computerNumber, userChoice);
        }
        
        private void DisplayFirstMoveIntroduction()
        {
            Console.WriteLine("Let's determine who makes the first move.");
        }
        
        private RandomNumberGenerator CreateRandomGenerator()
        {
            return new RandomNumberGenerator(0, 1);
        }
        
        private void DisplayHmac(RandomNumberGenerator randomNumber)
        {
            Console.WriteLine($"I selected a random value in the range 0..1 (HMAC={randomNumber.GetHmac()}).");
            Console.WriteLine("Try to guess my selection.");
            ShowSelectionMessage();
        }
        
        private void PromptForUserGuess()
        {
            ShowSelectionMessage();
        }
        
        private string GetUserInput()
        {
            return Console.ReadLine()?.Trim().ToUpper() ?? throw new InvalidOperationException();
        }
        
        private void HandleSpecialInputs(string input, RandomNumberGenerator randomNumber)
        {
            if (input == "X") Environment.Exit(0);
            if (input == "?")
            {
                ShowHelp();
                DetermineFirstMove();
                return;
            }
        }
        
        private int ParseUserChoice(string input)
        {
            if (!int.TryParse(input, out int userChoice) || (userChoice != 0 && userChoice != 1))
            {
                Console.WriteLine("Invalid input. Please enter 0, 1, X or ?.");
                DetermineFirstMove();
                throw new InvalidOperationException("Invalid user input");
            }
            return userChoice;
        }
        
        private void DisplayResults(RandomNumberGenerator randomNumber, int userChoice, int computerNumber)
        {
            int result = RandomNumberGenerator.CalculateFairResult(computerNumber, userChoice, 2);
            Console.WriteLine($"My selection: {computerNumber} (KEY={randomNumber.GetKey()}).");
            Console.WriteLine($"The fair number generation result is {computerNumber} + {userChoice} = {result} (mod 2).");
        }
        
        private void DetermineFirstMoveResult(int computerNumber, int userChoice)
        {
            int result = RandomNumberGenerator.CalculateFairResult(computerNumber, userChoice, 2);
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
            DisplayAvailableDice();
            string input = GetPlayerInput();
            HandleSpecialInputs(input);
            ProcessDiceSelection(input);
        }

        private void DisplayAvailableDice()
        {
            Console.WriteLine("Choose your dice:");
            for (int i = 0; i < _dice.Count; i++)
            {
                if (IsDiceAvailable(_dice[i]))
                {
                    Console.WriteLine($"{i} - {_dice[i]}");
                }
            }
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help");
            Console.Write("Your selection: ");
        }

        private bool IsDiceAvailable(Dice dice)
        {
            return _computerDice == null || dice.Id != _computerDice.Id;
        }

        private string GetPlayerInput()
        {
            return Console.ReadLine()?.Trim().ToUpper() ?? throw new InvalidOperationException();
        }

        private void HandleSpecialInputs(string input)
        {
            if (input == "X") Environment.Exit(0);
            if (input == "?")
            {
                ShowHelp();
                PlayerSelectDice();
                return;
            }
        }

        private void ProcessDiceSelection(string input)
        {
            if (!TryParseDiceSelection(input, out int choice))
            {
                Console.WriteLine("Invalid selection. Please choose an available dice.");
                PlayerSelectDice();
                return;
            }

            _playerDice = _dice[choice];
            Console.WriteLine($"You choose the {_playerDice} dice.");
        }

        private bool TryParseDiceSelection(string input, out int choice)
        {
            choice = -1;
            return int.TryParse(input, out choice) && 
                   choice >= 0 && 
                   choice < _dice.Count && 
                   IsDiceAvailable(_dice[choice]);
        }

        private void ComputerSelectDice()
        {
            var availableDice = _dice.Where(d => d.Id != _playerDice.Id).ToList();
            _computerDice = availableDice[new Random().Next(availableDice.Count)];
            Console.WriteLine($"I choose the {_computerDice} dice.");
        }

        private void PlayRound()
        {
            int computerRoll = RollComputerDice();
            int playerRoll = RollPlayerDice();
    
            DisplayRollResults(computerRoll, playerRoll);
            DetermineAndDisplayRoundWinner(computerRoll, playerRoll);
        }

        private int RollComputerDice()
        {
            return RollDice(_computerDice, "my");
        }

        private int RollPlayerDice()
        {
            return RollDice(_playerDice, "your");
        }

        private void DisplayRollResults(int computerRoll, int playerRoll)
        {
            Console.WriteLine();
            Console.WriteLine($"Computer rolled: {computerRoll}");
            Console.WriteLine($"You rolled: {playerRoll}");
        }

        private void DetermineAndDisplayRoundWinner(int computerRoll, int playerRoll)
        {
            string resultMessage = GetRoundResultMessage(computerRoll, playerRoll);
            Console.WriteLine(resultMessage);
        }

        private string GetRoundResultMessage(int computerRoll, int playerRoll)
        {
            if (playerRoll > computerRoll)
            {
                return $"You win ({playerRoll} > {computerRoll})!";
            }
            else if (computerRoll > playerRoll)
            {
                return $"I win ({computerRoll} > {playerRoll})!";
            }
            else
            {
                return $"It's a Draw ({playerRoll} = {computerRoll})!";
            }
        }
        
        private int RollDice(Dice dice, string owner)
        {
            DisplayRollIntroduction(owner);
            var fairRandom = InitializeRandomGenerator();
            DisplayHmacPrompt(fairRandom);
            string input = GetUserRollInput();
            HandleSpecialInputs(input, dice, owner);
            int userChoice = ParseUserRollInput(input);
            int rollResult = CalculateAndDisplayRollResult(dice, owner, fairRandom, userChoice);
            return rollResult;
        }
        
        private void DisplayRollIntroduction(string owner)
        {
            Console.WriteLine($"It's time for {owner} roll.");
        }

        private RandomNumberGenerator InitializeRandomGenerator()
        {
            return new RandomNumberGenerator(0, 5);
        }
        
        private void DisplayHmacPrompt(RandomNumberGenerator randomNumber)
        {
            DisplayRandomValueInfo(randomNumber);
            DisplayInstructions();
            DisplayOptionsMenu();
            DisplayInputPrompt();
        }

        private void DisplayRandomValueInfo(RandomNumberGenerator randomNumber)
        {
            Console.WriteLine($"I selected a random value in the range 0..5 (HMAC={randomNumber.GetHmac()}).");
        }

        private void DisplayInstructions()
        {
            Console.WriteLine("Add your number modulo 6.");
        }

        private void DisplayOptionsMenu(int maxValue = 5)
        {
            for (int i = 0; i <= maxValue; i++)
            {
                Console.WriteLine($"{i} - {i}");
            }
            DisplaySpecialCommands();
        }

        private void DisplaySpecialCommands()
        {
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help");
        }

        private void DisplayInputPrompt()
        {
            Console.Write("Your selection: ");
        }

        private string GetUserRollInput()
        {
            return Console.ReadLine()?.Trim().ToUpper() ?? throw new InvalidOperationException();
        }
        
        private void HandleSpecialInputs(string input, Dice dice, string owner)
        {
            if (input == "X") Environment.Exit(0);
            if (input == "?")
            {
                ShowHelp();
                RollDice(dice, owner);
                return;
            }
        }

        private int ParseUserRollInput(string input)
        {
            if (!int.TryParse(input, out int userChoice) || userChoice < 0 || userChoice > 5)
            {
                Console.WriteLine("Invalid input. Please enter a number between 0 and 5.");
                throw new InvalidOperationException("Invalid user input");
            }
            return userChoice;
        }
        
        private int CalculateAndDisplayRollResult(Dice dice, string owner, RandomNumberGenerator randomNumber, int userChoice)
        {
            int computerNumber = randomNumber.GetComputerNumber();
            int result = RandomNumberGenerator.CalculateFairResult(computerNumber, userChoice, 6);
            DisplayRollCalculation(computerNumber, randomNumber, userChoice, result);
            int rollResult = dice.Faces[result];
            Console.WriteLine($"{owner} roll result is {rollResult}.");
            return rollResult;
        }

        private void DisplayRollCalculation(int computerNumber, RandomNumberGenerator randomNumber, int userChoice, int result)
        {
            Console.WriteLine($"My number is {computerNumber} (KEY={randomNumber.GetKey()}).");
            Console.WriteLine($"The fair number generation result is {computerNumber} + {userChoice} = {result} (mod 6).");
        }

        private void ShowHelp()
        {
            Console.WriteLine("\nHelp: \nThis is a non-transitive dice game where different dice can beat each other in a non-transitive way.");
            Console.WriteLine("The game uses cryptographically secure fair random number generation to ensure neither player can cheat.");
            ShowTableProbability();
            Console.ReadKey();
            Console.WriteLine();
        }

        private void ShowTableProbability()
        {
            Console.WriteLine("\nHere are the probabilities of each dice beating another:");
            Console.WriteLine(ProbabilityTableGenerator.GenerateProbabilityTable(_dice));
            Console.WriteLine("Press any key to continue...");
        }
}