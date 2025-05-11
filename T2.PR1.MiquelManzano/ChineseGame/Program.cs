using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace SoparDelsReis
{
    class Program
    {
        private const int NUM_PHILOSOPHERS = 5;
        private const int SIMULATION_TIME_MS = 30000; // 30 seconds
        private const int MAX_STARVATION_TIME_MS = 15000; // 15 seconds

        // Color codes for console
        private static ConsoleColor[] philosopherColors = new ConsoleColor[]
        {
            ConsoleColor.Red,
            ConsoleColor.Green,
            ConsoleColor.Blue,
            ConsoleColor.Yellow,
            ConsoleColor.Magenta
        };

        private static Dictionary<string, ConsoleColor> stateColors = new Dictionary<string, ConsoleColor>()
        {
            { "Pensant", ConsoleColor.Cyan },
            { "Esperant", ConsoleColor.DarkYellow },
            { "Menjant", ConsoleColor.Green },
            { "Deixant", ConsoleColor.Gray }
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Sopar dels Reis - Simulació");
            Console.WriteLine("---------------------------");

            // Inicialitzar els palets (xopes)
            object[] chopsticks = new object[NUM_PHILOSOPHERS];
            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                chopsticks[i] = new object();
            }

            // Estadístiques per a cada filòsof
            int[] mealsEaten = new int[NUM_PHILOSOPHERS];
            long[] maxStarvationTime = new long[NUM_PHILOSOPHERS];

            // Inicialitzar filòsofs (threads)
            Thread[] philosophers = new Thread[NUM_PHILOSOPHERS];
            bool[] isStarving = new bool[NUM_PHILOSOPHERS];
            long[] lastMealTime = new long[NUM_PHILOSOPHERS];
            bool simulationRunning = true;

            // Inicialitzar temps d'última menjada
            long startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                lastMealTime[i] = startTime;
            }

            // Crear i iniciar els fils
            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                int philosopherId = i;
                philosophers[i] = new Thread(() => Philosopher(
                    philosopherId,
                    chopsticks[philosopherId],
                    chopsticks[(philosopherId + 1) % NUM_PHILOSOPHERS],
                    ref mealsEaten[philosopherId],
                    ref maxStarvationTime[philosopherId],
                    ref isStarving[philosopherId],
                    ref lastMealTime[philosopherId],
                    ref simulationRunning
                ));
                philosophers[i].Start();
            }

            // Controlar el temps de simulació
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < SIMULATION_TIME_MS && simulationRunning)
            {
                // Comprovar si algún filòsof passa fam
                for (int i = 0; i < NUM_PHILOSOPHERS; i++)
                {
                    long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    long starvationTime = currentTime - lastMealTime[i];

                    if (starvationTime > MAX_STARVATION_TIME_MS)
                    {
                        ConsoleWriteWithColor($"[!] El filòsof {i} ha passat massa fam! ({starvationTime}ms)", ConsoleColor.Red, ConsoleColor.White);
                        simulationRunning = false;
                        break;
                    }
                }

                Thread.Sleep(100); // Reduir l'ús de CPU
            }

            // Acabar la simulació
            simulationRunning = false;

            // Esperar que tots els fils acabin
            foreach (var philosopher in philosophers)
            {
                philosopher.Join(1000);
            }

            // Mostrar estadístiques finals
            Console.WriteLine("\nEstadístiques finals:");
            Console.WriteLine("---------------------");
            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                Console.WriteLine($"Filòsof {i}: Ha menjat {mealsEaten[i]} vegades. Temps màxim de fam: {maxStarvationTime[i]}ms");
            }

            // Guardar estadístiques en CSV
            SaveStatisticsToCSV(mealsEaten, maxStarvationTime);

            Console.WriteLine("\nPrograma finalitzat. Prem qualsevol tecla per sortir.");
            Console.ReadKey();
        }

        static void Philosopher(
            int id,
            object leftChopstick,
            object rightChopstick,
            ref int mealsEaten,
            ref long maxStarvationTime,
            ref bool isStarving,
            ref long lastMealTime,
            ref bool simulationRunning)
        {
            Random rnd = new Random(id + Environment.TickCount);

            while (simulationRunning)
            {
                // Pensar
                ConsoleWriteWithColor($"{GetTimestamp()} Filòsof {id} està pensant...", philosopherColors[id], stateColors["Pensant"]);
                Thread.Sleep(rnd.Next(500, 2000));

                // Implementació asimètrica per evitar deadlocks
                // Els filòsofs parells agafen primer el palet dret, els senars l'esquerre
                object firstChopstick, secondChopstick;
                string firstSide, secondSide;

                if (id % 2 == 0)
                {
                    firstChopstick = rightChopstick;
                    secondChopstick = leftChopstick;
                    firstSide = "dret";
                    secondSide = "esquerre";
                }
                else
                {
                    firstChopstick = leftChopstick;
                    secondChopstick = rightChopstick;
                    firstSide = "esquerre";
                    secondSide = "dret";
                }

                // Temps que porta sense menjar
                long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                long starvationTime = currentTime - lastMealTime;

                // Actualitzar temps màxim de fam
                if (starvationTime > maxStarvationTime)
                {
                    maxStarvationTime = starvationTime;
                }

                // Agafar palets
                ConsoleWriteWithColor($"{GetTimestamp()} Filòsof {id} vol agafar el palet {firstSide}...", philosopherColors[id], stateColors["Esperant"]);

                // Implementant una estratègia per evitar deadlocks: timeout al bloquejar els palets
                bool hasFirstChopstick = false;
                bool hasSecondChopstick = false;

                try
                {
                    // Intenta agafar el primer palet
                    Monitor.TryEnter(firstChopstick, 1000, ref hasFirstChopstick);

                    if (hasFirstChopstick)
                    {
                        ConsoleWriteWithColor($"{GetTimestamp()} Filòsof {id} ha agafat el palet {firstSide}", philosopherColors[id], stateColors["Esperant"]);
                        ConsoleWriteWithColor($"{GetTimestamp()} Filòsof {id} vol agafar el palet {secondSide}...", philosopherColors[id], stateColors["Esperant"]);

                        // Intenta agafar el segon palet
                        Monitor.TryEnter(secondChopstick, 1000, ref hasSecondChopstick);

                        if (hasSecondChopstick)
                        {
                            ConsoleWriteWithColor($"{GetTimestamp()} Filòsof {id} ha agafat el palet {secondSide}", philosopherColors[id], stateColors["Esperant"]);

                            // Menjar
                            ConsoleWriteWithColor($"{GetTimestamp()} Filòsof {id} està menjant!", philosopherColors[id], stateColors["Menjant"]);
                            Thread.Sleep(rnd.Next(500, 1000));

                            // Actualitzar estadístiques
                            mealsEaten++;
                            lastMealTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            isStarving = false;

                            // Deixar el segon palet
                            ConsoleWriteWithColor($"{GetTimestamp()} Filòsof {id} deixa el palet {secondSide}", philosopherColors[id], stateColors["Deixant"]);
                        }
                        else
                        {
                            ConsoleWriteWithColor($"{GetTimestamp()} Filòsof {id} no ha pogut agafar el palet {secondSide}, torna a provar", philosopherColors[id], ConsoleColor.Red);
                        }

                        // Deixar el primer palet
                        ConsoleWriteWithColor($"{GetTimestamp()} Filòsof {id} deixa el palet {firstSide}", philosopherColors[id], stateColors["Deixant"]);
                    }
                    else
                    {
                        ConsoleWriteWithColor($"{GetTimestamp()} Filòsof {id} no ha pogut agafar el palet {firstSide}, torna a provar", philosopherColors[id], ConsoleColor.Red);
                    }
                }
                finally
                {
                    // Alliberar els recursos
                    if (hasSecondChopstick) Monitor.Exit(secondChopstick);
                    if (hasFirstChopstick) Monitor.Exit(firstChopstick);
                }

                // Comprovar si passa fam
                currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                starvationTime = currentTime - lastMealTime;

                if (starvationTime > 5000 && !isStarving)
                {
                    isStarving = true;
                    ConsoleWriteWithColor($"{GetTimestamp()} Filòsof {id} està començant a passar fam! ({starvationTime}ms)", philosopherColors[id], ConsoleColor.DarkRed);
                }
            }
        }

        static void ConsoleWriteWithColor(string message, ConsoleColor foreground, ConsoleColor background)
        {
            lock (Console.Out)
            {
                Console.ForegroundColor = foreground;
                Console.BackgroundColor = background;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        static string GetTimestamp()
        {
            return $"[{DateTime.Now:HH:mm:ss.fff}]";
        }

        static void SaveStatisticsToCSV(int[] mealsEaten, long[] maxStarvationTime)
        {
            string filePath = "sopar_stats.csv";

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Numero de comensal,Temps maxim que ha passat gana (ms),Numero de vegades que ha menjat");

                    for (int i = 0; i < NUM_PHILOSOPHERS; i++)
                    {
                        writer.WriteLine($"{i},{maxStarvationTime[i]},{mealsEaten[i]}");
                    }
                }

                Console.WriteLine($"Estadístiques guardades a {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar les estadístiques: {ex.Message}");
            }
        }
    }
}