﻿using AutoMapper;
using blog_api.models;
using blog_api.models.DTOs.Http;
using blog_api.models.v1;
using blog_api.services.Exceptions;
using blog_api.services.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace blog_api.services.services
{
    public class UserService: IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMapper _mapper;

        public UserService(
            IUserDatabaseSettings settings,
            IMapper mapper)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UserCollection);
            _mapper = mapper;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<UserResDto> CreateAsync(UserReqDto user)
        {
            try
            {
                // Map the request to user
                var userToRepo = _mapper.Map<User>(user);

                // Create a password hash and salt for the text plain password prop
                PasswordManager.CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
                userToRepo.PasswordHash = passwordHash;
                userToRepo.PasswordSalt = passwordSalt;

                await _users.InsertOneAsync(userToRepo);

                // Map the user to reponse
                var response = _mapper.Map<UserResDto>(userToRepo);

                return response;
            }
            catch (Exception ex)
            {
                throw new CathedException(ex.Message);
            }
        }

        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="CathedException"></exception>
        public async Task<UserResDto> UpdateAsync(UserUpdateReqpDto user)
        {
            // TODO: Validate the passsword is correct

            User currUser = await GetUserAsync(user.Id).ConfigureAwait(false);

            string email = string.Empty;

            if (currUser == null)
            {
                throw new CathedException("User does not exist");
            }

            // User wants to change email address
            if (currUser.Email != user.Email)
            {
                email = (await EmailExist(user.Email).ConfigureAwait(false)) ? currUser.Email : user.Email;
            }

            // Replace the document except for the ID
            var updateFilter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(user.Id));

            var replacement = new User
            {
                LastModification = DateTime.UtcNow,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhotoUrl = user.PhotoUrl,
                UserName = user.UserName,
                CreatedOn = currUser.CreatedOn,
                Id = user.Id,
                Email = email,
                PasswordHash = currUser.PasswordHash,
                PasswordSalt = currUser.PasswordSalt,
                SecretPin = currUser.SecretPin
            };

            var options = new ReplaceOptions { IsUpsert = false };

            var result = await _users.ReplaceOneAsync(updateFilter, replacement, options);

            if (result.IsAcknowledged)
            {
                // Map the user to reponse
                UserResDto response = _mapper.Map<UserResDto>(replacement);

                return response;
            }

            return null;
        }

        /// <summary>
        /// Validate if the email exist
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private async Task<bool> EmailExist(string email) => 
            await _users.Find(user => user.Email == email).AnyAsync();

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<User> GetUserAsync(string id) =>
            await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
    }
}
