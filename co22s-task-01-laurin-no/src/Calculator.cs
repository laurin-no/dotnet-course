namespace ConsoleApp;

public class Calculator
{
    public int? CalculateArea(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return null;
        }

        var res = (long)x * y;
        if (res > int.MaxValue)
        {
            return null;
        }

        return (int)res;
    }
}