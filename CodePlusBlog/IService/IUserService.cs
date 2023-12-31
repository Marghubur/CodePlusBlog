﻿using CodePlusBlog.Model;

namespace CodePlusBlog.IService
{
    public interface IUserService
    {
        Task<LoginResponse> AuthenticateUserService(User user);
        Task<string> RegisterUserService(User user);
        Task<string> ForgotPasswordService(string email);
        Task<string> ChangePasswordService(User user);
        Task<string> GenerateOtp(string email);
    }
}
