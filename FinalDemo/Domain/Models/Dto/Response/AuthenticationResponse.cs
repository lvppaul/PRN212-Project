using Domain.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Dto.Response
{
    public class AuthenticationResponse
    {
        public  string UserId { get; set; }
        public string Message { get; set; } 
    }
}

