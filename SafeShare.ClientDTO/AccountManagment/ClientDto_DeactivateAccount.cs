using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeShare.ClientDTO.AccountManagment
{
    public class ClientDto_DeactivateAccount
    {
        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
