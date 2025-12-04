# GagLib

Speech-to-muffled-speech transformer for Discord bots. Converts text to phonemes, then mangles them like you're talking through a ball gag. Extendable to add other types of gags/speech transformations.

## Usage

```csharp
using GagLib;

var output = Gag.Transform(GagType.BallGag, "Hello, world!");
// → "hhuhlloh, wwrrllnn!"
```

Preserves punctuation, emoji, and Discord embeds (`<@mentions>`, `<:custom:emoji>`).

## How it works

1. Tokenize input (words vs preserved content)
2. Words → phonemes (CMU dictionary → trie splitting → heuristic fallback)
3. Phonemes → muffled output based on articulation physics

## Run the demo

```bash
dotnet run --project examples/GagLib.Example
```

## License

MIT
