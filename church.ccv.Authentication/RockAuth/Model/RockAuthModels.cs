﻿
using System;

namespace church.ccv.Authentication.RockAuth.Model
{
    public enum LoginResponse
    {
        Invalid,
        LockedOut,
        Confirm,
        Success
    }

    [Serializable]
    public class LoginData
    {
        public string Username;
        public string Password;
        public string Persist;
    }
    
    [Serializable]
    public class PersonWithLoginModel
    {
        public string FirstName;
        public string LastName;
        public string Email;

        public string Username;
        public string Password;

        public string ConfirmAccountUrl;
        public string AccountCreatedEmailTemplateGuid;
        public string AppUrl;
        public string ThemeUrl;
    }

    [Serializable]
    public class CreateLoginModel
    {
        public enum Response
        {
            Created, //Used if we did create a login for the user
            Emailed, //Used if we emailed the user existing credentials
            Failed //Used if something horrible happened
        }

        public int PersonId;
        
        public string Username;
        public string Password;

        public string ConfirmAccountUrl;
        public string ConfirmAccountEmailTemplateGuid;
        public string ForgotPasswordEmailTemplateGuid;
        
        public string AppUrl;
        public string ThemeUrl;
    }

    [Serializable]
    public class DuplicatePersonInfo
    {
        public int Id;
        public string FullName;
        public string Birthday;
        public bool HasUsernames;
    }
}
