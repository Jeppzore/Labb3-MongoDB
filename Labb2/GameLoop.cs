using Labb3_MongoDB;
using Labb3_MongoDB.Models;
using Labb3_MongoDB.MongoDB;
using Labb3_MongoDB.MongoDB.Entities;
using MongoDB.Driver;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;

class GameLoop()
{
    private readonly MongoDbService? _mongoDbService;
    private readonly GameManager? _gameManager;

    private Labb3_MongoDB.Models.Player? _player;
    private Labb3_MongoDB.MongoDB.Entities.Player? _players;

    private Labb3_MongoDB.Models.Rat? _rat;
    private Labb3_MongoDB.MongoDB.Entities.Rat? _rats;

    private Labb3_MongoDB.Models.Snake? _snake;
    private Labb3_MongoDB.MongoDB.Entities.Snake? _snakes;


    public int _numberOfTurns = 0;
    public int _totalEnemies = 0;
    private List<Enemy> _deadEnemies = new();

    public void Start()
    {
        Setup();

        GameLoopHelper.DisplayControls();

        while (_player!.IsAlive)
        {
            RunPlayerTurn();
            RunEnemiesTurn();
        }

        RestartGame();
    }

    private void RunPlayerTurn()
    {
        var hasWon = GameLoopHelper.IsWinCondition(_totalEnemies - _deadEnemies.Count <= 0);
        if (hasWon)
        {
            Start();
        }

        _player!.PlayerLevelCheck();

        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Name: {_player.Name}     Health: {_player.Health}/{_player.MaxHealth}    Level: {_player.Level}     Turns: {_numberOfTurns}".PadRight(Console.BufferWidth));
        Console.ResetColor();

        _player.Draw();

        LevelData.DrawElementsWithinRange(_player, _player.VisionRange);

        MovePlayer();

        _numberOfTurns++;
        _player.Turns = _numberOfTurns;
    }

    private void RunEnemiesTurn()
    {
        var enemies = LevelData.Elements.OfType<Enemy>().ToList();

        foreach (var enemy in enemies)
        {
            enemy.Update(_player!);
            if (enemy.IsDead)
            {
                _deadEnemies.Add(enemy);
            }
        }

        foreach (var enemy in _deadEnemies)
        {
            //_mongoDbService!.DeleteElements(enemy.Id!);
            LevelData.Elements.Remove(enemy);
            enemy.Clear();
        }
    }

    private void MovePlayer()
    {
        var key = Console.ReadKey(true).Key;

        _player!.Clear();

        switch (key)
        {
            case ConsoleKey.W: // Up
                if (_player.Position.Y > 0)
                {
                    _player.MoveToPosition(new Position(_player.Position.X, _player.Position.Y - 1));
                }
                break;

            case ConsoleKey.S: // Down
                if (_player.Position.Y < 18 - 1)
                {
                    _player.MoveToPosition(new Position(_player.Position.X, _player.Position.Y + 1));
                }
                break;

            case ConsoleKey.A: // Left
                if (_player.Position.X > 0)
                {
                    _player.MoveToPosition(new Position(_player.Position.X - 1, _player.Position.Y));
                }
                break;

            case ConsoleKey.D: // Right
                if (_player.Position.X < 53 - 1)
                {
                    _player.MoveToPosition(new Position(_player.Position.X + 1, _player.Position.Y));
                }
                break;

            case ConsoleKey.Escape: // ESC
                Console.Clear();
                Start();
                break;

            case ConsoleKey.Enter: // ENTER
                SaveAndExit();
                break;
        }
    }

