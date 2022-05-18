/*
*	<description>Pergunta, objeto da camada "BO"</description>
* 	<author>João Carlos Pinto</author>
*   <date>16-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.ComponentModel.DataAnnotations;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.BO
{
    [Serializable]
    public class Pergunta:ObjDBInfo, IComparable<Pergunta>
    {
        private ulong _idPergunta;
        private string _textoPergunta;
        private Questionario _questionario;
        private int _sequenciaPergunta;

        public Pergunta(string textoPergunta, Questionario questionario, Utilizador utilizadorAtivo = null)
        {
            _idPergunta = IdTools.IdGenerate();
            TextoPergunta = textoPergunta;
            Questionario = questionario;
            SequenciaPergunta = 1;
            if (utilizadorAtivo != null) {
                CriadoPor = utilizadorAtivo;
                CriadoEm = DateTime.Now;
                ModificadoPor = utilizadorAtivo;
                ModificadoEm = DateTime.Now;
            }
        }

        public Pergunta(ulong idPergunta, string textoPergunta, Questionario questionario, int sequenciaPergunta, DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor) : base(criadoEm, criadoPor, modificadoEm, modificadoPor)
        {
            _idPergunta = idPergunta;
            TextoPergunta = textoPergunta;
            Questionario = questionario;
            SequenciaPergunta = sequenciaPergunta;
        }

        private void ChangeTextoPergunta(string textoPergunta)
        {
            if (!(textoPergunta.Trim().Length > 0)) 
                throw new MyException("Pergunta.ChangeTextoPergunta()-> textoPergunta tem que estar preenchida!");
            if (_textoPergunta != textoPergunta)
            {
                _textoPergunta = textoPergunta;
                ObjetoModificadoEm();
            }
        }

        private void ChangeQuestionario(Questionario questionario)
        {
            if (ReferenceEquals(questionario, null)) 
                throw new MyException("Pergunta.ChangeQuestionario(null)->recebeu objeto vazio!");
            if (_questionario != questionario)
            {
                _questionario = questionario;
                ObjetoModificadoEm();
            }
        }

        private void ChangeSequenciaPergunta(int sequenciaPergunta)
        {
            if (_sequenciaPergunta != sequenciaPergunta)
            {
                _sequenciaPergunta = sequenciaPergunta;
                ObjetoModificadoEm();
            }
        }

        public ulong IdPergunta => _idPergunta;
        
        [Required]
        public string TextoPergunta
        {
            get => _textoPergunta;
            set => ChangeTextoPergunta(value);
        }

        [Required]
        public Questionario Questionario
        {
            get => _questionario;
            set => ChangeQuestionario(value);
        }
        
        [Required]
        public int SequenciaPergunta
        {
            get => _sequenciaPergunta;
            set => ChangeSequenciaPergunta(value);
        }
        
        public int CompareTo(Pergunta other)
        {
            if (ReferenceEquals(null, other)) return -1;
            if (ReferenceEquals(this, other)) return 0;
            return IdPergunta.CompareTo(other.IdPergunta);
        }
        
        public override string ToString()
        {
            return $"({IdPergunta}) {TextoPergunta}";
        }

        public bool Equals(Pergunta other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal)==0;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Pergunta) return Equals(obj as Pergunta);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (Pergunta obj1, Pergunta obj2)
        {
            if (ReferenceEquals(null, obj1)) return ReferenceEquals(null, obj2);
            return obj1.Equals(obj2);
        }

        public static bool operator != (Pergunta obj1, Pergunta obj2)
        {
            return !(obj1 == obj2);
        }
    }
}