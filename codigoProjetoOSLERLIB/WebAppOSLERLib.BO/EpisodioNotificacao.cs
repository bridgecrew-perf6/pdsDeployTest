/*
*	<description>EpisodioNotificacao, objeto da camada "BO"</description>
* 	<author>João Carlos Pinto</author>
*   <date>10-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.ComponentModel.DataAnnotations;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.BO
{
    [Serializable]
    public class EpisodioNotificacao: ObjDBInfo, IComparable<EpisodioNotificacao>
    {
        private ulong _idNotificacao;
        private DateTime _dataHora;
        private string _mensagem;
        private DateTime? _dataHoraLeitura;
        private Episodio _episodio;

        public EpisodioNotificacao(string mensagem, DateTime dataHora, Episodio episodio, Utilizador utilizadorAtivo = null)
        {
            _idNotificacao = IdTools.IdGenerate();
            Mensagem = mensagem;
            DataHora = dataHora;
            Episodio = episodio;
            DataHoraLeitura = null;
            if (utilizadorAtivo != null) {
                CriadoPor = utilizadorAtivo;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizadorAtivo;
                ModificadoEm = DateTime.Now;
            }
        }

        public EpisodioNotificacao(ulong idNotificacao, string mensagem, DateTime dataHora, DateTime? dataHoraLeitura, Episodio episodio, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idNotificacao = idNotificacao;
            Mensagem = mensagem;
            DataHora = dataHora;
            DataHoraLeitura = dataHoraLeitura;
            Episodio = episodio;
        }

        private void ChangeMensagem(string mensagem)
        {
            if (!(mensagem.Trim().Length > 0)) 
                throw new MyException("EpisodioNotificacao.ChangeMensagem()-> mensagem tem que estar preenchida!");
            if (_mensagem != mensagem)
            {
                _mensagem = mensagem;
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

        private void ChangeDataHoraLeitura(DateTime? dataHoraLeitura)
        {
            if (_dataHoraLeitura != dataHoraLeitura)
            {
                _dataHoraLeitura = dataHoraLeitura;
                ObjetoModificadoEm();
            }
        }

        private void ChangeEpisodio(Episodio episodio)
        {
            if (ReferenceEquals(episodio, null)) 
                throw new MyException("EpisodioNotificacao.ChangeEpisodio(null)->recebeu objeto vazio!");
            if (_episodio != episodio)
            {
                _episodio = episodio;
                ObjetoModificadoEm();
            }
        }

        public ulong IdNotificacao => _idNotificacao;
        
        [Required]
        public string Mensagem
        {
            get => _mensagem;
            set => ChangeMensagem(value);
        }
        
        [Required]
        public DateTime DataHora
        {
            get => _dataHora;
            set => ChangeDataHora(value);
        }

        public DateTime? DataHoraLeitura
        {
            get => _dataHoraLeitura;
            set => ChangeDataHoraLeitura(value);
        }
        
        [Required]
        public Episodio Episodio
        {
            get => _episodio;
            set => ChangeEpisodio(value);
        }

        public int CompareTo(EpisodioNotificacao other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdNotificacao.CompareTo(other.IdNotificacao);
        }
        
        public override string ToString()
        {
            return $"({IdNotificacao}) {Mensagem}";
        }

        public bool Equals(EpisodioNotificacao other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is EpisodioNotificacao) return Equals(obj as EpisodioNotificacao);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (EpisodioNotificacao obj1, EpisodioNotificacao obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (EpisodioNotificacao obj1, EpisodioNotificacao obj2)
        {
            return !(obj1 == obj2);
        }
    }
}