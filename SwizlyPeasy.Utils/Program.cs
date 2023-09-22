using SwizlyPeasy.Utils.Ocelot;

if (args.Length < 2)
{
    Console.WriteLine("Please provide the path to the ocelot config file and the path to the swizly config file.");
    return;
}

if (!File.Exists(args[0]))
{
    throw new FileNotFoundException("Ocelot configuration file couldn't be found.");
}

if (!File.Exists(args[1]))
{
    throw new FileNotFoundException("Swizly configuration file couldn't be found.");
}

var input = File.ReadAllText(args[0]);
var swizlyConfigurationString = OcelotConverter.ConvertOcelotConfigurationFile(input);

File.WriteAllText(args[1], swizlyConfigurationString);