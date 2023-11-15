namespace SnpCore.Interfaces;

public interface IAudio
{
    string   FileName { get; set; }
    ulong   FileSize   { get; set; }
    string   Hash     { get; set; }
    ulong  Duration   { get; set; }
}