    private void RestartGame()
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.WriteLine("Game over! Restarting game...");
        Thread.Sleep(3000);
        Console.Clear();
        Start();
    }

    private void Setup()
    {
        var mongoDbService = new MongoDbService();
        var gameManager = new GameManager(mongoDbService);

        Console.SetWindowSize(120, 35);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Welcome to Jesper & Robin's Dungeon Crawler!\n");

        LevelData.Load("Levels\\Level1.txt");

        var existingElements = mongoDbService.GetElements();

        if (existingElements.Count != 0)
        {
            Console.WriteLine("Existing Save file found. Loading...");
            Thread.Sleep(2000);

            gameManager.LoadElements(existingElements);

            _player = LevelData.Elements.OfType<Labb3_MongoDB.Models.Player>().FirstOrDefault(x => x.Type == ElementType.Player); //x => x.Type == ElementType.Player
            Debug.WriteLine($"Player loaded: {_player!.Name}");
            _numberOfTurns = _player!.Turns;

            var existingRats = LevelData.Elements.OfType<Labb3_MongoDB.Models.Rat>().ToList();
            Debug.WriteLine($"Rats loaded: {existingRats.Count}");

            var existingSnakes = LevelData.Elements.OfType<Labb3_MongoDB.Models.Snake>().ToList();
            Debug.WriteLine($"Snakes loaded: {existingSnakes.Count}");

            var discoveredWalls = LevelData.Elements.OfType<Wall>().Where(wall => wall.IsDiscovered).ToList();
            Debug.WriteLine($"Number of discovered walls: {discoveredWalls.Count}");

            foreach (var wall in discoveredWalls)
            {
                wall.Draw();
            }



            //CreateExistingPlayer(existingElements);

        }
        //if (existingRat != null)
        //{
        //    Console.SetCursorPosition(0, 19);
        //    Console.WriteLine("Existing rat found. Loading...");
        //    Thread.Sleep(1000);
        //    gameManager.LoadRatData(existingRat);
        //    CreateExistingRat(existingRat);
        //}
        else
        {
            Console.Write("Please enter your name (max 8 characters): ");
            CreateNewPlayer();
            //CreateNewRat();
            //CreateNewSnake();
            _player!.Turns = 0;
            _numberOfTurns = 0;
            Console.Clear();
        }

        _totalEnemies = LevelData.Elements.OfType<Enemy>().ToList().Count;
        _deadEnemies = new();

        Console.ResetColor(); //Console.Clear();
        Console.CursorVisible = false;
    }

    private void CreateNewPlayer()
    {
        var name = Console.ReadLine()!;
        if (name.Length <= 0 || name.Length > 8)
        {
            Console.Clear();
            Start();
        }

        _player = (Labb3_MongoDB.Models.Player)LevelData.Elements.FirstOrDefault(x => x.Type == ElementType.Player)!;
        _player.SetName(name);
    }

    private void CreateNewRat()
    {
        foreach (var element in LevelData.Elements.OfType<Labb3_MongoDB.Models.Rat>())
        {
            _rat = (Labb3_MongoDB.Models.Rat)LevelData.Elements.FirstOrDefault(x => x.Type == ElementType.Rat)!;
            Debug.WriteLine("Rat detected");
        }
    }

    private void CreateExistingPlayer(Labb3_MongoDB.MongoDB.Entities.Player existingPlayer)
    {
        _player = (Labb3_MongoDB.Models.Player)LevelData.Elements.FirstOrDefault(x => x.Type == ElementType.Player)!;
        _player.Id = existingPlayer.Id!;
        _player.SetName(existingPlayer.Name!);
        _player.Health = existingPlayer.Health;
        _player.MaxHealth = existingPlayer.MaxHealth;
        _player.Level = existingPlayer.Level;
        _player.Experience = existingPlayer.Experience;
        _player.VisionRange = existingPlayer.VisionRange;
        _player.AttackPower = existingPlayer.AttackPower;
        _player.DefenseStrength = existingPlayer.DefenseStrength;
        _player.Turns = existingPlayer.Turns;
        _player.IsMongo = existingPlayer.IsMongo;
        _player.Position = (Position)existingPlayer.CurrentLocation!;
    }

    private void CreateExistingRat(Labb3_MongoDB.MongoDB.Entities.Rat existingRat)
    {
        _rat = (Labb3_MongoDB.Models.Rat)LevelData.Elements.FirstOrDefault(x => x.Type == ElementType.Rat)!;
        _rat.Id = existingRat.Id!;
        _rat.Name = existingRat.Name!;
        _rat.Health = existingRat.Health;
        _rat.AttackPower = existingRat.AttackPower;
        _rat.DefenseStrength = existingRat.DefenseStrength;
        _rat.IsMongo = existingRat.IsMongo;
        _rat.Position = (Position)existingRat.CurrentLocation!;
    }

    public void SaveAndExit()
    {
        //SavePlayer();
        //SaveRat();
        //SaveSnake();
        SaveElements();

        SaveGameText();
        Environment.Exit(0);
    }

    private void SaveElements()
    {
        var mongoDbService = new MongoDbService();
        var gameManager = new GameManager(mongoDbService);

        var currentElements = LevelData.Elements.ToList();

        gameManager.SaveProgress(currentElements);

    }

    private void SavePlayer()
    {
        // If there is no existing saved game - create a new one in MongoDB
        if (_player!.IsMongo == false)
        {
            var mongoDbService = new MongoDbService();
            var gameManager = new GameManager(mongoDbService);

            var currentPlayer = new Labb3_MongoDB.MongoDB.Entities.Player
            {
                Id = Guid.NewGuid().ToString(), // Generates a new unique ID
                Name = _player!.Name,
                Health = _player.Health,
                MaxHealth = _player.MaxHealth,
                Level = _player.Level,
                Experience = _player.Experience,
                VisionRange = _player.VisionRange,
                AttackPower = _player.AttackPower,
                DefenseStrength = _player.DefenseStrength,
                Turns = _numberOfTurns,
                IsMongo = _player.IsMongo = true, // Mark as saved in MongoDB
                CurrentLocation = _player.Position,
                LastSaveTime = DateTime.UtcNow
            };

            //gameManager.SaveProgress(currentPlayer);
            _players = currentPlayer;
        }
        // Update existing saved game with the changed property values
        else
        {
            var mongoDbService = new MongoDbService();

            var updatedPlayer = new Labb3_MongoDB.MongoDB.Entities.Player
            {
                Id = _player.Id, // Use the existing player's ID
                Name = _player.Name,
                Health = _player.Health,
                MaxHealth = _player.MaxHealth,
                Level = _player.Level,
                Experience = _player.Experience,
                VisionRange = _player.VisionRange,
                AttackPower = _player.AttackPower,
                DefenseStrength = _player.DefenseStrength,
                Turns = _numberOfTurns,
                IsMongo = true,
                CurrentLocation = _player.Position,
                LastSaveTime = DateTime.UtcNow
            };

            //mongoDbService.SavePlayer(updatedPlayer);

            //SaveGameText();
            //Environment.Exit(0);
        }
    }

    private void SaveRat()
    {
        // If there is no existing saved game - create a new one in MongoDB
        if (_rat!.IsMongo == false)
        {
            var mongoDbService = new MongoDbService();
            var gameManager = new GameManager(mongoDbService);

            var currentRat = new Labb3_MongoDB.MongoDB.Entities.Rat
            {
                Id = Guid.NewGuid().ToString(), // Generates a new unique ID
                Name = _rat!.Name,
                Health = _rat.Health,
                AttackPower = _rat.AttackPower,
                DefenseStrength = _rat.DefenseStrength,
                IsMongo = _rat.IsMongo = true, // Mark as saved in MongoDB
                CurrentLocation = _rat.Position,
            };

            //gameManager.SaveProgress(currentRat);
            _rats = currentRat;
        }
        // Update existing saved game with the changed property values
        else
        {
            var mongoDbService = new MongoDbService();

            var updatedRat = new Labb3_MongoDB.MongoDB.Entities.Rat
            {
                Id = _rat.Id, // Use the existing rat's ID
                Name = _rat!.Name,
                Health = _rat.Health,
                AttackPower = _rat.AttackPower,
                DefenseStrength = _rat.DefenseStrength,
                IsMongo = _rat.IsMongo = true, // Mark as saved in MongoDB
                CurrentLocation = _rat.Position,
            };

            //mongoDbService.SaveRat(updatedRat);
        }
    }

    private void SaveSnake()
    {
        // If there is no existing saved game - create a new one in MongoDB
        if (_snake!.IsMongo == false)
        {
            var mongoDbService = new MongoDbService();
            var gameManager = new GameManager(mongoDbService);

            var currentSnake = new Labb3_MongoDB.MongoDB.Entities.Snake
            {
                Id = Guid.NewGuid().ToString(), // Generates a new unique ID
                Name = _snake!.Name,
                Health = _snake.Health,
                AttackPower = _snake.AttackPower,
                DefenseStrength = _snake.DefenseStrength,
                IsMongo = _snake.IsMongo = true, // Mark as saved in MongoDB
                CurrentLocation = _snake.Position,
            };

            //gameManager.SaveProgress(currentSnake);
            _snakes = currentSnake;
        }
        // Update existing saved game with the changed property values
        else
        {
            var mongoDbService = new MongoDbService();

            var updatedSnake = new Labb3_MongoDB.MongoDB.Entities.Snake
            {
                Id = _snake.Id, // Use the existing snake's ID
                Name = _snake!.Name,
                Health = _snake.Health,
                AttackPower = _snake.AttackPower,
                DefenseStrength = _snake.DefenseStrength,
                IsMongo = _snake.IsMongo = true, // Mark as saved in MongoDB
                CurrentLocation = _snake.Position,
            };

            //mongoDbService.SaveSnake(updatedSnake);
        }
    }

    public void SaveGameText()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(0, 28);
        Console.WriteLine($"Successfully saved game at turn: {_numberOfTurns}".PadRight(Console.BufferWidth));
        Console.ResetColor();
    }
}