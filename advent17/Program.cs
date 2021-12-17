

using System.Text.RegularExpressions;

var input = File.ReadAllText("input.txt");

var xMatch = Regex.Match(input, @"x=(.*)\.\.(.*),");
var xExtent = (From: int.Parse(xMatch.Groups[1].ToString()), To: int.Parse(xMatch.Groups[2].ToString()));

var yMatch = Regex.Match(input, @"y=(.*)\.\.(.*)$");
var yExtent = (From: int.Parse(yMatch.Groups[1].ToString()), To: int.Parse(yMatch.Groups[2].ToString()));

int highestY = 0;
int noOfPossibilities = 0;

for(int xStep = 1; xStep <= xExtent.To; xStep++)
{
    for(int yStep = yExtent.From; yStep <= 1000; yStep++)
    {
        var result = SimulateTrajectory((xStep, yStep), xExtent, yExtent);
        if(result >= 0)
        {
            if(result > highestY)
            {
                highestY = result;
            }
            noOfPossibilities++;
        }
        
    }
}
//a
Console.WriteLine(highestY);
//b
Console.WriteLine(noOfPossibilities);





int SimulateTrajectory((int X, int Y) step, (int From, int To) xExtent, (int From, int To) yExtent)
{
    var position = (X: 0, Y: 0);

    int highestY = 0;

    while (true)
    {
        if(position.Y >= highestY)
        {
            highestY = position.Y;
        }

        if(position.X >= xExtent.From && position.X <= xExtent.To && position.Y >= yExtent.From && position.Y <= yExtent.To)
        {
            return highestY;
        }

        if(position.X > xExtent.To || position.Y < yExtent.From)
        {
            return int.MinValue;
        }

        if(step.X == 0 && position.X < xExtent.From)
        {
            return int.MinValue;
        }

        position = (position.X + step.X, position.Y + step.Y);

        step.X -= Math.Sign(step.X);
        step.Y--;
    }
}



