using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces;

public interface IGenerateIntelliTextService
{
    Task<string[]> GenerateListenerDescriptionAsync(Listener listener);

    Task<string[]> GenerateArtistDescriptionAsync(Artist artist);
}
