using AutoMapper;
using blog_api.models;
using blog_api.models.DTOs.Http;
using blog_api.models.v1;
using blog_api.services.Exceptions;
using blog_api.services.Helpers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blog_api.services.services
{
    public class UserService 
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

        public List<User> Get() =>
            _users.Find(user => true).ToList();

        public User Get(string id) =>
            _users.Find<User>(user => user.Id == id).FirstOrDefault();

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<UserResDto> CreateAsync(UserReqDto user)
        {
            try
            {
                // Validate if the request has corrupted data
                if (HasCorruptedProps(user))
                {
                    // Throw a new exception if there is information or any potential XSS attack
                    throw new CorruptedException("The information contains corrupted information. Information has been saved and report has been generated.");
                }

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

        public void Update(string id, User userIn) =>
            _users.ReplaceOne(user => user.Id == id, userIn);

        public void Remove(User userIn) =>
            _users.DeleteOne(user => user.Id == userIn.Id);

        public void Remove(string id) =>
            _users.DeleteOne(user => user.Id == id);

        /// <summary>
        /// Validate if the has corrupted data
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool HasCorruptedProps(UserReqDto user)
        {
            Type userType = user.GetType();
            PropertyInfo[] properties = userType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(user);
                bool xssResult = SanitizeHandler.SanitizePropForXss(value);
                if (!xssResult)
                {
                    // Contains corrupted data
                    return true;
                }
            }

            return false;
        }
    }
}
