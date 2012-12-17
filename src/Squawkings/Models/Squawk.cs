using System;
using NPoco;

namespace Squawkings.Models
{
    [TableName("Squawks")]
    [PrimaryKey("SquawkId")]
    public class Squawk
    {
        public string Content { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}