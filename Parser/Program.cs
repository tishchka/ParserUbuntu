using Parser;

while (true)
{
    Console.WriteLine("Введите путь к файлу: ");
    string path = Console.ReadLine();
    if (IsValidFile(path))
    {
        Analysis analysis = new Analysis(path);
        analysis.Analyze();

        Console.ReadLine();
        Console.Clear();
    }
    else
    {
        Console.WriteLine("Введите корректный путь к файлу.");
        Thread.Sleep(1000);
        Console.Clear();
    }
}




bool IsValidFile(string path) => string.IsNullOrWhiteSpace(path) is false &&
    path.EndsWith(".txt") && File.Exists(path);