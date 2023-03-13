using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCode.Models.Requests
{
    public class GetDepListRequest
    {
        public int DepId { get; set; }
        public string DepName { get; set;  }
    }
}
