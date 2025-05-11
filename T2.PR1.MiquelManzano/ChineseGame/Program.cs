using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ChineseGame;

class Program
{
    static async Task Main()
    {
        var dinner = new Dinner(5);
        await dinner.StartDinner();
    }
}

class Dinner
{
    private int numChinos;
    private Chino[] chinos;
    private object[] chopsticks;
    private CancellationTokenSource cts = new CancellationTokenSource();
    private DateTime startTime;
    private const int MaxDurationSeconds = 60;

    public Dinner(int num)
    {
        numChinos = num;
        chopsticks = new object[num];
        for (int i = 0; i < num; i++)
            chopsticks[i] = new object();

        chinos = new Chino[num];
        for (int i = 0; i < num; i++)
            chinos[i] = new Chino(i, chopsticks[i], chopsticks[(i + 1) % num]);
    }

    public async Task StartDinner()
    {
        startTime = DateTime.Now;
        var tasks = new List<Task>();

        foreach (var p in chinos)
        {
            tasks.Add(Task.Run(() => p.Start(cts.Token)));
        }

        // Monitor starvation and time limit
        await Task.Run(() => MonitorSimulation());
        cts.Cancel();
        await Task.WhenAll(tasks);
        SaveStats();
    }

    private void MonitorSimulation()
    {
        while (!cts.IsCancellationRequested)
        {
            foreach (var p in chinos)
            {
                if ((DateTime.Now - p.LastMeal).TotalSeconds > 15)
                {
                    Console.WriteLine($"\nChino {p.Id} is starving! Ending simulation.");
                    cts.Cancel();
                    return;
                }
            }

            if ((DateTime.Now - startTime).TotalSeconds > MaxDurationSeconds)
            {
                Console.WriteLine("\n60 seconds elapsed. Ending simulation.");
                cts.Cancel();
                return;
            }

            Thread.Sleep(1000);
        }
    }

    private void SaveStats()
    {
        using var writer = new StreamWriter("dinner_stats.csv");
        writer.WriteLine("Chino,Times Eaten,Max Time Hungry (s)");

        foreach (var p in chinos)
        {
            writer.WriteLine($"{p.Id},{p.TimesEaten},{p.MaxHungryTime.TotalSeconds:F2}");
        }
        Console.WriteLine("\nSimulation ended. Stats saved to dinner_stats.csv");
    }
}
