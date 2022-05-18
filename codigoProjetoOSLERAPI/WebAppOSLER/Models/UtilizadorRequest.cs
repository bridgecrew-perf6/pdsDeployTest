/*
*	<description>UtilizadorRequest.cs - para lidar com formulário com um utilizador</description>
* 	<author>João Carlos Pinto</author>
*   <date>12-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER.Models
{
    [Serializable]
    public class UtilizadorRequest
    {
        private string _nome;
        private string _password;
        private bool _ativo;
        private int _nivelacesso;
        private ulong _idioma;
        
        public UtilizadorRequest(string nome, string password, bool ativo, int nivelacesso, ulong idioma)
        {
            _nome = nome;
            _password = password;
            _ativo = ativo;
            _nivelacesso = nivelacesso;
            _idioma = idioma;
        }
        
        [Required]
        public string Nome 
        { 
            get => _nome;
            set => _nome = value;
        }
        
        [Required]
        public string Password 
        { 
            get => _password;
            set => _password = value;
        }

        [Required]
        public bool Ativo
        {
            get => _ativo;
            set => _ativo = value;
        }
        
        [Required]
        public int NivelAcesso
        {
            get => _nivelacesso;
            set => _nivelacesso = value;
        }
        
        [Required]
        public ulong Idioma
        {
            get => _idioma;
            set => _idioma = value;
        }
        
    }
}