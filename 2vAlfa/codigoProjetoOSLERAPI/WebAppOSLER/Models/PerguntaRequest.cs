namespace WebAppOSLER.Models
{
    public class PerguntaRequest
    {
        private string _textoPergunta;

        public PerguntaRequest(string textoPergunta)
        {
            TextoPergunta = textoPergunta;
        }
        
        public string TextoPergunta
        {
            get => _textoPergunta;
            set => _textoPergunta = value;
        }

    }
}