/*
*	<description>Idioma, objeto da camada "BO"</description>
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
    public class Idioma: ObjDBInfo, IComparable<Idioma>
    {
        private ulong _idIdioma;
        private string _descricao;
        
        public Idioma(string descricao, Utilizador utilizadorAtivo = null)
        {
            _idIdioma = IdTools.IdGenerate();
            Descricao = descricao;
            if (utilizadorAtivo != null) {
                CriadoPor = utilizadorAtivo;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizadorAtivo;
                ModificadoEm = DateTime.Now;
            }
        }

        public Idioma(ulong idIdioma, string descricao, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idIdioma = idIdioma;
            Descricao = descricao;
        }

        private void ChangeDescricao(string descricao)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("Idioma.ChangeDescricao()-> descricao tem que estar preenchida!");
            if (_descricao != descricao)
            {
                _descricao = descricao;
                ObjetoModificadoEm();
            }
        }

        public ulong IdIdioma => _idIdioma;
        
        [Required]
        public string Descricao
        {
            get => _descricao;
            set => ChangeDescricao(value);
        }

        /// <summary>
        /// é necessário em locais onde é permitido apenas o acesso via instância (i.e. não permite acesso ao método static)
        /// </summary>
        public ulong DefaultId => DefaultIdiomaId();
        
        public static ulong DefaultIdiomaId()
        {
            return 1;
        }

        /// <summary>
        /// é necessário em locais onde é permitido apenas o acesso via instância (i.e. não permite acesso ao método static)
        /// </summary>
        public string DefaultDescricao => DefaultIdiomaDescricao();
        
        public static string DefaultIdiomaDescricao()
        {
            return "Português";
        }

        public int CompareTo(Idioma other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdIdioma.CompareTo(other.IdIdioma);
        }
        
        public override string ToString()
        {
            return $"({IdIdioma}) {Descricao}";
        }

        public bool Equals(Idioma other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Idioma) return Equals(obj as Idioma);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (Idioma obj1, Idioma obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (Idioma obj1, Idioma obj2)
        {
            return !(obj1 == obj2);
        }
    }
}
