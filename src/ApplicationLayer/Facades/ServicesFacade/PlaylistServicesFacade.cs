﻿using ApplicationLayer.ViewModels;
using ApplicationLayer.Services;
using DomainLayer.Entities;

namespace ApplicationLayer.Facades.ServicesFacade
{
    public class PlaylistServicesFacade
    {
        private readonly RecordService _recordService;
        private readonly VerifyService _verifyService;
        private readonly SearchService _searchService;

        public PlaylistServicesFacade(RecordService recordService, VerifyService verifyService, SearchService searchService)
        {
            _recordService = recordService;
            _verifyService = verifyService;
            _searchService = searchService;
        }

        public async Task<EntityQuery<Playlist>> RecordPlaylistAsnyc(PlaylistViewModel playlistVM) 
        {
            return await _recordService.CreatePlaylistAsync(playlistVM);
        }


        public EntityVerify<PlaylistViewModel> VerifyPlaylistVM(PlaylistViewModel playlistVM)
        {
            return _verifyService.VefifyPlaylistVM(playlistVM);
        }

        public async Task<Listener> FindCurrentListenerAsync() 
        {
            return await _searchService.FindCurrentUserAsync<Listener>();
        }
    }
}
