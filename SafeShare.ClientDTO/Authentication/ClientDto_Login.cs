﻿using System.ComponentModel;
using SafeShare.ClientDTO.Validators;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.Authentication;

public class ClientDto_Login
{
    [NoXss]
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6, ErrorMessage ="Invalid password length")]
    public string Password { get; set; } = string.Empty;
}