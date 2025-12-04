using GagLib;

Console.WriteLine("GagLib Demo - Ball Gag Simulator");
Console.WriteLine("================================");
Console.WriteLine("Type a message and see it transformed.");
Console.WriteLine("Type 'quit' to exit.\n");

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();

    if (string.IsNullOrEmpty(input))
        continue;

    if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
        break;

    var gagged = Gag.Transform(GagType.BallGag, input);
    Console.WriteLine($"Gagged: {gagged}\n");
}

Console.WriteLine("Bye!");
