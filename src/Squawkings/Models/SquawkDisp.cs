using System;
using NPoco;

namespace Squawkings.Models
{
    public class SquawkTemplate :TemplateBuilder
    {
        public  SquawkTemplate()
        {
  
            temp1 = AddTemplate("select u.FirstName,u.LastName,u.UserName,s.Content,s.CreatedAt,u.AvatarUrl "
                                     + "from Users u inner join Squawks s on s.UserId = u.UserId "
                                     + "where /**where**/ order by s.SquawkId desc");
        }

    }

    public class SquawkDisp
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AvatarUrl { get; set; }
        public string Bio { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}