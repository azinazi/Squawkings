using NPoco;

namespace Squawkings.Models
{
    [TableName("Followers")]
    [PrimaryKey("UserId,FollowerUserId")]
    public class Followers
    {
        [Column("UserId")]
        public int? UserId { get; set; }

        [Column("FollowerUserId")]
        public int? FollowerUserId { get; set; }
    }
}