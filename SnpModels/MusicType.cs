namespace Snp.Models
{
   
    public class MusicType : IdObject
    { 
        
        public string Name { get; set; }
        
        public string ImageUrl { get; set; }
        public uint FileCount { get; set; }
        public int  Size { get; set; }
        public int  TotalDuration { get; set; }
        
        public override string ToString() => $"{Name} \n{ImageUrl} \n{FileCount} \n{Size} \n{TotalDuration}";
    }
}