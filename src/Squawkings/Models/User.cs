using NPoco;

namespace Squawkings.Models
{
    [TableName("Users")]
    [PrimaryKey("UserId", AutoIncrement = true)]
    public class User
    {
        [Column("UserId")]
        public int UserId { get; set; }

        [Column("UserName")]
        public string UserName { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; }

        [Column("LastName")]
        public string LastName { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("AvatarUrl")]
        public string AvatarUrl { get; set; }

        [Column("bio")]
        public string Bio { get; set; }

        [Column("IsGravatar")]
        public bool IsGravatar { get; set ; }
    }
}