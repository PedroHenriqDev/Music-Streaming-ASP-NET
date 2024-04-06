using DomainLayer.Entities;

namespace ApplicationLayer.Services
{
    public class GenerateIntelliTextService
    {
        private readonly SearchService _searchService;

        public GenerateIntelliTextService(SearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<string[]> GenerateListenerDescriptionAsync(Listener listener)
        {
            string allGenres = string.Empty;
            var genres = await _searchService.FindUserGenresAsync(listener);
            int amountGenres = genres.Count() + 1;
            string[] descriptions = new string[amountGenres];
            int i = 0;

            foreach (var genre in genres)
            {
                descriptions[i] = $"I appreciate the {genre.Name} musical style.";
                i++;

                if (i == genres.Count())
                {
                    allGenres += $"and {genre.Name}.";
                }
                else
                {
                    allGenres += $"{genre.Name}, ";
                }
            }
            descriptions[amountGenres - 1] = $"I appreciate these musical style, {allGenres}";
            return descriptions;
        }

        public async Task<string[]> GenerateArtistDescriptionAsync(Artist artist)
        {
            string allGenres = string.Empty;
            var genres = await _searchService.FindUserGenresAsync(artist);
            int amountGenres = genres.Count() + 1;
            string[] descriptions = new string[amountGenres];
            int i = 0;

            foreach (var genre in genres)
            {
                descriptions[i] = $"I produce {genre.Name} music style";
                i++;

                if (i == genres.Count())
                {
                    allGenres += $"and {genre.Name}.";
                }
                else
                {
                    allGenres += $"{genre.Name}, ";
                }
            }
            descriptions[amountGenres - 1] = $"I produce {allGenres} musical styles";
            return descriptions;
        }
    }
}
