using FundooNotesAPI.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotesAPI.Models.ResponseModels
{
    public class LoginResponse
    {
        public FundooUser UserDetails { get; set; }

        public string token { get; set; }
    }
}
