using GagLib;

Console.WriteLine("GagLib Demo");
Console.WriteLine("===========");
Console.WriteLine("Select gag type:");
Console.WriteLine("  1. Ball Gag");
Console.WriteLine("  2. Cow Gag");
Console.WriteLine("  3. Dog Gag (ruh-roh!)");
Console.WriteLine("  4. Barking Dog Gag (ruff woof grr!)");
Console.WriteLine("  5. Catgirl Gag (nya~)");
Console.WriteLine("  6. Cat Gag (meow!)");
Console.Write("\nChoice: ");

var choice = Console.ReadLine();
var gagType = choice switch
{
    "2" => GagType.CowGag,
    "3" => GagType.DogGag,
    "4" => GagType.BarkingDogGag,
    "5" => GagType.CatgirlGag,
    "6" => GagType.CatGag,
    _ => GagType.BallGag
};

Console.WriteLine($"\nUsing: {gagType}");
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

    var gagged = Gag.Transform(gagType, input);
    Console.WriteLine($"Gagged: {gagged}\n");
}

Console.WriteLine("Bye!");
