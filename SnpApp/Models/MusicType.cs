namespace SnpApp.Models
{
   
    public class MusicType : IdObject
    { 
        
        public string Name { get; set; } = default!;
        
        public string ImageUrl { get; set; } = default!;
        public uint FileCount { get; set; } = default!;
        public int  Size { get; set; } = default!;
        public int  TotalDuration { get; set; } = default!;
        
        public override string ToString() => $"{Name} \n{ImageUrl} \n{FileCount} \n{Size} \n{TotalDuration}";
    }
}