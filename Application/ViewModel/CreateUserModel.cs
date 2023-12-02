using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class CreateUserModel
    {
        public string? Id { get; set; }
        public string Username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string passwordConfirm { get; set; }

    }
}
