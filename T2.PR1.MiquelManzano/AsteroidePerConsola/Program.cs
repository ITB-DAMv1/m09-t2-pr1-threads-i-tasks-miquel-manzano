using System;
using AsteroidePerConsola;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.CursorVisible = false;
        var game = new Game();
        await game.Start();
    }
}

public class Game
{
    private const int FrameRate = 20; // Hz
    private const int LogicRate = 50; // Hz
    private bool isRunning = true;
    private Spaceship spaceship;
    private List<Asteroid> asteroids = new List<Asteroid>();
    private int width, height;
    private int dodged = 0;
    private int lives = 0;
    private DateTime startTime;
    private Random rand = new Random();

    public async Task Start()
    {
        width = Console.WindowWidth;
        height = Console.WindowHeight;
        spaceship = new Spaceship(width / 2, height - 1);
        startTime = DateTime.Now;

        var tasks = new List<Task>
        {
            Task.Run(() => InputLoop()),
            Task.Run(() => LogicLoop()),
            Task.Run(() => DrawLoop()),
            Task.Run(() => SimulateWebCheck())
        };

        await Task.WhenAny(tasks);
        isRunning = false;
        await Task.WhenAll(tasks);

        EndGame();
    }

    private async Task DrawLoop()
    {
        while (isRunning)
        {
            Console.Clear();
            Console.SetCursorPosition(spaceship.X, spaceship.Y);
            Console.Write("^");

            foreach (var a in asteroids)
            {
                if (a.Y < height)
                {
                    Console.SetCursorPosition(a.X, a.Y);
                    Console.Write("*");
                }
            }
            await Task.Delay(1000 / FrameRate);
        }
    }

    private async Task LogicLoop()
    {
        while (isRunning)
        {
            List<Asteroid> toRemove = new List<Asteroid>();

            foreach (var a in asteroids)
            {
                a.Y++;
                if (a.Y >= height)
                {
                    toRemove.Add(a);
                    dodged++;
                }
                else if (a.X == spaceship.X && a.Y == spaceship.Y)
                {
                    lives++;
                    spaceship.X = width / 2;
                    toRemove.Add(a);
                }
            }
            foreach (var a in toRemove)
                asteroids.Remove(a);

            if (rand.NextDouble() < 0.2)
            {
                asteroids.Add(new Asteroid(rand.Next(0, width), 0));
            }
            await Task.Delay(1000 / LogicRate);
        }
    }

    private void InputLoop()
    {
        while (isRunning)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q)
                {
                    isRunning = false;
                    break;
                }
                if (key == ConsoleKey.A && spaceship.X > 0)
                    spaceship.X--;
                if (key == ConsoleKey.D && spaceship.X < width - 1)
                    spaceship.X++;
            }
            Thread.Sleep(10);
        }
    }

    private async Task SimulateWebCheck()
    {
        await Task.Delay(rand.Next(30000, 60000));
        isRunning = false;
    }

    private void EndGame()
    {
        Console.Clear();
        Console.WriteLine("\nGAME OVER");
        Console.WriteLine($"Asteroides esquivats: {dodged}");
        Console.WriteLine($"Vides perdudes: {lives}");
        Console.WriteLine($"Temps de joc: {(DateTime.Now - startTime).TotalSeconds:F1} s");

        string path = "game_stats.csv";
        File.WriteAllText(path, "Asteroides esquivats,Vides perdudes,Temps de joc\n");
        File.AppendAllText(path, $"{dodged},{lives},{(DateTime.Now - startTime).TotalSeconds:F1}\n");

        Console.WriteLine($"\nDades guardades a: {path}");
        Console.WriteLine("Prem qualsevol tecla per sortir...");
        Console.ReadKey();
    }
}