/*
*	<description>Nacionalidade, objeto da camada "BO"</description>
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
    public class Nacionalidade:ObjDBInfo, IComparable<Nacionalidade>
    {
        private ulong _idNacionalidade;
        private string _descricao;
        private Idioma _idioma;

        public Nacionalidade(string descricao, Idioma idioma, Utilizador utilizadorAtivo = null)
        {
            _idNacionalidade = IdTools.IdGenerate();
            Descricao = descricao;
            IdiomaNacionalidade = idioma;
            if (utilizadorAtivo != null) {
                CriadoPor = utilizadorAtivo;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizadorAtivo;
                ModificadoEm = DateTime.Now;
            }
        }

        public Nacionalidade(ulong idNacionalidade, string descricao, Idioma idioma, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idNacionalidade = idNacionalidade;
            Descricao = descricao;
            IdiomaNacionalidade = idioma;
        }

        private void ChangeDescricao(string descricao)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("Nacionalidade.ChangeDescricao()-> descricao tem que estar preenchida!");
            if (_descricao != descricao)
            {
                _descricao = descricao;
                ObjetoModificadoEm();
            }
        }

        private void ChangeIdioma(Idioma idioma)
        {
            if (ReferenceEquals(idioma, null)) 
                throw new MyException("Nacionalidade.ChangeIdioma(null)->recebeu objeto vazio!");
            if (this._idioma != idioma)
            {
                this._idioma = idioma;
                ObjetoModificadoEm();
            }
        }

        public ulong IdNacionalidade => _idNacionalidade;
        
        [Required]
        public string Descricao
        {
            get => _descricao;
            set => ChangeDescricao(value);
        }

        [Required]
        public Idioma IdiomaNacionalidade
        {
            get => _idioma;
            set => ChangeIdioma(value);
        }

        /// <summary>
        /// é necessário em locais onde é permitido apenas o acesso via instância (i.e. não permite acesso ao método static)
        /// </summary>
        public ulong DefaultId => DefaultNacionalidadeId();
        
        public static ulong DefaultNacionalidadeId()
        {
            return 1;
        }

        /// <summary>
        /// é necessário em locais onde é permitido apenas o acesso via instância (i.e. não permite acesso ao método static)
        /// </summary>
        public string DefaultDescricao => DefaultNacionalidadeDescricao();
        
        public static string DefaultNacionalidadeDescricao()
        {
            return "portuguesa";
        }

        public int CompareTo(Nacionalidade other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdNacionalidade.CompareTo(other.IdNacionalidade);
        }
        
        public override string ToString()
        {
            return $"({IdNacionalidade}) {Descricao}";
        }

        public bool Equals(Nacionalidade other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Nacionalidade) return Equals(obj as Nacionalidade);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (Nacionalidade obj1, Nacionalidade obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (Nacionalidade obj1, Nacionalidade obj2)
        {
            return !(obj1 == obj2);
        }
    }
}