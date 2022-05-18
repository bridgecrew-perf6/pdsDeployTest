/*
*	<description>TipoLeituraRequest.cs, para lidar com formulário com uma descrição+medida</description>
* 	<author>João Carlos Pinto</author>
*   <date>11-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER.Models
{
    [Serializable]
    public class TipoLeituraRequest
    {
        private string _descricao;
        private string _medida;
        
        public TipoLeituraRequest(string descricao, string medida)
        {
            Descricao = descricao;
            Medida = medida;
        }
        
        [Required]
        public string Descricao 
        { 
            get => _descricao;
            set => _descricao = value;
        }
        
        [Required]
        public string Medida 
        { 
            get => _medida;
            set => _medida = value;
        }
    }
}