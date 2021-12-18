var inputNumbers = File.ReadAllLines("input.txt").Select(l => Tokenize(l).ToList()).ToList();

var result = inputNumbers.First();

foreach (var number in inputNumbers.Skip(1))
{
    result = AddReduce(result, number).ToList();
}

//a
Console.WriteLine(Magnitude(result));

//b
Console.WriteLine(GetAllPairs(inputNumbers).Max(p => Magnitude(AddReduce(p.Item1.ToList(), p.Item2.ToList()))));

IEnumerable<(IEnumerable<string>, IEnumerable<string>)> GetAllPairs(IEnumerable<IEnumerable<string>> numbers)
{
    foreach (var first in numbers)
    {
        foreach (var second in numbers)
        {
            if(first != second)
            {
                yield return (first, second);
                yield return (second, first);
            }
        }
    }
}


long Magnitude(IList<string> tokens)
{
   var stack = new Stack<string>();

    foreach (var token in tokens)
    {
        if (token == "[")
        {
            stack.Push(token);
            continue;
        }

        if(token == "]")
        {
            var rightNumber = int.Parse(stack.Pop());
            var leftNumber = int.Parse(stack.Pop());
            stack.Pop();
            stack.Push((3 * leftNumber + 2 * rightNumber).ToString());
            continue;
        }

        stack.Push(token);
    }

    return long.Parse(stack.Pop());

}

IList<string> AddReduce(IList<string> a, IList<string> b)
{
    var result = Add(a, b).ToList();
    Reduce(result);
    return result;
}

void Reduce(IList<string> tokens)
{
    bool change = true;
    while (change)
    {
        change = false;
        change = Explode(tokens);
        if(change)
        {
            continue;
        }
        change = Split(tokens);
    }
}


void Print(IEnumerable<string> tokens)
{
    int depth = 0;
    foreach (var token in tokens)
    {
        if (token == "]")
        {
            depth--;
        }

        for (int i = 0; i < depth; i++)
        {
            Console.Write('\t');
        }

        if (token == "[")
        {
            depth++;
        }


        Console.WriteLine(token);
    }
}

void PrintLine(IEnumerable<string> tokens)
{
    Console.WriteLine(string.Join(" ", tokens));
}

bool Explode(IList<string> tokens)
{
    int depth = 0;
    var position = 0;

    while (position < tokens.Count)
    {
        if (tokens[position] == "[")
        {
            depth++;
        }

        if (tokens[position] == "]")
        {
            depth--;
        }

        if (int.TryParse(tokens[position], out var leftValue) && depth > 4)
        {
            if(!int.TryParse(tokens[position + 1], out var rightValue))
            {
                position++;
                continue;
            }
            for (int j = position - 2; j >= 0; j--)
            {
                if (int.TryParse(tokens[j], out var newLeft))
                {
                    tokens[j] = (newLeft + leftValue).ToString();
                    break;
                }
            }
            for (int j = position + 3; j < tokens.Count; j++)
            {
                if (int.TryParse(tokens[j], out var newRight))
                {
                    tokens[j] = (newRight + rightValue).ToString();
                    break;
                }
            }

            tokens[position] = "0";
            tokens.RemoveAt(position + 2);
            tokens.RemoveAt(position + 1);
            tokens.RemoveAt(position - 1);

            return true;
        }
        position++;
    }

    return false;
}

bool Split(IList<string> tokens)
{
    var position = 0;
    while (position < tokens.Count)
    {
        if (int.TryParse(tokens[position], out var value) && value > 9)
        {
            var newLeft = Math.Floor(value / 2.0);
            var newRight = Math.Ceiling(value / 2.0);

            tokens[position] = "[";
            tokens.Insert(position + 1, newLeft.ToString());
            tokens.Insert(position + 2, newRight.ToString());
            tokens.Insert(position + 3, "]");

            return true;
        }

        position++;
    }

    return false;
}

IEnumerable<string> Tokenize(string input)
{
    var position = 0;
    string currentNumber = string.Empty;
    while(position < input.Length)
    {

        if (char.IsDigit(input[position]))
        {
            currentNumber += input[position];
        }
        else
        {
            if (currentNumber.Length > 0)
            {
                yield return currentNumber;
                currentNumber = string.Empty;
            }
        }
        if (input[position] == '[' || input[position] == ']')
        {
            yield return input[position].ToString();
        }

        position++;
    }
}

IEnumerable<string> Add(IEnumerable<string> a, IEnumerable<string> b)
{
    yield return "[";

    foreach (var token in a)
    {
        yield return token;
    }

    foreach (var token in b)
    {
        yield return token;
    }

    yield return "]";
}