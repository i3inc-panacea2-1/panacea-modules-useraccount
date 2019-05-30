using Panacea.Multilinguality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.UserAccount.Models
{
    [DataContract]
    public class SecurityQuestion : Translatable
    {
        [IsTranslatable]
        [DataMember(Name = "question")]
        public string Question
        {
            get => GetTranslation();
            set => SetTranslation(value);
        }
    }
}
