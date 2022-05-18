/*
*	<description>Episodio.cs, objeto da camada "BO"</description>
* 	<author>João Carlos Pinto</author>
*   <date>23-03-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.ComponentModel.DataAnnotations;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.BO
{
    [Serializable]
    public class Episodio: ObjDBInfo, IComparable<Episodio>
    {
        private ulong _idEpisodio;
        private string _descricao;
        private string _idSns;
        private int _estado;
        private string _estadoTxt;
        private DateTime _dataNascimento;
        private short _pin4;
        private string _codEpisodio;
        private DateTime? _dataAberto;
        private DateTime? _dataFechado;
        private Idioma _idioma;
        private Nacionalidade _nacionalidade;
        private CorTriagem _corTriagem;

        public Episodio(string codEpisodio, string descricao, DateTime dataNascimento, string idSns, int estado, string estadoTxt, CorTriagem corTriagem, Idioma idioma, Nacionalidade nacionalidade, Utilizador utilizadorAtual)
        {
            _idEpisodio = IdTools.IdGenerate();
            CriadoEm=DateTime.Now;
            ModificadoEm=DateTime.Now;
            CriadoPor = utilizadorAtual;
            ModificadoPor = utilizadorAtual;
            CodEpisodio = codEpisodio;
            DataNascimento = dataNascimento;
            Descricao = descricao;
            IdSns = idSns;
            DataAberto = DateTime.Now;
            Estado = estado;
            EstadoTxt = estadoTxt;
            if (ReferenceEquals(corTriagem, null))  throw new MyException("Episodio.CorTriagem-> não pode ser null!");
            CorTriagem = corTriagem;
            if (ReferenceEquals(idioma, null))  throw new MyException("Episodio.Idioma-> não pode ser null!");
            Idioma = idioma;
            if (ReferenceEquals(nacionalidade, null))  throw new MyException("Episodio.Nacionalidade-> não pode ser null!");
            Nacionalidade = nacionalidade;
        }

        public Episodio(ulong idEpisodio, string codEpisodio, string descricao, DateTime dataNascimento, string idSns, int estado, string estadoTxt, DateTime? dataAberto, DateTime? dataFechado, CorTriagem corTriagem, Idioma idioma, Nacionalidade nacionalidade, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idEpisodio = idEpisodio;
            CodEpisodio = codEpisodio;
            Descricao = descricao;
            DataNascimento = dataNascimento;
            IdSns = idSns;
            DataAberto = dataAberto;
            DataFechado = dataFechado;
            if (ReferenceEquals(corTriagem, null))  throw new MyException("Episodio.CorTriagem-> não pode ser null!");
            CorTriagem = corTriagem;
            if (ReferenceEquals(idioma, null))  throw new MyException("Episodio.Idioma-> não pode ser null!");
            Idioma = idioma;
            if (ReferenceEquals(nacionalidade, null))  throw new MyException("Episodio.Nacionalidade-> não pode ser null!");
            Nacionalidade = nacionalidade;
            Estado = estado;
            EstadoTxt = estadoTxt;
        }

        private void ChangeIdSns(string idsns)
        {
            if (_idSns != idsns)
            {
                _idSns = idsns;
                ObjetoModificadoEm();
            }
        }
        
        private void ChangeDescricao(string descricao)
        {
            if (_descricao != descricao)
            {
                _descricao = descricao;
                ObjetoModificadoEm();
            }
        }
        
        private void ChangeEstado(int estado)
        {
            if (_estado != estado)
            {
                _estado = estado;
                ObjetoModificadoEm();
            }
        }
        
        private void ChangeIdioma(Idioma idioma)
        {
            if (_idioma != idioma)
            {
                _idioma = idioma;
                ObjetoModificadoEm();
            }
        }
        
        private void ChangeNacionalidade(Nacionalidade nacionalidade)
        {
            if (_nacionalidade != nacionalidade)
            {
                _nacionalidade = nacionalidade;
                ObjetoModificadoEm();
            }
        }
        
        private void ChangeCorTriagem(CorTriagem cortriagem)
        {
            if (_corTriagem != cortriagem)
            {
                _corTriagem = cortriagem;
                ObjetoModificadoEm();
            }
        }
        
        private void ChangeDataAberto(DateTime? dataAberto)
        {
            if (_dataAberto != dataAberto)
            {
                _dataAberto = dataAberto;
                ObjetoModificadoEm();
            }
        }
        
        private void ChangeDataFechado(DateTime? dataFechado)
        {
            if (_dataFechado != dataFechado)
            {
                _dataFechado = dataFechado;
                ObjetoModificadoEm();
            }
        }

        private void ChangeDtNascimento(DateTime dataNascimento)
        {
            if (_dataNascimento != dataNascimento)
            {
                _dataNascimento = dataNascimento;
                _pin4 = (short)_dataNascimento.Year;
                ObjetoModificadoEm();
            }
        }

        private void ChangeEstadoTxt(string estadoTxt)
        {
            if (_estadoTxt != estadoTxt)
            {
                _estadoTxt = estadoTxt;
                ObjetoModificadoEm();
            }
        }

        private void ChangeCodEpisodio(string codEpisodio)
        {
            if (_codEpisodio != codEpisodio)
            {
                _codEpisodio = codEpisodio;
                ObjetoModificadoEm();
            }
        }
        
        public ulong IdEpisodio => _idEpisodio;
        [Required]
        public string Descricao
        {
            get => _descricao;
            set => ChangeDescricao(value);
        }
        [Required]
        public string IdSns
        {
            get => _idSns;
            set => ChangeIdSns(value);
        }
        [Required]
        public int Estado
        {
            get => _estado;
            set => ChangeEstado(value);
        }
        public DateTime? DataAberto
        {
            get => _dataAberto;
            set => ChangeDataAberto(value);
        }
        public DateTime? DataFechado
        {
            get => _dataFechado;
            set => ChangeDataFechado(value);
        }
        public Idioma Idioma
        {
            get => _idioma;
            set => ChangeIdioma(value);
        }
        public Nacionalidade Nacionalidade
        {
            get => _nacionalidade;
            set => ChangeNacionalidade(value);
        }
        public CorTriagem CorTriagem
        {
            get => _corTriagem;
            set => ChangeCorTriagem(value);
        }
        public DateTime DataNascimento
        {
            get => _dataNascimento;
            set => ChangeDtNascimento(value);
        }
        public short Pin4 => _pin4;
        public string EstadoTxt
        {
            get => _estadoTxt;
            set => ChangeEstadoTxt(value);
        }
        public string CodEpisodio
        {
            get => _codEpisodio;
            set => ChangeCodEpisodio(value);
        }
        public ulong DefaultDummyId => DummyEpisodioId();
        public static ulong DummyEpisodioId()
        {
            return 0;
        }
        public bool OverrideIdEpisodioDummyId()
        {
            if (!IsPersistent)
            {
                _idEpisodio = DefaultDummyId;
                return true;
            }
            return false;
        }
        public int CompareTo(Episodio other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdEpisodio.CompareTo(other.IdEpisodio);
        }
        public override string ToString()
        {
            return $"({IdEpisodio}) {Descricao}";
        }
        public bool Equals(Episodio other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Episodio) return Equals(obj as Episodio);
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator == (Episodio obj1, Episodio obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }
        public static bool operator != (Episodio obj1, Episodio obj2)
        {
            return !(obj1 == obj2);
        }
    }
}