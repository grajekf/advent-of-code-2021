var lines = File.ReadAllLines("input.txt");

var positions = lines.Select(l => l.Split(" ", StringSplitOptions.RemoveEmptyEntries).Last()).Select(int.Parse).ToList();

(var scores, var rolls) = Play(positions.ToList(), new DeterministicDie());
//a
Console.WriteLine(scores.Min() * rolls);

//b
var universeCount = new long[31, 31, 11, 11, 30];


var threeDiePossibilities = new long[10];
threeDiePossibilities[3] = 1;
threeDiePossibilities[4] = 3;
threeDiePossibilities[5] = 6;
threeDiePossibilities[6] = 7;
threeDiePossibilities[7] = 6;
threeDiePossibilities[8] = 3;
threeDiePossibilities[9] = 1;

//Fill first step of player 1
for (int i = 0; i < threeDiePossibilities.Length; i++)
{
    var player1NewPosition = positions[0] + i;
    if(player1NewPosition > 10)
    {
        player1NewPosition %= 10;
    }
    universeCount[player1NewPosition, 0, player1NewPosition, positions[1], 1] = threeDiePossibilities[i];
}

for(int step = 2; step < 30; step++)
{
    for (int player1Score = 0; player1Score < 21; player1Score++)
    {
        for (int player2Score = 0; player2Score < 21; player2Score++)
        {
            for (int player1Position = 0; player1Position < 11; player1Position++)
            {
                for (int player2Position = 0; player2Position < 11; player2Position++)
                {
                    var currentCount = universeCount[player1Score, player2Score, player1Position, player2Position, step - 1];
                    if (currentCount == 0)
                    {
                        continue;
                    }
                    for (int roll = 3; roll < threeDiePossibilities.Length; roll++)
                    {
                        var newUniverseCount = threeDiePossibilities[roll];

                        if (step % 2 == 1)
                        {

                            var newPosition = player1Position + roll;
                            if (newPosition > 10)
                            {
                                newPosition %= 10;
                            }
                            var newScore = player1Score + newPosition;
                            universeCount[newScore, player2Score, newPosition, player2Position, step] += currentCount * newUniverseCount;                        }
                        else
                        {
                            var newPosition = player2Position + roll;
                            if (newPosition > 10)
                            {
                                newPosition %= 10;
                            }
                            var newScore = player2Score + newPosition;
                            universeCount[player1Score, newScore, player1Position, newPosition, step] += currentCount * newUniverseCount;                        }
                    }
                }
            }
        }
    }
}

long player1Wins = 0;
long player2Wins = 0;
for (int step = 0; step < 30; step++)
{
    for (int player1Score = 0; player1Score < 31; player1Score++)
    {
        for (int player2Score = 0; player2Score < 31; player2Score++)
        {
            for (int player1Position = 0; player1Position < 11; player1Position++)
            {
                for (int player2Position = 0; player2Position < 11; player2Position++)
                {
                    var currentCount = universeCount[player1Score, player2Score, player1Position, player2Position, step];

                    if(currentCount == 0 || (player1Score < 21 && player2Score < 21))
                    {
                        continue;
                    }
                    if (player1Score > player2Score)
                    {
                        player1Wins += currentCount;
                    }
                    if (player2Score > player1Score)
                    {
                        player2Wins += currentCount;
                    }
                }
            }
        }
    }
}

Console.WriteLine(Math.Max(player1Wins, player2Wins));


(List<int> Scores, int Rolls) Play(List<int> playerPositions, Die die)
{
    var scores = new List<int> { 0, 0 };
    var rolls = 0;

    while (!scores.Any(s => s >= 1000))
    {
        for(int i = 0; i < playerPositions.Count; i++)
        {
            playerPositions[i] = Step(playerPositions[i], die);
            rolls += 3;
            scores[i] += playerPositions[i];
            
            if(scores[i] >= 1000)
            {
                return (scores, rolls);
            }
        }
        
    }

    throw new Exception("Shouldn't happen");
}

int Step(int position, Die die)
{
    var newPosition = position + die.Roll(3);
    if(newPosition > 10)
    {
        newPosition %= 10;
    }

    return newPosition;
}

abstract class Die
{
    public abstract int Roll(int times);
}

class DeterministicDie : Die
{
    private int _lastResult = 0;
    public override int Roll(int times)
    {
        var sum = 0;

        for (int i = 0; i < times; i++)
        {
            sum += Roll();
            if(_lastResult >= 100)
            {
                _lastResult = 0;
            }
        }

        return sum;
        
    }

    private int Roll()
    {
        return ++_lastResult;  
    }
}