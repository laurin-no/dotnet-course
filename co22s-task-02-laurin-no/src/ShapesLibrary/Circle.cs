namespace ShapesLibrary;

public class Circle : IShape
{
    public double Radius { get; set; }

    public Circle()
    {
    }

    public Circle(double radius)
    {
        Radius = radius;
    }

    public double Area()
    {
        return Radius * Radius * Math.PI;
    }

    public double Circumference()
    {
        return 2 * Radius * Math.PI;
    }

    public override string ToString()
    {
        return $"Circle with radius {Radius:0.00} has area {Area():0.00} and circumference {Circumference():0.00}";
    }
}