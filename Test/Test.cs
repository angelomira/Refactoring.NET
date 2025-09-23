using System.Diagnostics;
using RefExample2.Algorithm;

namespace RefExample2.Test;

public static class Test
{
    /// <summary>
    /// Method to initialize a metric test for all sorting
    /// algorithms.
    /// </summary>
    /// <param name="name">
    /// Display name of the test.
    /// </param>
    /// <param name="data">
    /// An 32-bit signed integer array which will be sorted.
    /// </param>
    /// <param name="runBubble">
    /// Is test for bubble sort or not: bubble sort is applicable only for small arrays.
    /// </param>
    public static void RunTests(string name, int[] data, bool runBubble = false)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        
        Console.WriteLine($"{name}: array length N = {data.Length};");
        Console.ResetColor();
        
        /* Block sort, "grail sort" algorithm */
        var copy1 = (int[])data.Clone();
        var sw = Stopwatch.StartNew();
        var sorter = new BlockSortAlgorithm(copy1); // Include initialization as part of algorithm's work time
        sorter.Sort();
        sw.Stop();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Block sort: sort time = {sw.ElapsedMilliseconds}ms;");
        Console.ResetColor();
        
        /* Native C# sorting algorithm: Array.Sort (TimSort/IntroSort) */
        var copy2 = (int[])data.Clone();
        sw.Restart();
        Array.Sort(copy2);
        sw.Stop();
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"C# native: sort time = {sw.ElapsedMilliseconds}ms;");
        Console.ResetColor();
        
        /* Bubble sort, using only for small/medium arrays because of its inefficiency. */
        if (runBubble)
        {
            var copy3 = (int[])data.Clone();
            sw.Restart();
            BubbleSortAlgorithm.Sort(copy3);
            sw.Stop();
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Bubble sort: sort time = {sw.ElapsedMilliseconds}ms;");
            Console.ResetColor();
        }
    }
}