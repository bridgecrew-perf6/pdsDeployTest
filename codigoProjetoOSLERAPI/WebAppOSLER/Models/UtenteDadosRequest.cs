using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER.Models
{
    [Serializable]
    public class UtenteDadosRequest
    {
        private ulong _idEpisodio;
        private string _idSns;
        private DateTime _dataNascimento;

        public UtenteDadosRequest(ulong idEpisodio, string idSns, DateTime dataNascimento)
        {
            IdEpisodio = idEpisodio;
            IdSns = idSns;
            DataNascimento = dataNascimento;
        }

        [Required]
        public ulong IdEpisodio
        {
            get => _idEpisodio;
            set => _idEpisodio = value;
        }

        [Required]
        public string IdSns
        {
            get => _idSns;
            set => _idSns = value;
        }
        
        [Required]
        public DateTime DataNascimento
        {
            get => _dataNascimento;
            set => _dataNascimento = value;
        }
    }
}