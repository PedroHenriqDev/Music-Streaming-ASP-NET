using Datas.Sql;
using Microsoft.AspNetCore.Http;
using Models.ConcreteClasses;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace Services
{
    public class UserPageService
    {
        public ArtistPageViewModel BuildArtistViewModelAsync(Artist artist) 
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
