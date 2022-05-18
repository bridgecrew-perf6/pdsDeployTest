namespace WebAppOSLER.Models
{
    public class EpisodioQuestRepostaRequest
    {
        private ulong _idEpisodioQuestionario;
        private short _sequenciaResposta;
        private ulong _idQuestionario;
        private ulong _idPergunta;
        private string _resposta;
        private bool _ativo;

        public EpisodioQuestRepostaRequest(ulong idEpisodioQuestionario, short sequenciaResposta, ulong idQuestionario,
            ulong idPergunta, string resposta, bool ativo)
        {
             IdEpisodioQuestionario = idEpisodioQuestionario;
             SequenciaResposta = sequenciaResposta;
             IdQuestionario = idQuestionario;
             IdPergunta = idPergunta;
             Resposta = resposta;
             Ativo = ativo;
        }
        
        public ulong IdEpisodioQuestionario{
            get => _idEpisodioQuestionario;
            set => _idEpisodioQuestionario = value;
            }
            
        public short SequenciaResposta{
            get => _sequenciaResposta;
            set => _sequenciaResposta = value;
            }
            
        public ulong IdQuestionario{
            get => _idQuestionario;
            set => _idQuestionario = value;
            }
            
        public ulong IdPergunta{
            get => _idPergunta;
            set => _idPergunta = value;
            }
            
        public string Resposta{
            get => _resposta;
            set => _resposta = value;
            }
            
        public bool Ativo{
            get => _ativo;
            set => _ativo = value;
            }
            
    }
}