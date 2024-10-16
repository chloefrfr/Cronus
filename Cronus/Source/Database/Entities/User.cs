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
    }
}
