using BranchAndBound;
using BranchAndBound.Tasks;

do
{
    Console.WriteLine("Problems available:");
    Console.WriteLine("1 - Travelling Salesperson Problem");
    Console.WriteLine("2 - Quadratic Assignment Problem");
    Console.WriteLine("3 - Flow-Shop Scheduling Problem");
    Console.WriteLine("-1 - Run Example Problems");
    Console.Write("Choose a problem to solve (exit with 0): ");
    string? input = Console.ReadLine();
    if (input == null) continue;
    if (!int.TryParse(input, out int problem) || problem < -1 || problem > 3) continue;
    if (problem == 0) break;
    if (problem > 0)
    {
        do
        {
            Console.Write("Choose a problem size (go back with 0): ");
            input = Console.ReadLine();
            if (input == null) continue;
            if (!int.TryParse(input, out int size) || size < 0) continue;
            if (size == 0) break;
            do
            {
                Console.Write("Choose a number of threads to be used (go back with 0, benchmark different numbers with -1): ");
                input = Console.ReadLine();
                if (input == null) continue;
                if (!int.TryParse(input, out int nThreads) || nThreads < -1) continue;
                if (nThreads == 0) break;
                if (nThreads == -1)
                {
                    List<BnBBenchmarkResult> results = [];
                    for (int i = 1; i <= 8; i *= 2)
                    {
                        Console.WriteLine($"Benchmarking for {i} thread{(i == 1 ? "" : "s")}");
                        for (int j = 0; j < 5; j++)
                        {
                            IBnBTask task = problem switch
                            {
                                1 => new TSPTask(size),
                                2 => new QAPTask(size),
                                3 => new FSSTask(size),
                                _ => throw new ArgumentException($"Problem {problem} does not exist. This should not happen")
                            };
                            BnB bnb = new(task, i);
                            DateTime dateTime = DateTime.Now;
                            await bnb.Run();
                            TimeSpan time = DateTime.Now - dateTime;
                            results.Add(new BnBBenchmarkResult(i, time.TotalSeconds));
                        }
                    }
                    List<BnBBenchmarkResult> averagedResults = BnBBenchmarkResult.GroupAndAverage(results);
                    averagedResults.ForEach(res => Console.WriteLine($"{res.NTasks}: {res.Time} seconds"));
                }
                else
                {
                    IBnBTask task = problem switch
                    {
                        1 => new TSPTask(size),
                        2 => new QAPTask(size),
                        3 => new FSSTask(size),
                        _ => throw new ArgumentException($"Problem {problem} does not exist. This should not happen")
                    };
                    task.PrintProblem();
                    BnB bnb = new(task, nThreads);
                    await bnb.Run();
                    Console.WriteLine(bnb.GlobalBest);
                }
            } while (true);
        } while (true);
    }
    else
    {
        int[,] tspDistances =
        {
            { 7, 1, 2, 7, 6, 9, 1, 9, 1, 1, 0, 9, 7, 7, 3 },
            { 9, 0, 0, 0, 2, 8, 0, 5, 7, 4, 3, 5, 9, 0, 0 },
            { 0, 9, 2, 0, 7, 2, 4, 9, 2, 0, 0, 0, 6, 1, 7 },
            { 2, 0, 5, 6, 0, 0, 1, 0, 8, 9, 3, 0, 4, 0, 3 },
            { 0, 8, 3, 2, 0, 3, 1, 5, 2, 1, 1, 2, 7, 0, 7 },
            { 0, 2, 2, 3, 4, 7, 2, 0, 0, 0, 6, 6, 6, 5, 8 },
            { 8, 9, 0, 0, 0, 1, 0, 0, 0, 9, 0, 0, 8, 4, 0 },
            { 9, 1, 2, 5, 6, 9, 0, 3, 0, 0, 8, 2, 0, 0, 9 },
            { 0, 7, 5, 0, 2, 5, 9, 7, 0, 9, 9, 8, 4, 5, 6 },
            { 5, 2, 6, 6, 0, 0, 1, 2, 0, 3, 4, 7, 4, 6, 9 },
            { 6, 3, 8, 9, 4, 7, 4, 3, 4, 3, 2, 9, 2, 4, 9 },
            { 7, 0, 7, 1, 9, 8, 0, 8, 9, 5, 2, 3, 9, 7, 0 },
            { 8, 8, 1, 2, 1, 3, 4, 2, 6, 0, 4, 7, 4, 0, 1 },
            { 0, 9, 5, 4, 0, 3, 4, 0, 6, 0, 0, 9, 7, 9, 9 },
            { 3, 2, 1, 3, 4, 6, 6, 7, 0, 0, 6, 0, 9, 0, 2 }
        };
        BnB testBnb = new(new TSPTask(tspDistances));
        await testBnb.Run();
        Console.WriteLine(testBnb.GlobalBest);

        int[,] qapFlows =
        {
            { 0, 50, 60 },
            { 50, 0, 36 },
            { 60, 36, 0 }
        };
        int[,] qapDistances =
        {
            { 0, 100, 30 },
            { 100, 0, 65 },
            { 30, 65, 0 }
        };
        testBnb = new(new QAPTask(qapFlows, qapDistances));
        await testBnb.Run();
        Console.WriteLine(testBnb.GlobalBest);

        int[,] fssTasks = {
            { 4, 3 },
            { 1, 2 },
            { 5, 4 },
            { 2, 3 },
            { 5, 6 }
        };
        testBnb = new(new FSSTask(fssTasks));
        await testBnb.Run();
        Console.WriteLine(testBnb.GlobalBest);
    }
    Console.WriteLine();
} while (true);
Console.WriteLine("Goodbye!");
