using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.ComponentModel.DataAnnotations;

namespace blog_api.models.DTOs.Http
{
    /// <summary>
    /// Object that the controller is requesting
    /// </summary>
    public class UserReqDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public Int16 SecretPin { get; set; }
    }

    /// <summary>
    /// Object that the controller is responding
    /// </summary>
    public class UserResDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// Update user
    /// </summary>
    public class UserUpdateReqpDto 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PhotoUrl { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
