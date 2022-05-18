using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER.Models
{
    [Serializable]
    public class EpisodioQuestionarioRequest
    {
        private ulong _idQuestionario;
        private short _sequenciaQuestionario;

        public EpisodioQuestionarioRequest(ulong idQuestionario, short sequenciaQuestionario)
        {
            IdQuestionario = idQuestionario;
            SequenciaQuestionario = sequenciaQuestionario;
        }
        
        [Required]
        public ulong IdQuestionario 
        { 
            get=> _idQuestionario;
            set => _idQuestionario = value;
        }
        
        [Required]
        public short SequenciaQuestionario 
        { 
            get=> _sequenciaQuestionario;
            set => _sequenciaQuestionario = value;
        }
        
    }
}