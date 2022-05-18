using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER
{
    [Serializable]
    public class LoginResponse
    {
        private string _token;
        public LoginResponse(string token)
        {
            Token = token;
        }
        [Required]
        public string Token 
        { 
            get => _token;
            set => _token = value; 
        }
    }
}