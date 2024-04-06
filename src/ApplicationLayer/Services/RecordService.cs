﻿using ApplicationLayer.Factories;
using ApplicationLayer.ViewModels;
using DataAccessLayer.Sql;
using DomainLayer.Interfaces;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.Services
{
    public class RecordService
    {
        private readonly ILogger<RecordService> _logger;
        private readonly ConnectionDb _connectionDb;
        private readonly VerifyService _verifyService;
        private readonly EncryptService _encryptService;
        private readonly ModelFactory _modelFactory;

        public RecordService(
            ILogger<RecordService> logger,
            ConnectionDb connectionDb,
            VerifyService verifyService,
            EncryptService encryptService,
            ModelFactory modelFactory)
        {
            _logger = logger;
            _connectionDb = connectionDb;
            _verifyService = verifyService;
            _encryptService = encryptService;
            _modelFactory = modelFactory;
        }

        public async Task<EntityQuery<T>> CreateUserAsync<T>(RegisterUserViewModel userVM)
            where T : class, IUser<T>, new()
        {
            string userType = typeof(T).Name;
            T user = _modelFactory.FacUser<T>(Guid.NewGuid().ToString(), userVM.Name, userVM.Email, _encryptService.EncryptPasswordSHA512(userVM.Password), userVM.PhoneNumber, userVM.BirthDate, DateTime.Now);
            try
            {
                await _connectionDb.RecordUserAndUserGenresAsync(user, _modelFactory.FacUserGenres<T>(user.Id, userVM.SelectedGenreIds));
                return new EntityQuery<T>(true, $"{userType} created successfully", user, DateTime.Now);
            }
            catch (RecordAssociationException ex)
            {
                _logger.LogError("The error occurred when creating the object in the associated table, Genre and User");
                throw new RecordException<EntityQuery<T>>($"The error occurred when creating the object associated with the genre, contact the developer, error in sql: {ex.Message}", new EntityQuery<T>(false, $"Unable to create a {userType}", user, DateTime.Now));
            }
            catch (Exception ex)
            {
                _logger.LogError("Brutal error in method CreateUserAsync");
                throw new RecordException<EntityQuery<T>>($"This error occurred while registration was happening, {ex.Message}", new EntityQuery<T>(false, "Unable to create a artist", user, DateTime.Now));
            }
        }
    }
}
