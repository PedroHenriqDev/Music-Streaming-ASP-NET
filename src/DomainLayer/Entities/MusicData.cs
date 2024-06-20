namespace DomainLayer.Entities;

public class MusicData
{
    public string Id { get; set; }
    public byte[] Picture { get; set; }
    public byte[] Audio { get; set; }

    public MusicData(string id, byte[] picture, byte[] audio)
    {
        Id = id;
        Picture = picture;
        Audio = audio;
    }
}
