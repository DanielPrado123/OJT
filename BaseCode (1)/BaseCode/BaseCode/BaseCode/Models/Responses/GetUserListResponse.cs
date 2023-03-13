﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseCode.Models.Tables;

namespace BaseCode.Models.Responses
{
    public class GetUserListResponse
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public List<User> UsersList { get; set; }

    }
}
