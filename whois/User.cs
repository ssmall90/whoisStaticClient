using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whois
{
    public class User
    {
        public User() { }
        public String UserID { get; set; }
        public String Surname { get; set; }
        public String Forenames { get; set; }
        public String Title { get; set; }
        public String Position { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public String Location { get; set; }

         public User(
             String userID,
             String surname,
             String forenames,
             String title,
             String position,
             String phone,
             String email,
             String location)
        {
            UserID = userID;
            Surname = surname;
            Forenames = forenames;
            Title = title;
            Position = position;
            Phone = phone;
            Email = email;
            Location = location;

        }
    }
}
