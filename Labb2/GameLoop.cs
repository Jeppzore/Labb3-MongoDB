using Labb3_MongoDB;
using Labb3_MongoDB.Models;
using Labb3_MongoDB.MongoDB;
using MongoDB.Driver;
using System.Threading.Tasks;

class GameLoop()
{
    private Player? _player;
    private Players? _players;
    private int _numberOfTurns = 0;
    private int _totalEnemies = 0;
    private List<Enemy> _deadEnemies = new();

    public async Task SaveGame()
    {
        if (_player == null)
        {
            return;
        }

        var mongoDbService = new MongoDbService();
        var gameManager = new GameManager(mongoDbService);

        var newPlayer = new Players
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
            CurrentLocation = _player.Position,
            LastSaveTime = DateTime.UtcNow
        };

        await gameManager.SaveProgress(newPlayer);


        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(70, 15);
        Console.WriteLine($"Successfully saved game at turn: {_numberOfTurns}".PadRight(Console.BufferWidth));
        Console.ResetColor();
    }

    public void Start()
    {
        _ = Setup();

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
                SaveGame();
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

    private async Task Setup()
    {
        var mongoDbService = new MongoDbService();
        var gameManager = new GameManager(mongoDbService);

        Console.SetWindowSize(120, 30);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Welcome to Jesper & Robin's Dungeon Crawler!\nPlease enter your name (max 8 characters): ");

        LevelData.Load("Levels\\Level1.txt");

        CreatePlayer();
        _totalEnemies = LevelData.Elements.OfType<Enemy>().ToList().Count;
        _deadEnemies = new();

        Console.ResetColor(); Console.Clear();
        Console.CursorVisible = false;
        _numberOfTurns = 0;


        /* TODO: See if an if-null check is viable to only create
           a new instance of Players if it doesn't already exists. */

        var newPlayer = new Players
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
            CurrentLocation = _player.Position,
            LastSaveTime = DateTime.UtcNow
        };

        // Waits for the Player to get saved to MongoDB before continuing
        await gameManager.SaveProgress(newPlayer);
    }

    private void CreatePlayer()
    {
        var name = Console.ReadLine()!;
        if (name.Length <= 0 || name.Length > 8)
        {
            Console.Clear();
            Start();
        }

        _player = (Player)LevelData.Elements.FirstOrDefault(x => x.Type == ElementType.Player)!;
        _player.SetName(name);       
    }

}