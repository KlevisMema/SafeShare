﻿namespace SafeShare.DataTransormObject.SafeShareApi.GroupManagment;

public class DTO_UsersGroupDetails
{
    public byte[]? UserImage { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public decimal Balance { get; set; }
}