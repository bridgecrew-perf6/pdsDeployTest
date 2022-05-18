/*
*	<description>Local, objeto da camada "BO"</description>
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
    public class Local: ObjDBInfo, IComparable<Local>
    {
        private ulong _idLocal;
        private string _descricao;

        public Local(string descricao, Utilizador utilizadorAtivo = null)
        {
            _idLocal = IdTools.IdGenerate();
            Descricao = descricao;
            if (utilizadorAtivo != null) {
                CriadoPor = utilizadorAtivo;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizadorAtivo;
                ModificadoEm = DateTime.Now;
            }
        }

        public Local(ulong idLocal, string descricao, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idLocal = idLocal;
            Descricao = descricao;
        }

        private void ChangeDescricao(string descricao)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("Local.ChangeDescricao()-> descricao tem que estar preenchida!");
            if (_descricao != descricao)
            {
                _descricao = descricao;
                ObjetoModificadoEm();
            }
        }

        public ulong IdLocal => _idLocal;
        
        [Required]
        public string Descricao
        {
            get => _descricao;
            set => ChangeDescricao(value);
        }

        /// <summary>
        /// é necessário em locais onde é permitido apenas o acesso via instância (i.e. não permite acesso ao método static)
        /// </summary>
        public ulong DefaultId => DefaultLocalId();
        public static ulong DefaultLocalId()
        {
            return 1;
        }

        /// <summary>
        /// é necessário em locais onde é permitido apenas o acesso via instância (i.e. não permite acesso ao método static)
        /// </summary>
        public string DefaultDescricao => DefaultLocalDescricao();
        public static string DefaultLocalDescricao()
        {
            return "Sala de espera";
        }

        public int CompareTo(Local other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdLocal.CompareTo(other.IdLocal);
        }

        public override string ToString()
        {
            return $"({IdLocal}) {Descricao}";
        }

        public bool Equals(Local other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Local) return Equals(obj as Local);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (Local obj1, Local obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (Local obj1, Local obj2)
        {
            return !(obj1 == obj2);
        }
    }
}