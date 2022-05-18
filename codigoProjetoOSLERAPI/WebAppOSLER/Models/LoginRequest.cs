using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER.Models
{
    [Serializable]
    public class LoginRequest
    {
        private string _user;
        private string _password;
        
        public LoginRequest(string user, string password)
        {
            User = user;
            Password = password;
        }
        
        [Required]
        public string User { 
            get => _user;
            set => _user = value; 
        }
        [Required]
        public string Password { 
            get => _password;
            set => _password = value;
        }
        
    }
}