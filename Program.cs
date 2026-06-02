using System;

class Program
{
    static void Main()
    {
        int sum = 0;
        
        // Add numbers from 1 to 100
        for (int i = 1; i <= 100; i++)
        {
            sum += i;
        }
        
        Console.WriteLine($"Sum of numbers from 1 to 100: {sum}");
    }
}
