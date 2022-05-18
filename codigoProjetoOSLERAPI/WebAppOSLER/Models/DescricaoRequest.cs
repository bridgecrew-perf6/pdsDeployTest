/*
*	<description>DescricaoRequest.cs, para lidar com formulário com uma descrição</description>
* 	<author>João Carlos Pinto</author>
*   <date>11-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER.Models
{
    [Serializable]
    public class DescricaoRequest
    {
        private string _descricao;
        
        public DescricaoRequest(string descricao)
        {
            _descricao = descricao;
        }
        
        [Required]
        public string Descricao { get=>_descricao;
            set => _descricao = value;
        }
        
    }
}