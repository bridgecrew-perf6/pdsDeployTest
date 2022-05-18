/*
*	<description>EpisodioQuestResposta, objeto da camada "BO"</description>
* 	<author>Rui Alves</author>
*   <date>18-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.BO
{
    [Serializable]
    public class EpisodioQuestResposta : ObjDBInfo, IComparable<EpisodioQuestResposta>
    {
        private EpisodioQuestionario _episodioquestionario;
        private short _sequenciaresposta;
        private Questionario _questionario;
        private Pergunta _pergunta;
        private string _resposta;
        private bool _ativo;

        public EpisodioQuestResposta(EpisodioQuestionario episodioquestionario, Questionario questionario, Pergunta pergunta, string resposta, bool ativo, Utilizador utilizadorAtivo = null)
        {
            EpisodioQuestionario = episodioquestionario;
            SequenciaResposta = 1;
            Questionario = questionario;
            Pergunta = pergunta;
            Resposta = resposta;
            Ativo = ativo;
            
            if (utilizadorAtivo != null) {
                CriadoPor = utilizadorAtivo;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizadorAtivo;
                ModificadoEm = DateTime.Now;
            }
        }

        public EpisodioQuestResposta(EpisodioQuestionario episodioquestionario, short sequenciaresposta, Questionario questionario, Pergunta pergunta,
            string resposta, bool ativo, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm,
            Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            EpisodioQuestionario = episodioquestionario;
            SequenciaResposta = sequenciaresposta;
            Questionario = questionario;
            Pergunta = pergunta;
            Resposta = resposta;
            Ativo = ativo;
        }
        
        #region Set/Getter

        public EpisodioQuestionario EpisodioQuestionario
        {
            get => _episodioquestionario;
            set => ModificarEpisodioQuestionario(value);
        }

        private void ModificarEpisodioQuestionario(EpisodioQuestionario episodioQuestionario)
        {
            if(ReferenceEquals(episodioQuestionario, null))
                throw new MyException("EpisodioQuestResposta.ModificarEpisodioQuestionario(null)->recebeu objeto vazio");
            if (_episodioquestionario != episodioQuestionario)
            {
                _episodioquestionario = episodioQuestionario;
                ObjetoModificadoEm();
            }
        }
        
        public short SequenciaResposta
        {
            get => _sequenciaresposta;
            set => ModificarSequenciaResposta(value);
        }

        private void ModificarSequenciaResposta(short sequenciaResposta)
        {
            if (_sequenciaresposta != sequenciaResposta)
            {
                _sequenciaresposta = sequenciaResposta;
                ObjetoModificadoEm();
            }
        }

        public Questionario Questionario
        {
            get => _questionario;
            set => ModificarQuestionario(value);
        }

        private void ModificarQuestionario(Questionario questionario)
        {
            if (ReferenceEquals(questionario, null))
                throw new MyException("EpisodioQuestResposta.ModificarQuestionario(null)->recebeu objeto vazio");
            if (_questionario != questionario)
            {
                _questionario = questionario;
                ObjetoModificadoEm();
            }
        }

        public Pergunta Pergunta
        {
            get => _pergunta;
            set => ModificarPergunta(value);
        }

        private void ModificarPergunta(Pergunta pergunta)
        {
            if (ReferenceEquals(pergunta, null))
                throw new MyException("EpisodioQuestResposta.ModificarPergunta(null)->recebeu objeto vazio");
            if (_pergunta != pergunta)
            {
                _pergunta = pergunta;
                ObjetoModificadoEm();
            }
        }

        public string Resposta
        {
            get => _resposta;
            set => ModificarResposta(value);
        }

        private void ModificarResposta(string resposta)
        {
            if (_resposta != resposta)
            {
                _resposta = resposta;
                ObjetoModificadoEm();
            }
        }


        public bool Ativo
        {
            get => _ativo;
            set => ModificarAtivo(value);
        }

        private void ModificarAtivo(bool ativo)
        {
            if (_ativo != ativo)
            {
                _ativo = ativo;
                ObjetoModificadoEm();
            }
        }

        #endregion

        public override string ToString()
        {
            return $"({EpisodioQuestionario.IdEpisodioQuestionario}, {SequenciaResposta}) {Questionario.IdQuestionario} {Pergunta.IdPergunta} {Resposta}";
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Comparables
        
        public int CompareTo(EpisodioQuestResposta other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            int r = EpisodioQuestionario.IdEpisodioQuestionario.CompareTo(other.EpisodioQuestionario
                .IdEpisodioQuestionario);
            if (r != 0)
                r = SequenciaResposta.CompareTo(other.SequenciaResposta);
            return r;
        }

        public bool Equals(EpisodioQuestResposta outro)
        {
            if (ReferenceEquals(null, outro)) return false;
            if (ReferenceEquals(this, outro)) return true;
            return String.Compare(ToString(), outro.ToString(), StringComparison.Ordinal) == 0;
        }
        
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is EpisodioQuestResposta) return Equals(obj as EpisodioQuestResposta);
            return false;
        }

        public static bool operator == (EpisodioQuestResposta obj1, EpisodioQuestResposta obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (EpisodioQuestResposta obj1, EpisodioQuestResposta obj2)
        {
            return !(obj1 == obj2);
        }
        
        #endregion
        
    }
}