using NttProject.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NttProject.Data
{
    public static class UserRepo
    {

        public static UserDto LoggedUser { get; set; } = null;

        public static bool login(UserDto userDto) 
        {
            LoggedUser = userDto;
            return true;
        }

        public static void logout()
        {
            LoggedUser = null;
        }
    }
}
