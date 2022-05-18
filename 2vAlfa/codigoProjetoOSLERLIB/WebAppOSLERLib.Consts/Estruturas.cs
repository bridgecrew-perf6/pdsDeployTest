/*
*	<description>Estruturas.cs, camada "Consts" tem as estruturas necessárias para produzir
*           listas em LIB e respostas em API</description>
* 	<author>João Carlos Pinto</author>
*   <date>11-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLERLib.Consts
{
    [Serializable]
    public struct RecAuthorization
    {
        private string _role;
        private bool _user;
        private string _username;
        private ulong _userId;
        private bool _episodio;
        private ulong _episodioId;
        private bool _valid;
        private bool _expired;
        private int _errorcod;
        private string _errormsg;

        public RecAuthorization(string role = null, bool user = default, string username = null, ulong userId = default, bool episodio = default, ulong episodioId = default, bool valid = default, bool expired = default, int errorcod = default, string errormsg = null)
        {
            _role = role;
            _user = user;
            _username = username;
            _userId = userId;
            _episodio = episodio;
            _episodioId = episodioId;
            _valid = valid;
            _expired = expired;
            _errorcod = errorcod;
            _errormsg = errormsg;
        }
        [Required]
        public string Role
        {
            get => _role;
            set => _role = value;
        }
        [Required]
        public bool User
        {
            get => _user;
            set => _user = value;
        }
        [Required]
        public string Username
        {
            get => _username;
            set => _username = value;
        }
        [Required]
        public ulong UserId
        {
            get => _userId;
            set => _userId = value;
        }
        [Required]
        public bool Episodio
        {
            get => _episodio;
            set => _episodio = value;
        }
        [Required]
        public ulong EpisodioId
        {
            get => _episodioId;
            set => _episodioId = value;
        }
        [Required]
        public bool Valid
        {
            get => _valid;
            set => _valid = value;
        }
        [Required]
        public int Errorcod
        {
            get => _errorcod;
            set => _errorcod = value;
        }
        [Required]
        public string Errormsg
        {
            get => _errormsg;
            set => _errormsg = value;
        }
        [Required]
        public bool Expired
        {
            get => _expired;
            set => _expired = value;
        }

        public bool Utente => UserId == Constantes.IDefaultUtenteId;
        [Required]
        public bool Acompanhante => UserId == Constantes.IDefaultAcompanhanteId;
        [Required]
        public bool Sysadmin => UserId == Constantes.IDefaultSysadminId;
    }
    
    [Serializable]
    public struct RecMensagem
    {
        private string _msg;
        private int _cod;

        public RecMensagem(string msg, int cod=0)
        {
            _msg = msg;
            _cod = cod;
        }
        [Required]
        public string Msg => _msg;
        [Required]
        public int Erro => _cod;
    }
    
    [Serializable]
    public struct RecTipoLeitura
    {
        private ulong _id;
        private string _descricao;
        private string _medida;

        public RecTipoLeitura(ulong id, string descricao, string medida)
        {
            _id = id;
            _descricao = descricao;
            _medida = medida;
        }
        [Required]
        public ulong Id
        {
            get => _id;
            set => _id = value;
        }
        [Required]
        public string Descricao
        {
            get => _descricao;
            set => _descricao = value;
        }
        [Required]
        public string Medida
        {
            get => _medida;
            set => _medida = value;
        }
    }
    
    [Serializable]
    public struct RecIdTexto
    {
        private ulong _id;
        private string _texto;

        public RecIdTexto(ulong id, string texto)
        {
            _id = id;
            _texto = texto;
        }
        [Required]
        public ulong Id
        {
            get => _id;
            set => _id = value;
        }
        [Required]
        public string Texto
        {
            get => _texto;
            set => _texto = value;
        }
    }
    
    [Serializable]
    public struct RecIdDescIdioma
    {
        private ulong _id;
        private ulong _idIdioma;
        private string _descricao;

        public RecIdDescIdioma(ulong id, ulong idIdioma, string descricao)
        {
            _id = id;
            _idIdioma = idIdioma;
            _descricao = descricao;
        }
        [Required]
        public ulong Id
        {
            get => _id;
            set => _id = value;
        }
        [Required]
        public ulong IdIdioma
        {
            get => _idIdioma;
            set => _idIdioma = value;
        }
        [Required]
        public string Descricao
        {
            get => _descricao;
            set => _descricao = value;
        }
    }
    
    [Serializable]
    public struct RecIdDescCor
    {
        private ulong _id;
        private string _texto;
        private string _cor;

        public RecIdDescCor(ulong id, string texto, string cor)
        {
            _id = id;
            _cor = cor;
            _texto = texto;
        }
        [Required]
        public ulong Id
        {
            get => _id;
            set => _id = value;
        }
        [Required]
        public string Texto
        {
            get => _texto;
            set => _texto = value;
        }
        [Required]
        public string Cor
        {
            get => _cor;
            set => _cor = value;
        }
    }

    [Serializable]
    public struct RecItemFluxoManchester
    {
        private ulong _id;
        private string _descricao;
        private ulong _idclassificacao;
        private string _classificacao;
        private string _cor;

        public RecItemFluxoManchester(ulong id, string descricao, ulong idClassificacao, string classificacao, string cor)
        {
            _id = id;
            _idclassificacao = idClassificacao;
            _classificacao = classificacao;
            _descricao = descricao;
            _cor = cor;
        }
        [Required]
        public ulong Id
        {
            get => _id;
            set => _id = value;
        }
        [Required]
        public string Descricao
        {
            get => _descricao;
            set => _descricao = value;
        }
        [Required]
        public ulong Idclassificacao
        {
            get => _idclassificacao;
            set => _idclassificacao = value;
        }
        [Required]
        public string Classificacao
        {
            get => _classificacao;
            set => _classificacao = value;
        }
        [Required]
        public string Cor
        {
            get => _cor;
            set => _cor = value;
        }
    }

    [Serializable]
    public struct RecUtilizador
    {
        private string _nome;
        private string _password;
        private bool _ativo;
        private int _nivelacesso;
        private ulong _idioma;

        public RecUtilizador(string nome, string password, bool ativo, int nivelacesso, ulong idioma)
        {
            _nome = nome;
            _password = password;
            _ativo = ativo;
            _nivelacesso = nivelacesso;
            _idioma = idioma;
        }
        [Required]
        public string Nome
        {
            get => _nome;
            set => _nome = value;
        }
        [Required]
        public string Password
        {
            get => _password;
            set => _password = value;
        }
        [Required]
        public bool Ativo
        {
            get => _ativo;
            set => _ativo = value;
        }
        [Required]
        public int Nivelacesso
        {
            get => _nivelacesso;
            set => _nivelacesso = value;
        }
        [Required]
        public ulong Idioma
        {
            get => _idioma;
            set => _idioma = value;
        }
    }

    [Serializable]
    public struct RecRegistoDados
    {
        private ulong _idepisodio;
        private ulong _idtipoleitura;
        private string _valor;
        private DateTime _datahora;

        public RecRegistoDados(ulong idEpisodio, ulong idTipoLeitura, string valor, DateTime dataHora)
        {
            _idepisodio = idEpisodio;
            _idtipoleitura = idTipoLeitura;
            _valor = valor;
            _datahora = dataHora;
        }
        [Required]
        public ulong Idepisodio
        {
            get => _idepisodio;
            set => _idepisodio = value;
        }
        [Required]
        public ulong Idtipoleitura
        {
            get => _idtipoleitura;
            set => _idtipoleitura = value;
        }
        [Required]
        public string Valor
        {
            get => _valor;
            set => _valor = value;
        }
        [Required]
        public DateTime Datahora
        {
            get => _datahora;
            set => _datahora = value;
        }
    }
    
    [Serializable]
    public struct RecEpisodioQuestionario
    {
        private ulong _IdEpisodioQuestionario;
        private ulong _IdEpisodio;
        private ulong _IdQuestionario;
        private short _SequenciaQuestionario;

        public RecEpisodioQuestionario(ulong idEpisodioQuestionario, ulong idEpisodio, ulong idQuestionario, short sequenciaQuestionario)
        {
            _IdEpisodioQuestionario = idEpisodioQuestionario;
            _IdEpisodio = idEpisodio;
            _IdQuestionario = idQuestionario;
            _SequenciaQuestionario = sequenciaQuestionario;
        }
        [Required]
        public ulong IdEpisodioQuestionario
        {
            get => _IdEpisodioQuestionario;
            set => _IdEpisodioQuestionario = value;
        }
        [Required]
        public ulong IdEpisodio
        {
            get => _IdEpisodio;
            set => _IdEpisodio = value;
        }
        [Required]
        public ulong IdQuestionario
        {
            get => _IdQuestionario;
            set => _IdQuestionario = value;
        }
        [Required]
        public short SequenciaQuestionario
        {
            get => _SequenciaQuestionario;
            set => _SequenciaQuestionario = value;
        }
    }

    [Serializable]
    public struct RecQuestResposta
    {
        private ulong idEpisodioQuestionario;
        private string pergunta;
        private string resposta;
        private ulong enviadoPor;
        public RecQuestResposta(ulong idEpisodioQuestionario, string pergunta, string resposta, ulong enviadoPor)
        {
            this.idEpisodioQuestionario = idEpisodioQuestionario;
            this.pergunta = pergunta;
            this.resposta = resposta;
            this.enviadoPor = enviadoPor;
        }
        
        public ulong IdEpisodioQuestionario => idEpisodioQuestionario;
        
        public string Pergunta => pergunta;
        
        public string Resposta => resposta;
        public ulong EnviadoPor => enviadoPor;
        
    }

    [Serializable]
    public struct RecEpisodio
    {
        private ulong _idEpisodio;
        private string _episodioTxt;
        private string _descricao;
        private DateTime _dataNascimento;
        private string _idSns;

        public RecEpisodio(ulong idEpisodio, string episodioTxt, string descricao, DateTime dataNascimento, string idSns)
        {
            _idEpisodio = idEpisodio;
            _episodioTxt = episodioTxt ?? throw new ArgumentNullException(nameof(episodioTxt));
            _descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
            _dataNascimento = dataNascimento;
            _idSns = idSns ?? throw new ArgumentNullException(nameof(idSns));
        }
        [Required]
        public ulong IdEpisodio
        {
            get => _idEpisodio;
            set => _idEpisodio = value;
        }
        [Required]
        public string EpisodioTxt
        {
            get => _episodioTxt;
            set => _episodioTxt = value;
        }
        [Required]
        public string Descricao
        {
            get => _descricao;
            set => _descricao = value;
        }
        [Required]
        public DateTime DataNascimento
        {
            get => _dataNascimento;
            set => _dataNascimento = value;
        }
        [Required]
        public string IdSns
        {
            get => _idSns;
            set => _idSns = value;
        }
    }
    
    [Serializable]
    public struct RecLocal
    {
        private ulong _IdLocal;
        private DateTime _DataHora;
        private ulong _IdEpisodio;
        public RecLocal(ulong idLocal, DateTime dataHora, ulong idEpisodio)
        {
            _IdLocal = idLocal;
            _DataHora = dataHora;
            _IdEpisodio = idEpisodio;
        }
        [Required]
        public ulong IdLocal
        {
            get => _IdLocal;
            set => _IdLocal = value;
        }
        [Required]
        public DateTime DataHora
        {
            get => _DataHora;
            set => _DataHora = value;
        }
        [Required]
        public ulong IdEpisodio
        {
            get => _IdEpisodio;
            set => _IdEpisodio = value;
        }
    }

}