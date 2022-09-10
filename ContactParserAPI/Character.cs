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
        public Character(int id, string? state, string? name, byte namechars, byte descchars, string? description)
        {
            this.ID = id;
            this.State = state;
            this.Name = name;
            this.NameCharacters = (string.IsNullOrEmpty(name)) ? this.NameCharacters = 0 : this.NameCharacters = (byte)Name.Length;
            this.DescriptionCharacters = descchars;
            this.Description = description;
        }
        public void SetByteID(byte[] id)
        {
            ID = id[0] + id[1] * 256 + id[2] * 65536;
        }
        public byte[] GetByteID()
        {
            return BitConverter.GetBytes(ID);
        }
        public override bool Equals(object? obj)
        {
            try
            {
                Character? c = (Character?)obj;
                return c?.ID == ID;
            }
            catch (NullReferenceException n_ex) { throw n_ex; }
            catch (Exception) { throw; }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
