﻿using DomainLayer.Interfaces;

namespace DomainLayer.Entities
{
    public class Listener : IUser<Listener>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public byte[] PictureProfile { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime DateCreation { get; set; }
        public ICollection<FavoriteMusic> FavoritesMusics { get; set; }
        public ICollection<FavoritePlaylist> FavoritePlaylists { get; set; }

        public Listener()
        {
        }

        public Listener(
            string id,
            string name,
            string password,
            string email,
            string phoneNumber,
            DateTime birthDate,
            DateTime dateCreation)
        {
            Id = id;
            Name = name;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            BirthDate = birthDate;
            DateCreation = dateCreation;
        }
    }
}