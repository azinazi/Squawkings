using NPoco;

namespace Squawkings.Models
{
    [TableName("UserSecurityInfo")]
    [PrimaryKey("UserId, autoIncrement = false")]
    public class UserSecurityInfo
    {
        [Column("UserId")]
        public int UserId { get; set; }

        [Column("Password")]
        public string Password { get; set; }
    }
}