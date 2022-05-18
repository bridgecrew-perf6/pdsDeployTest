/*
*	<description>Questionario, objeto da camada "BO"</description>
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
    public class Questionario: ObjDBInfo, IComparable<Questionario>
    {
        private ulong _idQuestionario;
        private string _descricao;
        private Idioma _idiomaQuestionario;

        public Questionario(string descricao, Idioma idiomaQuestionario, Utilizador utilizador = null)
        {
            IdQuestionario = IdTools.IdGenerate();
            Descricao = descricao;
            IdiomaQuestionario = idiomaQuestionario;
            if (utilizador != null) {
                CriadoPor = utilizador;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizador;
                ModificadoEm = DateTime.Now;
            }
        }

        public Questionario(ulong idQuestionario, string descricao, Idioma idiomaQuestionario, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            IdQuestionario = idQuestionario;
            Descricao = descricao;
            IdiomaQuestionario = idiomaQuestionario;
        }

        public ulong IdQuestionario
        {
            get => _idQuestionario;
            set => _idQuestionario = value;
        }

        private void ChangeDescricao(string descricao)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("Questionario.ChangeDescricao()-> descricao tem que estar preenchida!");
            if (_descricao != descricao)
            {
                _descricao = descricao;
                ObjetoModificadoEm();
            }
        }
        
        private void ChangeIdioma(Idioma idiomaQuestionario)
        {
            if (ReferenceEquals(idiomaQuestionario, null)) 
                throw new MyException("Questionario.ChangeIdioma(null)->recebeu objeto vazio!");
            if (_idiomaQuestionario != idiomaQuestionario)
            {
                _idiomaQuestionario = idiomaQuestionario;
                ObjetoModificadoEm();
            }
        }

        [Required]
        public string Descricao
        {
            get => _descricao;
            set => ChangeDescricao(value);
        }

        [Required]
        public Idioma IdiomaQuestionario
        {
            get => _idiomaQuestionario;
            set => ChangeIdioma(value);
        }

        public int CompareTo(Questionario other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdQuestionario.CompareTo(other.IdQuestionario);
        }
        
        public override string ToString()
        {
            return $"({IdQuestionario}) {Descricao}";
        }

        public bool Equals(Questionario other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Questionario) return Equals(obj as Questionario);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (Questionario obj1, Questionario obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (Questionario obj1, Questionario obj2)
        {
            return !(obj1 == obj2);
        }
    }
}