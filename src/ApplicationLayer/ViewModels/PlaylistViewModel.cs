﻿using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.ViewModels
{
    public class PlaylistViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile FileImage { get; set; }
        public byte[] Image { get; set; }
        public IEnumerable<CompleteMusicViewModel> Musics { get; set; }
    }
}