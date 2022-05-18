using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER
{
    [Serializable]
    public class QuestionarioRequest
    {
        private string _descricao;
        private ulong _idIdioma;

        public QuestionarioRequest(string descricao, ulong idIdioma)
        {
            Descricao = descricao;
            IdIdioma = idIdioma;
        }
        
        [Required]
        public string Descricao 
        { 
            get=> _descricao;
            set => _descricao = value;
        }
        
        [Required]
        public ulong IdIdioma 
        { 
            get=> _idIdioma;
            set => _idIdioma = value;
        }
    }
}