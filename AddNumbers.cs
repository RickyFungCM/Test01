using System;

public class NumberSum
{
    /// <summary>
    /// Adds all numbers from 1 to 110 and returns the sum.
    /// </summary>
    /// <returns>The sum of numbers from 1 to 110</returns>
    public static int AddOneTo110()
    {
        int sum = 0;
        for (int i = 1; i <= 110; i++)
        {
            sum += i;
        }
        return sum;
    }

    /// <summary>
    /// Alternative method using the mathematical formula: n * (n + 1) / 2
    /// </summary>
    /// <returns>The sum of numbers from 1 to 110</returns>
    public static int AddOneTo110Formula()
    {
        int n = 110;
        return n * (n + 1) / 2;
    }

    // Example usage
    static void Main()
    {
        Console.WriteLine("Sum (loop method): " + AddOneTo110());
        Console.WriteLine("Sum (formula method): " + AddOneTo110Formula());
    }
}
