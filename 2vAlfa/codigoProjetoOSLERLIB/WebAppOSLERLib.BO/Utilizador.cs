/*
*	<description>Utilizador, objeto da camada "BO"</description>
* 	<author>João Carlos Pinto</author>
*   <date>23-03-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.BO
{
    public class Utilizador:ObjDBInfo, IComparable<Utilizador>
    {
        private ulong _idUtilizador;
        private string _nome;
        private string _password;
        private int _nivelAcesso;
        private bool _ativo;
        private Idioma _idioma;

        public Utilizador(string nome, string password, int nivelAcesso, Idioma idioma, Utilizador utilizador=null)
        {
            _idUtilizador = IdTools.IdGenerate();
            Nome = nome;
            _password = password;
            _nivelAcesso = nivelAcesso;
            _ativo = true;
            Idioma = idioma;
            if (utilizador != null) {
                CriadoPor = utilizador;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizador;
                ModificadoEm = DateTime.Now;
            }
        }

        public Utilizador(ulong idUtilizador, string nome, string password, Idioma idioma, int nivelAcesso, bool ativo, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idUtilizador = idUtilizador;
            Nome = nome;
            _password = password;
            _nivelAcesso = nivelAcesso;
            _ativo = ativo;
            Idioma = idioma;
        }

        private void ChangeNome(string nome)
        {
            if (!(nome.Trim().Length > 0)) 
                throw new MyException("Utilizador.ChangeNome()-> nome tem que estar preenchido!");
            if (_nome != nome)
            {
                _nome = nome;
                ObjetoModificadoEm();
            }
        }

        private void ChangePassword(string password)
        {
            if (_password != password)
            {
                _password = password;
                ObjetoModificadoEm();
            }
        }
        
        private void ChangeNivelAcesso(int nivelAcesso)
        {
            if (_nivelAcesso != nivelAcesso)
            {
                _nivelAcesso = nivelAcesso;
                ObjetoModificadoEm();
            }
        }

        private void ChangeIdioma(Idioma idioma)
        {
            if (!IsLoading && ReferenceEquals(idioma, null)) 
                throw new MyException("Utilizador.ChangeIdioma(null)->recebeu objeto vazio!");
            if (_idioma != idioma)
            {
                _idioma = idioma;
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
        
        public ulong IdUtilizador => _idUtilizador;
        
        public bool Ativo
        {
            get => _ativo;
            set => ChangeAtivo(value);
        }

        public string Nome { 
            get => _nome;
            set => ChangeNome(value);
        }
        
        public string Password
        {
            get => _password;
            set => ChangePassword(value);
        }
        
        public int NivelAcesso
        {
            get => _nivelAcesso;
            set => ChangeNivelAcesso(value);
        }
        
        public Idioma Idioma
        {
            get => _idioma;
            set => ChangeIdioma(value);
        }

        public int CompareTo(Utilizador other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdUtilizador.CompareTo(other.IdUtilizador);
        }
       
        public override string ToString()
        {
            return $"({IdUtilizador}) {Nome}";
        }

        public bool Equals(Utilizador other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Utilizador) return Equals(obj as Utilizador);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (Utilizador obj1, Utilizador obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (Utilizador obj1, Utilizador obj2)
        {
            return !(obj1 == obj2);
        }
    }
}