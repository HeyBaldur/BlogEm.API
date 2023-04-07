using AutoMapper;
using blog_api.models;
using blog_api.models.DTOs.Http;
using blog_api.models.v1;
using blog_api.services.Exceptions;
using blog_api.services.Helpers;
using Microsoft.Security.Application;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
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
                var cleanUser = SanitizeInput<UserReqDto>(user);

                // Map the request to user
                var userToRepo = _mapper.Map<User>(cleanUser);

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

            var cleanUser = SanitizeInput<UserUpdateReqpDto>(user);

            User currUser = await GetUserAsync(cleanUser.Id).ConfigureAwait(false);

            string email = string.Empty;

            if (currUser == null)
            {
                throw new CathedException("User does not exist");
            }

            // User wants to change email address
            if (currUser.Email != cleanUser.Email)
            {
                email = (await EmailExist(cleanUser.Email).ConfigureAwait(false)) ? currUser.Email : cleanUser.Email;
            }

            // Replace the document except for the ID
            var updateFilter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(user.Id));

            var replacement = new User
            {
                LastModification = DateTime.UtcNow,
                FirstName = cleanUser.FirstName,
                LastName = cleanUser.LastName,
                PhotoUrl = cleanUser.PhotoUrl,
                UserName = cleanUser.UserName,
                CreatedOn = currUser.CreatedOn,
                Id = cleanUser.Id,
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

        /// <summary>
        /// SanitizeInput
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static T SanitizeInput<T>(T data) where T : class
        {
            var sanitizer = new Ganss.Xss.HtmlSanitizer();
            Type type = data.GetType();
            PropertyInfo[] properties = type.GetProperties();
            object[] sanitizedValues = new object[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].PropertyType == typeof(string))
                {
                    string stringValue = (string)properties[i].GetValue(data);
                    string sanitized = sanitizer.Sanitize(stringValue);
                    properties[i].SetValue(data, sanitized);
                }
                else
                {
                    properties[i].SetValue(data, properties[i].GetValue(data));
                }
            }

            return data;
        }
    }
}
