/*
*	<description>EpisodioHistLocal, objeto da camada "BO"</description>
* 	<author>João Carlos Pinto</author>
*   <date>16-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.ComponentModel.DataAnnotations;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.BO
{
    [Serializable]
    public class EpisodioQuestionario: ObjDBInfo, IComparable<EpisodioQuestionario>
    {
        private ulong _idEpisodioQuestionario;
        private Questionario _questionario;
        private short _sequenciaQuestionario;
        private Episodio _episodio;

        public EpisodioQuestionario(Questionario questionario, Episodio episodio, short sequenciaQuestionario = 1, Utilizador utilizadorAtivo = null)
        {
            _idEpisodioQuestionario = IdTools.IdGenerate();
            Questionario = questionario;
            Episodio = episodio;
            SequenciaQuestionario = sequenciaQuestionario;
            if (utilizadorAtivo != null) {
                CriadoPor = utilizadorAtivo;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizadorAtivo;
                ModificadoEm = DateTime.Now;
            }
        }

        public EpisodioQuestionario(ulong idEpisodioQuestionario, Questionario questionario, short sequenciaQuestionario, Episodio episodio, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idEpisodioQuestionario = idEpisodioQuestionario;
            Questionario = questionario;
            SequenciaQuestionario = sequenciaQuestionario;
            Episodio = episodio;
        }

        private void ChangeEpisodio(Episodio episodio)
        {
            if (ReferenceEquals(episodio, null)) 
                throw new MyException("EpisodioQuestionario.ChangeEpisodio(null)->recebeu objeto vazio!");
            if (_episodio != episodio)
            {
                _episodio = episodio;
                ObjetoModificadoEm();
            }
        }

        private void ChangeQuestionario(Questionario questionario)
        {
            if (ReferenceEquals(questionario, null)) 
                throw new MyException("EpisodioQuestionario.ChangeQuestionario(null)->recebeu objeto vazio!");
            if (_questionario != questionario)
            {
                _questionario = questionario;
                ObjetoModificadoEm();
            }
        }

        private void ChangeSequenciaQuestionario(short sequenciaQuestionario)
        {
            if (_sequenciaQuestionario != sequenciaQuestionario)
            {
                _sequenciaQuestionario = sequenciaQuestionario;
                ObjetoModificadoEm();
            }
        }

        public ulong IdEpisodioQuestionario => _idEpisodioQuestionario;
        
        [Required]
        public Questionario Questionario
        {
            get => _questionario;
            set => ChangeQuestionario(value);
        }

        [Required]
        public Episodio Episodio
        {
            get => _episodio;
            set => ChangeEpisodio(value);
        }
        
        public short SequenciaQuestionario
        {
            get => _sequenciaQuestionario;
            set => ChangeSequenciaQuestionario(value);
        }

        public int CompareTo(EpisodioQuestionario other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdEpisodioQuestionario.CompareTo(other.IdEpisodioQuestionario);
        }
        
        public override string ToString()
        {
            return $"({IdEpisodioQuestionario},{SequenciaQuestionario}) {Questionario.Descricao}";
        }

        public bool Equals(EpisodioQuestionario other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is EpisodioQuestionario) return Equals(obj as EpisodioQuestionario);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (EpisodioQuestionario obj1, EpisodioQuestionario obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (EpisodioQuestionario obj1, EpisodioQuestionario obj2)
        {
            return !(obj1 == obj2);
        }
    }
}