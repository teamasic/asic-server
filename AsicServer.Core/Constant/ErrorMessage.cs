using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.Constant
{
    public class ErrorMessage
    {
        public static string PASSWORD_EMPTY = "Password cannot be empty";
        public static string USERNAME_EMPTY = "Username cannot be empty";
        public static string CREDENTIALS_NOT_MATCH = "Username or password is not matched";

        public static string PASSWORD_LENGTH_NOT_VALID = $"Password length must be at least {Constants.MIN_PASSWORD_LENGTH} and at max {Constants.MAX_PASSWORD_LENGTH} characters";
        public static string USERNAME_EXISTED = "Username is already existed";
        public static string BIRTHDATE_NOT_VALID = "Your birthdate is not valid";
        public static string EMAIL_WRONG_FORMAT = "Email is wrong format";
        public static string PHONE_WRONG_FORMAT = "Phone is wrong format";
        public static string ROLES_EMPTY = "Role cannot be empty";
        public static string ROLES_NOT_EXISTED = "Role is not existed";

        public static string INVALID_USERS = "Invalid users";
        public static string USERS_LACK_OF_IMAGES = "Users lack of images";
        public static string USER_EMAIL_NOT_FOUND = "Cannot find user with email {0}";
        public const string NOT_AUTHORIZED_USER = "Not authorized user";
    }
}
