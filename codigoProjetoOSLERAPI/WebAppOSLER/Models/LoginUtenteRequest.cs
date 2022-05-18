using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER.Models
{
    [Serializable]
    public class LoginUtenteRequest
    {
        private string _codEpisodio;
        private short _pin4;

        public LoginUtenteRequest(string codEpisodio, short pin4)
        {
            CodEpisodio = codEpisodio ?? throw new ArgumentNullException(nameof(codEpisodio));
            Pin4 = pin4;
        }

        [Required]
        public string CodEpisodio
        {
            get => _codEpisodio;
            set => _codEpisodio = value;
        }

        [Required]
        public short Pin4
        {
            get => _pin4;
            set => _pin4 = value;
        }
    }
}