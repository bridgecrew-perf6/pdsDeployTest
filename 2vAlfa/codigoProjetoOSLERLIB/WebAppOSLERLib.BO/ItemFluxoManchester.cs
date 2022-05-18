/*
*	<description>ItemFluxoManchester.cs, objeto da camada "BO"</description>
* 	<author>João Carlos Pinto</author>
*   <date>13-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.ComponentModel.DataAnnotations;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.BO
{
    [Serializable]
    public class ItemFluxoManchester: ObjDBInfo, IComparable<ItemFluxoManchester>
    {
        private ulong _idItemFluxoManchester;
        private string _descricao;
        private CorTriagem _classificacao;

        public ItemFluxoManchester(string descricao, CorTriagem classificacao, Utilizador utilizador = null)
        {
            _idItemFluxoManchester = IdTools.IdGenerate();
            Descricao = descricao;
            if (ReferenceEquals(classificacao, null)) 
                throw new MyException($"ItemFluxoManchester(\"{descricao}\"): a classificacao deve estar inicializada!");
            Classificacao = classificacao;
            if (utilizador != null) {
                CriadoPor = utilizador;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizador;
                ModificadoEm = DateTime.Now;
            }
        }

        public ItemFluxoManchester(ulong idItemFluxoManchester, string descricao, CorTriagem classificacao, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idItemFluxoManchester = idItemFluxoManchester;
            Descricao = descricao;
            if (ReferenceEquals(classificacao, null)) 
                throw new MyException($"ItemFluxoManchester(\"{descricao}\"): a classificacao deve estar inicializada!");
            Classificacao = classificacao;
        }

        private void ChangeDescricao(string descricao)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("ItemFluxoManchester.ChangeDescricao()-> descricao tem que estar preenchida!");
            if (_descricao != descricao)
            {
                _descricao = descricao;
                ObjetoModificadoEm();
            }
        }

        private void ChangeClassificacao(CorTriagem classificacao)
        {
            if (ReferenceEquals(classificacao, null)) 
                throw new MyException("ItemFluxoManchester.ChangeClassificacao(null)->recebeu objeto vazio!");
            if (_classificacao != classificacao)
            {
                _classificacao = classificacao;
                ObjetoModificadoEm();
            }
        }

        public ulong IdItemFluxoManchester => _idItemFluxoManchester;
        
        [Required]
        public string Descricao
        {
            get => _descricao;
            set => ChangeDescricao(value);
        }
        
        [Required]
        public CorTriagem Classificacao
        {
            get => _classificacao;
            set => ChangeClassificacao(value);
        }
    
        public int CompareTo(ItemFluxoManchester other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdItemFluxoManchester.CompareTo(other.IdItemFluxoManchester);
        }

        public override string ToString()
        {
            return $"({IdItemFluxoManchester}) {Descricao}";
        }

        public bool Equals(ItemFluxoManchester other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is ItemFluxoManchester) return Equals(obj as ItemFluxoManchester);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (ItemFluxoManchester obj1, ItemFluxoManchester obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (ItemFluxoManchester obj1, ItemFluxoManchester obj2)
        {
            return !(obj1 == obj2);
        }
        
    }
}