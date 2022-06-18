using System;

namespace ContactParserAPI
{
    public class Character
    {
        public int ID { get; set; } = -1;
        public string? State { get; set; } = "";
        public string? Name { get; set; } = "";
        public byte NameCharacters { get; set; } = 0;
        public byte DescriptionCharacters { get; set; } = 0;
        public string? Description { get; set; } = "";
        public Character() { }
        public Character(int id, string? name, byte namechars, string? state, string? description, byte descchars)
        {
            this.ID = id;
            this.State = state;
            this.Name = name;
            this.NameCharacters = namechars;
            this.DescriptionCharacters = descchars;
            this.Description = description;
        }
        public void SetByteID(byte[] id)
        {
            this.ID = id[0] + id[1] * 256 + id[2] * 65536;
        }
        public byte[] GetByteID()
        {
            return BitConverter.GetBytes(ID);
        }
        public override bool Equals(object? obj)
        {
            Character c = (Character)obj;
            return c.ID == this.ID;
        }
    }
}
