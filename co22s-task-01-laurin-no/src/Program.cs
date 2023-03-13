using ConsoleApp;

Console.WriteLine("Hello, .NET!");

int xLength, yLength;

if (args.Length == 2)
{
    var x = args[0];
    var y = args[1];
    Console.WriteLine($"Arguments: {x} {y}");

    xLength = int.Parse(x);
    yLength = int.Parse(y);
}
else if (args.Length == 1)
{
    var x = args[0];
    Console.WriteLine($"Arguments: {x}");

    xLength = int.Parse(x);
    yLength = 0;
}
else
{
    xLength = 0;
    yLength = 0;
}

var calculator = new Calculator();
var res = calculator.CalculateArea(xLength, yLength);

Console.WriteLine($"Area of {xLength} and {yLength} is {res}");