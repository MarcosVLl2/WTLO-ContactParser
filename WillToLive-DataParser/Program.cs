BinAnalyser analyser = new(FileOpener());

Console.WriteLine("\n###################\nSelect Option\n1: Display File\n2: Parse and Export File (csv)\n3: Combine Files\n4: Exit\n###################\n");
Console.Write("Option: ");
var key = Console.ReadKey();
switch (key.Key)
{
        case ConsoleKey.D1:
        {
            analyser.ReadFile();
            Console.Write("\n\nPrint in order of ID (Y/N)?");
            var k = Console.ReadKey().Key;
            if(k == ConsoleKey.Y)
            {
                analyser.charlist = analyser.charlist.OrderBy(x => x.GetTotalID()).ToList();
            }
            else if(k != ConsoleKey.N)
            {
                Console.WriteLine("Did not understand, will print in order or ID");
                Thread.Sleep(1000);
                analyser.charlist = analyser.charlist.OrderBy(x => x.GetTotalID()).ToList();
            }
            Console.WriteLine();
            foreach (Character c in analyser.charlist)
            {
                Console.WriteLine($"ID: {c.GetTotalID()} | Name: {c.Name} ({c.NameCharacters}) | Status: {c.State} | Description: {c.Description} ({c.DescriptionCharacters})");
            }
            Console.WriteLine("\n");
            Console.WriteLine("Finished");
            Console.ReadKey();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            break;
        }
        case ConsoleKey.D2:
        {
            analyser.ReadFile();
            StreamWriter csvfile = new(File.Create("list.csv"));
            csvfile.WriteLine("id,name,state,description");
            foreach (Character c in analyser.charlist)
            {
                csvfile.WriteLine($"{c.GetTotalID()},{c.Name},{c.State},{c.Description}");
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
                    bool exists = false;
                    foreach (Character b in charlist)
                    {
                        if (b.GetTotalID() == a.GetTotalID())
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (exists)
                    {
                        Character? selectedchar = charlist.Find(x => x.GetTotalID() == a.GetTotalID());
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
                charlist = charlist.OrderBy(x => x.GetTotalID()).ToList();
                Console.WriteLine("\n");
                WriteNewFriendList(charlist);
                Console.WriteLine("\n");
                Console.WriteLine("Finished");
                Console.ReadKey();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            break;
        }
        case ConsoleKey.D4:
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            break;
        }
}

string FileOpener()
{
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
        writer.Write(character.ID[0]);
        writer.Write(character.ID[1]);
        writer.Write(character.ID[2]);
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

class BinAnalyser
{
    public List<Character> charlist = new();
    private readonly string file;
    private byte[] bytelist = Array.Empty<byte>();
    public BinAnalyser(string file)
    {
        this.file = file;
        bytelist = File.ReadAllBytes(file);
    }
    // Reads the file and puts all data into charlist
    public bool ReadFile()
    {
        if (File.Exists(file))
        {
            int index = 0;
            while (true)
            {
                try
                {
                    charlist.Add(ReadCharacter(bytelist, index, out index));
                }
                catch (IndexOutOfRangeException)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private Character ReadCharacter(byte[] file, int index, out int finalindex)
    {
        // new character
        Character character = new();
        
        // defines if it's a friend or an enemy
        switch (file[index])
        {
            case 1:
                {
                    character.State = "Friend";
                    break;
                }
            case 2:
                {
                    character.State = "Enemy";
                    break;
                }
        }

        // extract the User ID (Identifier in game, this corresponds to the # and a number below the Alpha text)
        index++;
        character.ID[0] = file[index];
        index++;
        character.ID[1] += file[index];
        index++;
        character.ID[2] += file[index];
        index += 6;

        // extracts the amount of letters the name is going to have (thank you devs)
        character.NameCharacters = file[index];
        index += 4;

        // extracts the name, it is saved in 2 bytes with UTF32 config (russian and chinese characters also have to be input)
        for (int i = 0; i < character.NameCharacters; i++)
        {
            character.Name += char.ConvertFromUtf32(file[index] + file[index + 1] * 256);
            index += 2;
        }

        // extracts the amounts of letters the description is going to have (CRLF = Carriage Return/Line Feed count as two characters as its Carriage Return + Line Feed)
        character.DescriptionCharacters = Convert.ToInt32(file[index]);
        index += 4;

        // extracts the description of the character
        if (character.DescriptionCharacters != 0)
        {
            for (int i = 0; i < character.DescriptionCharacters; i++)
            {
                character.Description += char.ConvertFromUtf32(file[index] + file[index + 1] * 256);
                index += 2;
            }
        }

        finalindex = index;
        return character;
    }
}

class Character
{
    public byte[] ID { get; set; } = new byte[3];
    public string? State { get; set; } = "";
    public string? Name { get; set; } = "";
    public byte NameCharacters { get; set; } = 0;
    public int DescriptionCharacters { get; set; } = 0;
    public string? Description { get; set; } = "";
    public Character() { }
    public int GetTotalID()
    {
        return ID[0] + ID[1] * 256 + ID[2] * 65536;
    }
    public override bool Equals(object? obj)
    {
        Character charac = (Character)obj;
        return charac.GetTotalID() == GetTotalID();
    }
}