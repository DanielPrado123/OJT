using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCode.Models.Requests
{
    public class GetUserListRequest
    {
        public int UserId { get; set; }
        public string FirstName { get; set;  }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
    }
}
