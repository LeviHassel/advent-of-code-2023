namespace AdventOfCode2023.Day24;

public class Hail
{
    public int Id;
    public double VelocityX;
    public double VelocityY;
    public double Slope;
    public double YIntercept;
    public Dictionary<double, Position> PositionByNanosecond = new Dictionary<double, Position>();
}