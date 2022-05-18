/*
*	<description>CorTriagem, objeto da camada "BO"</description>
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
    public class CorTriagem: ObjDBInfo, IComparable<CorTriagem>
    {
        private ulong _idCorTriagem;
        private string _descricao;
        private string _codigoCorHex;

        public CorTriagem(string descricao, string codigoCorHex)
        {
            _idCorTriagem = IdTools.IdGenerate();
            Descricao = descricao;
            CodigoCorHex = codigoCorHex;
        }
        public CorTriagem(ulong idCorTriagem, string descricao, string codigoCorHex, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idCorTriagem = idCorTriagem;
            Descricao = descricao;
            CodigoCorHex = codigoCorHex;
        }

        private void ChangeDescricao(string descricao)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("CorTriagem()-> descricao tem que estar preenchida!");
            if (_descricao != descricao)
            {
                _descricao = descricao;
                ObjetoModificadoEm();
            }
        }

        private void ChangeCodigoCorHex(string codigoCorHex)
        {
            if (_codigoCorHex != codigoCorHex)
            {
                _codigoCorHex = codigoCorHex;
                ObjetoModificadoEm();
            }
        }

        public ulong IdCorTriagem => _idCorTriagem;
        
        [Required]
        public string Descricao
        {
            get => _descricao;
            set => ChangeDescricao(value);
        }
        
        [Required]
        public string CodigoCorHex
        {
            get => _codigoCorHex;
            set => ChangeCodigoCorHex(value);
        }

        public int CompareTo(CorTriagem other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdCorTriagem.CompareTo(other.IdCorTriagem);
        }

        public override string ToString()
        {
            return $"({IdCorTriagem}) {Descricao}";
        }

        public bool Equals(CorTriagem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is CorTriagem) return Equals(obj as CorTriagem);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (CorTriagem obj1, CorTriagem obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (CorTriagem obj1, CorTriagem obj2)
        {
            return !(obj1 == obj2);
        }
    }
}