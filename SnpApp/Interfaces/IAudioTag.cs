namespace SnpApp.Interfaces;

public interface IAudioTag
{
    string Artist     { get; set; }
    string Title { get; set; }
    string  Album { get; set; }
    string  Genre { get; set; }
}