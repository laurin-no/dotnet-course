namespace ShapesLibrary;

public class Rectangle : IShape
{
    private double Height { get; set; }
    private double Width { get; set; }

    public Rectangle()
    {
    }

    public Rectangle(double width, double height)
    {
        Height = height;
        Width = width;
    }

    public double Area()
    {
        return Height * Width;
    }

    public double Circumference()
    {
        return 2 * (Height + Width);
    }

    public override string ToString()
    {
        return
            $"Rectangle with width {Width:0.00} and height {Height:0.00} has area {Area():0.00} and circumference {Circumference():0.00}";
    }
}