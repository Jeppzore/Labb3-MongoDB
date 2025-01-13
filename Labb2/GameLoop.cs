using Elasticsearch.Net;
using Labb3_MongoDB;
using Labb3_MongoDB.Models;
using Labb3_MongoDB.MongoDB;
using Labb3_MongoDB.MongoDB.Entities;
using MongoDB.Driver;
using System.Diagnostics;

class GameLoop()
{
    private readonly MongoDbService? _mongoDbService;
    private readonly GameManager? _gameManager;

    private Labb3_MongoDB.Models.Player? _player;
    private Labb3_MongoDB.MongoDB.Entities.Player? _players;
    private int _numberOfTurns = 0;
    private int _totalEnemies = 0;
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

        var existingPlayer = mongoDbService.GetExistingPlayer();

        if(existingPlayer != null)
        {
            Console.WriteLine("Existing player found. Loading...");
            Thread.Sleep(1000);
            gameManager.LoadPlayerData(existingPlayer);
            CreateExistingPlayer(existingPlayer);
            _numberOfTurns = existingPlayer.Turns;
        }
        else
        {
            Console.Write("Please enter your name (max 8 characters): ");
            CreateNewPlayer();
            _player!.Turns = 0;
            _numberOfTurns = 0;
        }

        _totalEnemies = LevelData.Elements.OfType<Enemy>().ToList().Count;
        _deadEnemies = new();

        Console.ResetColor(); Console.Clear();
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

    private void CreateExistingPlayer(Labb3_MongoDB.MongoDB.Entities.Player existingPlayer)
    {
        _player = (Labb3_MongoDB.Models.Player)LevelData.Elements.FirstOrDefault(x => x.Type == ElementType.Player)!;
        _player.SetName(existingPlayer.Name!);
        _player.Health = existingPlayer.Health;
        _player.MaxHealth = existingPlayer.MaxHealth;
        _player.Level = existingPlayer.Level;
        _player.Experience = existingPlayer.Experience;
        _player.VisionRange = existingPlayer.VisionRange;
        _player.AttackPower = existingPlayer.AttackPower;
        _player.DefenseStrength = existingPlayer.DefenseStrength;
        _player.Turns = existingPlayer.Turns;
        _player.Position = (Position)existingPlayer.CurrentLocation!;
    }

    public void SaveAndExit()
    {

        // If there is no existing saved game - create a new one in MongoDB
        if (_players == null)
        {
            var mongoDbService = new MongoDbService();
            var gameManager = new GameManager(mongoDbService);

            var currentPlayer = new Labb3_MongoDB.MongoDB.Entities.Player
            {
                Id = Guid.NewGuid().ToString(),
                Name = _player!.Name,
                Health = _player.Health,
                MaxHealth = _player.MaxHealth,
                Level = _player.Level,
                Experience = _player.Experience,
                VisionRange = _player.VisionRange,
                AttackPower = _player.AttackPower,
                DefenseStrength = _player.DefenseStrength,
                Turns = _numberOfTurns,
                CurrentLocation = _player.Position,
                LastSaveTime = DateTime.UtcNow
            };

            gameManager.SaveProgress(currentPlayer);
            _players = currentPlayer;

            SaveGameText();

        }
        else if (_players != null)
        {
            SaveGameText();
        }
        Environment.Exit(0);
    }

    public void SaveGameText()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(0, 28);
        Console.WriteLine($"Successfully saved game at turn: {_numberOfTurns}".PadRight(Console.BufferWidth));
        Console.ResetColor();
    }
}