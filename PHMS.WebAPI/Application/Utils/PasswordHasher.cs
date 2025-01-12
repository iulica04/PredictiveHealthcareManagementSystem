<<<<<<<< HEAD:PHMS.WebAPI/Domain/Services/PasswordHasher.cs
﻿namespace Domain.Services
========
﻿namespace Application.Utils
>>>>>>>> origin/iulia:PHMS.WebAPI/Application/Utils/PasswordHasher.cs
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
