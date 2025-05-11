using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseGame
{
    class Chino
    {
        public int Id { get; }
        private object leftChopstick;
        private object rightChopstick;
        private Random rand = new Random();
        public int TimesEaten { get; private set; } = 0;
        public DateTime LastMeal { get; private set; } = DateTime.Now;
        public TimeSpan MaxHungryTime { get; private set; } = TimeSpan.Zero;

        public Chino(int id, object left, object right)
        {
            Id = id;
            leftChopstick = left;
            rightChopstick = right;
        }

        public async Task Start(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Think();

                var hungryStart = DateTime.Now;

                object first = Id % 2 == 0 ? leftChopstick : rightChopstick;
                object second = Id % 2 == 0 ? rightChopstick : leftChopstick;

                lock (first)
                {
                    Log($"picked up first chopstick", ConsoleColor.Yellow);
                    lock (second)
                    {
                        Log($"picked up second chopstick", ConsoleColor.Yellow);
                        Eat();
                        var hungryTime = DateTime.Now - hungryStart;
                        if (hungryTime > MaxHungryTime)
                            MaxHungryTime = hungryTime;
                        LastMeal = DateTime.Now;
                        TimesEaten++;
                    }
                }
            }
        }

        private void Think()
        {
            Log("is thinking", ConsoleColor.Cyan);
            Thread.Sleep(rand.Next(500, 2000));
        }

        private void Eat()
        {
            Log("is eating", ConsoleColor.Green);
            Thread.Sleep(rand.Next(500, 1000));
            Log("put down chopsticks", ConsoleColor.Red);
        }

        private void Log(string action, ConsoleColor color)
        {
            lock (Console.Out)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Chino {Id} {action}");
                Console.ResetColor();
            }
        }
    }
}
