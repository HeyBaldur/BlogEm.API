using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace blog_api.models.v1
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModification { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public Int16 SecretPin { get; set; }

        public User()
        {
            CreatedOn = DateTime.Now;
        }
    }
}
