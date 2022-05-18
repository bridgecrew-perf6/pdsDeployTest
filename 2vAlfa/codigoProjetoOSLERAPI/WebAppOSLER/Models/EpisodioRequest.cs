using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER.Models
{
    public class EpisodioRequest
    {
        private string _codEpisodio;
        private string _descricao;
        private DateTime _dataNascimento;
        private string _idSns;
        private ulong _idCor;
        private ulong _idIdioma;
        private ulong _idNacionalidade;

        
        public EpisodioRequest(string codEpisodio, string descricao, DateTime dataNascimento, string idSns, ulong idCor, ulong idIdioma, ulong idNacionalidade)
        {
            CodEpisodio = codEpisodio;
            Descricao = descricao;
            DataNascimento = dataNascimento;
            IdSns = idSns;
            IdCor = idCor;
            IdIdioma = idIdioma;
            IdNacionalidade = idNacionalidade;
        }
        
        
        [Required]
        public string CodEpisodio
        {
            get => _codEpisodio;
            set => _codEpisodio = value;
        }

        [Required]
        public string Descricao
        {
            get => _descricao;
            set => _descricao = value;
        }

        [Required]
        public DateTime DataNascimento
        {
            get => _dataNascimento;
            set => _dataNascimento = value;
        }

        [Required]
        public string IdSns
        {
            get => _idSns;
            set => _idSns = value;
        }

        [Required]
        public ulong IdCor
        {
            get => _idCor;
            set => _idCor = value;
        }

        [Required]
        public ulong IdIdioma
        {
            get => _idIdioma;
            set => _idIdioma = value;
        }

        [Required]
        public ulong IdNacionalidade
        {
            get => _idNacionalidade;
            set => _idNacionalidade = value;
        }
    }
}