using System.ComponentModel.DataAnnotations;
using Cronus.Source.Database.Attributes;

namespace Cronus.Source.Database.Entities
{
    [Entity("users")] 
    public class User : BaseEntity
    {
        [Column("username")]
        public string Username { get; set; }

        [Column("email")] 
        [EmailAddress]
        public string Email { get; set; }

        [Column("password")]
        [DataType(DataType.Password)] 
        public string Password { get; set; }

        [Column("accountId")]
        public string AccountId { get; set; }

        [Column("discordId")]
        public string DiscordId { get; set; }

        [Column("banned")]
        public bool Banned { get; set; }

        [Column("has_all")]
        public bool HasAll { get; set; }

        [Column("roles")]
        public string[] Roles { get; set; }
    }
}
