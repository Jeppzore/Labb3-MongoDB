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
            // Delete current Collection in mongoDB Database
            var mongoDbService = new MongoDbService();
            mongoDbService.DeleteCollection();
            Environment.Exit(0);
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
                var mongoDbService = new MongoDbService();
                mongoDbService.DeleteCollection();
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

            _player = LevelData.Elements.OfType<Labb3_MongoDB.Models.Player>().FirstOrDefault(x => x.Type == ElementType.Player);
            _numberOfTurns = _player!.Turns;

            var discoveredWalls = LevelData.Elements.OfType<Wall>().Where(wall => wall.IsDiscovered).ToList();
            foreach (var wall in discoveredWalls)
            {
                wall.Draw();
            }
        }
        else
        {
            Console.Write("Please enter your name (max 8 characters): ");
            CreateNewPlayer();
            _player!.Turns = 0;
            _numberOfTurns = 0;
            Console.Clear();
        }

        _totalEnemies = LevelData.Elements.OfType<Enemy>().ToList().Count;
        _deadEnemies = new();

        Console.ResetColor();
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

    public void SaveAndExit()
    {
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

    public void SaveGameText()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(0, 28);
        Console.WriteLine($"Successfully saved game at turn: {_numberOfTurns}".PadRight(Console.BufferWidth));
        Console.ResetColor();
    }
}