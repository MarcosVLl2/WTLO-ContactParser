using ContactParserAPI;

Console.WriteLine("\n###################\nSelect Option\n1: Display File\n2: Parse and Export File (bin to csv)\n3: Import File (csv to bin)\n4: Combine Files\n5: Exit\n###################\n");
Console.Write("Option: ");
var key = Console.ReadKey();
switch (key.Key)
{
        case ConsoleKey.D1:
        {
            BinAnalyser analyser = new(FileOpener());
            analyser.ReadFile();
            Console.Write("\n\nPrint in order of ID (Y/N)?");
            var k = Console.ReadKey().Key;
            if(k == ConsoleKey.Y)
            {
                analyser.charlist = analyser.charlist.OrderBy(x => x.ID).ToList();
            }
            else if(k != ConsoleKey.N)
            {
                Console.WriteLine("Did not understand, will print in order or ID\n");
                Thread.Sleep(1000);
                analyser.charlist = analyser.charlist.OrderBy(x => x.ID).ToList();
            }
            Console.WriteLine();
            foreach (Character c in analyser.charlist)
            {
                Console.WriteLine($"ID: {c.ID} | Name: {c.Name} ({c.NameCharacters}) | Status: {c.State} | Description: {c.Description} ({c.DescriptionCharacters})");
            }
            Console.WriteLine("\n\nFinished");
            Console.ReadKey();
            KillProcess();
            break;
        }
        case ConsoleKey.D2:
        {
            BinAnalyser analyser = new(FileOpener());
            analyser.ReadFile();
            var csvWriter = new CsvHelper.CsvWriter(new StreamWriter(File.Create("list.csv")), new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            { ShouldQuote = args => args.Row.Index == 1, Delimiter = ",", AllowComments = false });

            csvWriter.WriteHeader<Character>();
            csvWriter.NextRecord();
            csvWriter.WriteRecords(analyser.charlist);
            csvWriter.Flush(); csvWriter.Dispose();
            Console.WriteLine("\n\nFinished");
            Console.ReadKey();
            KillProcess();
            break;
        }
        case ConsoleKey.D3:
        {
            List<Character> charlist = new();
            var csvReader = new CsvHelper.CsvReader(File.OpenText(FileOpener()), new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            { ShouldQuote = args => args.Row.Index == 1, Delimiter = ",", AllowComments = false });

            WriteNewFriendList(csvReader.GetRecords<Character>().ToList());
            Console.WriteLine("\n\nFinished");
            Console.ReadKey();
            KillProcess();
            break;
        }
        case ConsoleKey.D4:
        {
            List<Character> charlist = new();
            BinAnalyser analyser = new(FileOpener());
            analyser.ReadFile();
            charlist = analyser.charlist;

            tryagain:
            Console.Write("Another File? (Y/N) ");
            var keypressed = Console.ReadKey();
            if (keypressed.Key == System.ConsoleKey.Y)
            {
                analyser = new BinAnalyser(FileOpener());
                analyser.ReadFile();
                List<Character> newcharlist = charlist;

                foreach (Character a in analyser.charlist)
                {
                    Character? selectedchar = charlist.Find(x => x.ID == a.ID);
                    if (selectedchar != null)
                    {
                        newcharlist.Remove(a);
                        if (a.DescriptionCharacters > selectedchar.DescriptionCharacters)
                        {
                            newcharlist.Add(a);
                        }
                        else
                        {
                            newcharlist.Add(selectedchar);
                        }
                    }
                    else
                    {
                        newcharlist.Add(a);
                    }
                }
                charlist = newcharlist;
                goto tryagain;
            }
            else if (keypressed.Key == System.ConsoleKey.N)
            {
                charlist = charlist.OrderBy(x => x.ID).ToList();
                Console.WriteLine("\n");
                WriteNewFriendList(charlist);
                Console.WriteLine("\n\nFinished");
                Console.ReadKey();
                KillProcess();
            }
            break;
        }
        case ConsoleKey.D5:
        {
            
            break;
        }
}

void KillProcess()
{
    System.Diagnostics.Process.GetCurrentProcess().Kill();
}

string FileOpener()
{
    Console.WriteLine("\n");
    Console.Write("File: ");
    string? f = Console.ReadLine();
    if (File.Exists(f))
    {
        return f;
    }
    System.Diagnostics.Process.GetCurrentProcess().Kill();
    return "";
}

void WriteNewFriendList(List<Character> charlist)
{
    BinaryWriter writer = new(File.OpenWrite("File.bin"), System.Text.Encoding.ASCII, false);
    int cont = 0;
    foreach (Character character in charlist)
    {
        cont++;
        switch (character.State)
        {
            case "Friend":
                {
                    writer.Write((byte)1); break;
                }
            case "Enemy":
                {
                    writer.Write((byte)2); break;
                }
        }
        writer.Write(character.GetByteID());
        for (int i = 0; i <= 4; i++) writer.Write((byte)0);
        writer.Write(character.NameCharacters);
        for (int i = 0; i <= 2; i++) writer.Write((byte)0);
        foreach (char letter in character.Name)
        {
            var bytes = BitConverter.GetBytes(letter);
            writer.Write(bytes[0]);
            writer.Write(bytes[1]);
        }
        if (character.DescriptionCharacters == 0)
        {
            for (int i = 0; i <= 3; i++) writer.Write((byte)0);
        }
        else
        {
            writer.Write(character.DescriptionCharacters);
            foreach (char letter in character.Description)
            {
                var bytes = BitConverter.GetBytes(letter);
                writer.Write(bytes[0]);
                writer.Write(bytes[1]);
            }
        }
        WTLO_ContactParser.ConsoleUtility.WriteProgressBar((cont / charlist.Count) * 100, true);
    }
    writer.Close();
}

