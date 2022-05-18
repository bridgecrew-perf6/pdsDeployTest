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
    public class EpisodioHistLocal: ObjDBInfo, IComparable<EpisodioHistLocal>
    {
        private ulong _idHistoricoLocal;
        private Local _local;
        private DateTime _dataHora;
        private Episodio _episodio;

        public EpisodioHistLocal(Local local, DateTime dataHora, Episodio episodio, Utilizador utilizadorAtivo = null)
        {
            _idHistoricoLocal = IdTools.IdGenerate();
            Local = local;
            DataHora = dataHora;
            Episodio = episodio;
            if (utilizadorAtivo != null) {
                CriadoPor = utilizadorAtivo;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizadorAtivo;
                ModificadoEm = DateTime.Now;
            }
        }

        public EpisodioHistLocal(ulong idHistoricoLocal, Local local, DateTime dataHora, Episodio episodio, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idHistoricoLocal = idHistoricoLocal;
            Local = local;
            DataHora = dataHora;
            Episodio = episodio;
        }

        private void ChangeDataHora(DateTime dataHora)
        {
            if (_dataHora != dataHora)
            {
                _dataHora = dataHora;
                ObjetoModificadoEm();
            }
        }

        private void ChangeEpisodio(Episodio episodio)
        {
            if (ReferenceEquals(episodio, null)) 
                throw new MyException("EpisodioHistLocal.ChangeEpisodio(null)->recebeu objeto vazio!");
            if (this._episodio != episodio)
            {
                this._episodio = episodio;
                ObjetoModificadoEm();
            }
        }

        private void ChangeLocal(Local local)
        {
            if (ReferenceEquals(local, null)) 
                throw new MyException("EpisodioHistLocal.ChangeLocal(null)->recebeu objeto vazio!");
            if (_local != local)
            {
                _local = local;
                ObjetoModificadoEm();
            }
        }

        public ulong IdHistoricoLocal => _idHistoricoLocal;
        
        [Required]
        public DateTime DataHora
        {
            get => _dataHora;
            set => ChangeDataHora(value);
        }

        [Required]
        public Local Local
        {
            get => _local;
            set => ChangeLocal(value);
        }

        [Required]
        public Episodio Episodio
        {
            get => _episodio;
            set => ChangeEpisodio(value);
        }

        public int CompareTo(EpisodioHistLocal other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdHistoricoLocal.CompareTo(other.IdHistoricoLocal);
        }
        
        public override string ToString()
        {
            return $"({IdHistoricoLocal}) {Local.Descricao}";
        }

        public bool Equals(EpisodioHistLocal other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is EpisodioHistLocal) return Equals(obj as EpisodioHistLocal);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (EpisodioHistLocal obj1, EpisodioHistLocal obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (EpisodioHistLocal obj1, EpisodioHistLocal obj2)
        {
            return !(obj1 == obj2);
        }
    }
}