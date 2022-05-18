/*
*	<description>LoginRegisto, objeto da camada "BO" para o registo de login</description>
* 	<author>João Carlos Pinto</author>
*   <date>09-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.BO
{
    public class LoginRegisto: ObjDBInfo, IComparable<LoginRegisto>
    {
        private ulong _idLogin;
        private Utilizador _utilizador;
        private Episodio _episodio;
        private DateTime _dataHora;
        private string _tokenPayload;
        private bool _ativo;
        private DateTime _validadeToken;
        private string _infoDispositivo;

        public LoginRegisto(Utilizador utilizador, Episodio episodio, DateTime dataHora, string tokenPayload, bool ativo, DateTime validadeToken, string infoDispositivo, Utilizador utilizadorAtivo=null)
        {
            _idLogin = IdTools.IdGenerate();
            Utilizador = utilizador;
            Episodio = episodio;
            DataHora = dataHora;
            TokenPayload = tokenPayload;
            Ativo = ativo;
            ValidadeToken = validadeToken;
            InfoDispositivo = infoDispositivo;
            if (utilizadorAtivo != null) {
                CriadoPor = utilizadorAtivo;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizadorAtivo;
                ModificadoEm = DateTime.Now;
            }
        }

        public LoginRegisto(ulong idLogin, Utilizador utilizador, Episodio episodio, DateTime dataHora, string tokenPayload, bool ativo, DateTime validadeToken, string infoDispositivo, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idLogin = idLogin;
            Utilizador = utilizador;
            Episodio = episodio;
            DataHora = dataHora;
            TokenPayload = tokenPayload;
            Ativo = ativo;
            ValidadeToken = validadeToken;
            InfoDispositivo = infoDispositivo;
        }

        private void ChangeUtilizador(Utilizador utilizador)
        {
            if (ReferenceEquals(utilizador, null)) 
                throw new MyException("LoginRegisto.ChangeUtilizador(null)->recebeu objeto vazio!");
            if (_utilizador != utilizador)
            {
                _utilizador = utilizador;
                ObjetoModificadoEm();
            }
        }
        
        private void ChangeEpisodio(Episodio episodio)
        {
            if (ReferenceEquals(episodio, null)) 
                throw new MyException("LoginRegisto.ChangeEpisodio(null)->recebeu objeto vazio!");
            if (_episodio != episodio)
            {
                _episodio = episodio;
                ObjetoModificadoEm();
            }
        }
        
        private void ChangeTokenPayload(string tokenPayload)
        {
            if (!(tokenPayload.Trim().Length > 0)) 
                throw new MyException("LoginRegisto.ChangeTokenPayLoad()-> tokenpayload tem que estar preenchido!");
            if (_tokenPayload != tokenPayload)
            {
                _tokenPayload = tokenPayload;
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
        
        private void ChangeAtivo(bool ativo)
        {
            if (_ativo != ativo)
            {
                _ativo = ativo;
                ObjetoModificadoEm();
            }
        }
        
        private void ChangeValidadeToken(DateTime validadeToken)
        {
            if (_validadeToken != validadeToken)
            {
                _validadeToken = validadeToken;
                ObjetoModificadoEm();
            }
        }
        
        private void ChangeInfoDispositivo(string infoDispositivo)
        {
            if (_infoDispositivo != infoDispositivo)
            {
                _infoDispositivo = infoDispositivo;
                ObjetoModificadoEm();
            }
        }
        
        public ulong IdLogin => _idLogin;
        
        public Utilizador Utilizador
        {
            get => _utilizador;
            set => ChangeUtilizador(value);
        }

        public Episodio Episodio
        {
            get => _episodio;
            set => ChangeEpisodio(value);
        }

        public string TokenPayload
        {
            get => _tokenPayload;
            set => ChangeTokenPayload(value);
        }

        public DateTime DataHora
        {
            get => _dataHora;
            set => ChangeDataHora(value);
        }
        
        public bool Ativo
        {
            get => _ativo;
            set => ChangeAtivo(value);
        }
        
        public DateTime ValidadeToken
        {
            get => _validadeToken;
            set => ChangeValidadeToken(value);
        }
        
        public string InfoDispositivo
        {
            get => _infoDispositivo;
            set => ChangeInfoDispositivo(value);
        }
        
        public int CompareTo(LoginRegisto other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdLogin.CompareTo(other.IdLogin);
        }

        public override string ToString()
        {
            return $"({IdLogin}) {TokenPayload}";
        }

        public bool Equals(LoginRegisto other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is LoginRegisto) return Equals(obj as LoginRegisto);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (LoginRegisto obj1, LoginRegisto obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (LoginRegisto obj1, LoginRegisto obj2)
        {
            return !(obj1 == obj2);
        }

    }
}