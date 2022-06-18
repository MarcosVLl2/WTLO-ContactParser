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
            Console.WriteLine("\n");
            Console.WriteLine("Finished");
            Console.ReadKey();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            break;
        }
        case ConsoleKey.D2:
        {
            BinAnalyser analyser = new(FileOpener());
            analyser.ReadFile();
            StreamWriter csvfile = new(File.Create("list.csv"));
            csvfile.WriteLine("id,name,namechars,state,descriptionchars,description");
            foreach (Character c in analyser.charlist)
            {
                csvfile.WriteLine($"{c.ID},{c.Name},{c.NameCharacters},{c.State},{c.Description},{c.DescriptionCharacters}");
            }
            csvfile.Close();
            Console.WriteLine("\n");
            Console.WriteLine("Finished");
            Console.ReadKey();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            break;
        }
        case ConsoleKey.D3:
        {
            List<Character> charlist = new();
            string[] csvfile = File.ReadAllLines(FileOpener());
            csvfile = csvfile.Take(new Range(1, csvfile.Length)).ToArray();
            int cont = 0;

            foreach (string c in csvfile)
            {
                cont++;
                Character charac = new();
                string[] data = c.Split(',');
                charlist.Add(new Character(Convert.ToInt32(data[0]), data[1], Convert.ToByte(data[2]), data[3], data[4], Convert.ToByte(data[5])));
                Console.WriteLine($"{cont} out of {csvfile.Length} parsed");
            }
            WriteNewFriendList(charlist);
            Console.WriteLine("\n");
            Console.WriteLine("Finished");
            Console.ReadKey();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
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
                Console.WriteLine("\n");
                Console.WriteLine("Finished");
                Console.ReadKey();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            break;
        }
        case ConsoleKey.D5:
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            break;
        }
}

string FileOpener()
{
    Console.WriteLine("\n");
    Console.Write("File: ");
    string f = Console.ReadLine();
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
    foreach (Character character in charlist)
    {
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
    }
    writer.Close();
}

