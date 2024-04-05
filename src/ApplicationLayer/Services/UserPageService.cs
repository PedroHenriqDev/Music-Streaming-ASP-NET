using DomainLayer.Entities;
using ApplicationLayer.ViewModels;

namespace ApplicationLayer.Services
{
    public class UserPageService
    {
        public ArtistPageViewModel BuildArtistViewModel(Artist artist) 
        {
            if(artist == null) 
            {
                throw new ArgumentNullException("Artist used in reference is null!");
            }

            return new ArtistPageViewModel 
            {
                Name = artist.Name, Description = artist.Description, PictureProfile = artist.PictureProfile 
            };
        } 
    }
}
