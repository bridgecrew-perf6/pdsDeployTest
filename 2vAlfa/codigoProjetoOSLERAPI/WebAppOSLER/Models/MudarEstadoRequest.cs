using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER.Models
{
    public class MudarEstadoRequest
    {
        private int _estado; // ou enum do estado, na eventualidade de ser criado
        private string _descricao;

        [Required]
        public int Estado
        {
            get => _estado;
            set => _estado = value;
        }
        [Required]
        public string Descricao
        {
            get => _descricao;
            set => _descricao = value;
        }

        public MudarEstadoRequest(int estado, string descricao)
        {
            Estado = estado;
            Descricao = descricao;
        }
    }
}