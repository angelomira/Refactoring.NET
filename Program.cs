using System.Diagnostics;
using RefExample2.Algorithm;

namespace RefExample2
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            /* Generate arrays:
             1. Small: 10 entries;
             2. Medium: 10000 entries;
             3. Large: 1000000 entries;
             */

            int[] small = [9, 4, 1, 7, 3, 8, 2, 6, 5, 0];
            var medium = new int[100000];
            var large = new int[1_000_000];

            var rnd = new Random();

            for (var i = 0; i < medium.Length; i++) medium[i] = rnd.Next(0, medium.Length);
            for (var i = 0; i < large.Length; i++) large[i] = rnd.Next(0, large.Length);

            Test.Test.RunTests("Small array", small, runBubble: true);
            Test.Test.RunTests("Medium array", medium, runBubble: true);
            Test.Test.RunTests("Large array", large, runBubble: false);
        }
    }
}