var codeLines = File.ReadAllLines("inputa.txt");

var errorSum = 0L;
var autoCompleteScores = new List<long>();

foreach (var line in codeLines)
{
    var firstIllegalCharacter = GetFirstIllegalCharacter(line, out var missing);
    if(firstIllegalCharacter == ' ' && missing.Length > 0)
    {
        autoCompleteScores.Add(ScoreAutocomplete(missing));
    }
    errorSum += ScoreChar(firstIllegalCharacter);

}
//a
Console.WriteLine(errorSum);
//b
Console.WriteLine(autoCompleteScores.OrderBy(s => s).ElementAt(autoCompleteScores.Count / 2));


char GetFirstIllegalCharacter(string line, out string missing)
{
    const string OpeningChars = "([{<";
    const string ClosingChars = ")]}>";

    var stack = new Stack<char>();
    for (int i = 0; i < line.Length; i++)
    {
        if(OpeningChars.Contains(line[i]))
        {
            stack.Push(line[i]);
        }
        if(ClosingChars.Contains(line[i]))
        {
            var matching = stack.Pop();
            if (ClosingChars.IndexOf(line[i]) != OpeningChars.IndexOf(matching))
            {
                missing = string.Empty;
                return line[i];
            }
        }
    }

    missing = string.Concat(stack.Select(c =>  ClosingChars[OpeningChars.IndexOf(c)]));
    return ' ';
}

int ScoreChar(char c)
{
    switch(c)
    {
        case ')':
            return 3;
        case ']':
            return 57;
        case '}':
            return 1197;
        case '>':
            return 25137;
        default:
            return 0;
    }
        
}

long ScoreAutocomplete(string autocomplete)
{
    long score = 0L;
    foreach (var c in autocomplete)
    {
        score *= 5L;
        score += ScoreAutoCompleteChar(c);
    }

    return score;
}

int ScoreAutoCompleteChar(char c)
{
    switch (c)
    {
        case ')':
            return 1;
        case ']':
            return 2;
        case '}':
            return 3;
        case '>':
            return 4;
        default:
            return 0;
    }

}