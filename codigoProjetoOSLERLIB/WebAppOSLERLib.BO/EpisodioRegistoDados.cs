/*
*	<description>EpisodioRegistoDados, objeto da camada "BO"</description>
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
    public class EpisodioRegistoDados: ObjDBInfo, IComparable<EpisodioRegistoDados>
    {
        private ulong _idEpisodioRegistoDados;
        private TipoLeitura _tipoLeitura;
        private DateTime _dataHora;
        private Episodio _episodio;
        private string _valor;

        public EpisodioRegistoDados(TipoLeitura tipoLeitura, DateTime dataHora, string valor, Episodio episodio, Utilizador utilizadorAtivo = null)
        {
            _idEpisodioRegistoDados = IdTools.IdGenerate();
            TipoLeitura = tipoLeitura;
            Episodio = episodio;
            DataHora = dataHora;
            Valor = valor;
            if (utilizadorAtivo != null) {
                CriadoPor = utilizadorAtivo;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizadorAtivo;
                ModificadoEm = DateTime.Now;
            }
        }

        public EpisodioRegistoDados(ulong idEpisodioRegistoDados, TipoLeitura tipoLeitura, DateTime dataHora, string valor, Episodio episodio, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idEpisodioRegistoDados = idEpisodioRegistoDados;
            TipoLeitura = tipoLeitura;
            Episodio = episodio;
            DataHora = dataHora;
            Valor = valor;
        }

        private void ChangeEpisodio(Episodio episodio)
        {
            if (ReferenceEquals(episodio, null)) 
                throw new MyException("EpisodioRegistoDados.ChangeEpisodio(null)->recebeu objeto vazio!");
            if (_episodio != episodio)
            {
                _episodio = episodio;
                ObjetoModificadoEm();
            }
        }

        private void ChangeTipoLeitura(TipoLeitura tipoLeitura)
        {
            if (ReferenceEquals(tipoLeitura, null)) 
                throw new MyException("EpisodioRegistoDados.ChangeTipoLeitura(null)->recebeu objeto vazio!");
            if (_tipoLeitura != tipoLeitura)
            {
                _tipoLeitura = tipoLeitura;
                ObjetoModificadoEm();
            }
        }

        private void ChangeDataHora(DateTime dataHora)
        {
            if (_dataHora != dataHora)
            {
                _dataHora = dataHora;
                ObjetoModificadoEm();
            }
        }

        private void ChangeValor(string valor)
        {
            if (!(valor.Trim().Length > 0)) 
                throw new MyException("EpisodioRegistoDados.ChangeValor()-> valor tem que estar preenchido!");
            if (_valor != valor)
            {
                _valor = valor;
                ObjetoModificadoEm();
            }
        }

        public ulong IdEpisodioRegistoDados => _idEpisodioRegistoDados;
        
        [Required]
        public TipoLeitura TipoLeitura
        {
            get => _tipoLeitura;
            set => ChangeTipoLeitura(value);
        }

        [Required]
        public Episodio Episodio
        {
            get => _episodio;
            set => ChangeEpisodio(value);
        }
        
        [Required]
        public DateTime DataHora
        {
            get => _dataHora;
            set => ChangeDataHora(value);
        }

        [Required]
        public string Valor
        {
            get => _valor;
            set => ChangeValor(value);
        }

        public int CompareTo(EpisodioRegistoDados other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdEpisodioRegistoDados.CompareTo(other.IdEpisodioRegistoDados);
        }
        
        public override string ToString()
        {
            return $"({IdEpisodioRegistoDados},{DataHora}) {TipoLeitura.Descricao}";
        }

        public bool Equals(EpisodioRegistoDados other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is EpisodioRegistoDados) return Equals(obj as EpisodioRegistoDados);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (EpisodioRegistoDados obj1, EpisodioRegistoDados obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (EpisodioRegistoDados obj1, EpisodioRegistoDados obj2)
        {
            return !(obj1 == obj2);
        }
    }
}