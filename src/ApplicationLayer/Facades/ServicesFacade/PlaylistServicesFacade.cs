using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;

namespace ApplicationLayer.Facades.ServicesFacade
{
    public class PlaylistServicesFacade
    {
        private readonly RecordService _recordService;

        public PlaylistServicesFacade(RecordService recordService)
        {
            _recordService = recordService;
        }

        public async Task<EntityQuery<Playlist>> RecordPlaylistAsnyc(PlaylistViewModel playlistVM) 
        {
            return await _recordService.CreatePlaylistAsync(playlistVM);
        }
    }
}
