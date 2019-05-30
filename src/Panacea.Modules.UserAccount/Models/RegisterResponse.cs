using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.UserAccount.Models
{
    [DataContract]
    public class RegisterResponse
    {
        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }
    }
}
