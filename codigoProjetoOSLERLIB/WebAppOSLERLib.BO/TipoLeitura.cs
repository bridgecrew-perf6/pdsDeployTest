/*
*	<description>TipoLeitura, objeto da camada "BO"</description>
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
    public class TipoLeitura: ObjDBInfo, IComparable<TipoLeitura>
    {
        private ulong _idTipoLeitura;
        private string _descricao;
        private string _medida;

        public TipoLeitura(string descricao, string medida, Utilizador utilizador = null)
        {
            _idTipoLeitura = IdTools.IdGenerate();
            Descricao = descricao;
            Medida = medida;
            if (utilizador != null) {
                CriadoPor = utilizador;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizador;
                ModificadoEm = DateTime.Now;
            }
        }

        public TipoLeitura(ulong idTipoLeitura, string descricao, string medida, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idTipoLeitura = idTipoLeitura;
            Descricao = descricao;
            Medida = medida;
        }

        private void ChangeDescricao(string descricao)
        {
            if (_descricao != descricao)
            {
                _descricao = descricao;
                ObjetoModificadoEm();
            }
        }

        private void ChangeMedida(string medida)
        {
            if (_medida != medida)
            {
                _medida = medida;
                ObjetoModificadoEm();
            }
        }

        public ulong IdTipoLeitura => _idTipoLeitura;
        
        [Required]
        public string Descricao
        {
            get => _descricao;
            set => ChangeDescricao(value);
        }
        
        [Required]
        public string Medida
        {
            get => _medida;
            set => ChangeMedida(value);
        }

        public int CompareTo(TipoLeitura other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdTipoLeitura.CompareTo(other.IdTipoLeitura);
        }
        
        public override string ToString()
        {
            return $"({IdTipoLeitura}) {Descricao}";
        }

        public bool Equals(TipoLeitura other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is TipoLeitura) return Equals(obj as TipoLeitura);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (TipoLeitura obj1, TipoLeitura obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (TipoLeitura obj1, TipoLeitura obj2)
        {
            return !(obj1 == obj2);
        }
    }
}