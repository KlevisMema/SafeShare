﻿namespace SafeShare.ClientServices.Interfaces;

public interface IClientAuthentication_TokenRefreshService
{
    Task<bool> RefreshToken();
}