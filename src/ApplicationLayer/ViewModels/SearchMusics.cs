﻿using DomainLayer.Entities;

namespace ApplicationLayer.ViewModels
{
    public class SearchMusics
    {
        public Listener Listener { get; set; }
        public IEnumerable<CompleteMusicViewModel>? MusicsSuggestion { get; set; }
        public IEnumerable<CompleteMusicViewModel> Musics { get; set; }
    }
}