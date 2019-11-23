using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VotingIrregularities.Api.Models {
    public class AuthenticateUserRequest {
        [Required]
        public string User { get; set; }
        [Required]
        public string Password { get; set; }
        public string UniqueId { get; set; }
    }
}