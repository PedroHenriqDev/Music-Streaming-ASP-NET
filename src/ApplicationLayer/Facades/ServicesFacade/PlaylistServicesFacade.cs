using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;

namespace ApplicationLayer.Facades.ServicesFacade
{
    public class PlaylistServicesFacade
    {
        private readonly RecordService _recordService;
        private readonly VerifyService _verifyService;

        public PlaylistServicesFacade(RecordService recordService, VerifyService verifyService)
        {
            _recordService = recordService;
            _verifyService = verifyService;
        }

        public async Task<EntityQuery<Playlist>> RecordPlaylistAsnyc(PlaylistViewModel playlistVM) 
        {
            return await _recordService.CreatePlaylistAsync(playlistVM);
        }


        public EntityVerify<PlaylistViewModel> VerifyPlaylistVM(PlaylistViewModel playlistVM)
        {
            return _verifyService.VefifyPlaylistVM(playlistVM);
        }
    }
}
