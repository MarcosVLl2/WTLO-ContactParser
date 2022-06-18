namespace ContactParserAPI
{
    public class BinAnalyser
    {
        public List<Character> charlist = new();
        private readonly string file;
        private readonly byte[] bytelist = Array.Empty<byte>();
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
        private static Character ReadCharacter(byte[] file, int index, out int finalindex)
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
            character.SetByteID(new byte[] {file[index], file[index + 1], file[index + 2]});
            index += 8;

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
            character.DescriptionCharacters = file[index];
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
}