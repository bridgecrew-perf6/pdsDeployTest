using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER.Models
{
    public class DadosRequest
    {
        private ulong _idTipoDado;
        private string _valor;

        public DadosRequest(ulong idTipoDado, string valor)
        {
            IdTipoDado= idTipoDado;
            Valor = valor;
        }
        
        [Required]
        public ulong IdTipoDado
        {
            get => _idTipoDado;
            set => _idTipoDado = value;
        }
        
        [Required]
        public string Valor
        {
            get => _valor;
            set => _valor = value;
        }
    }
}