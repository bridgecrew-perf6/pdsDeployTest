/*
*	<description>NacionalidadeRequest.cs, para lidar com formulário com uma descrição+idIdioma</description>
* 	<author>João Carlos Pinto</author>
*   <date>11-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER.Models
{
    [Serializable]
    public class NacionalidadeRequest
    {
        private string _descricao;
        private ulong _ididioma;
        
        public NacionalidadeRequest(string descricao, ulong ididioma)
        {
            _descricao = descricao;
            _ididioma = ididioma;
        }
        
        [Required]
        public string Descricao 
        { 
            get=>_descricao;
            set => _descricao = value;
        }
        
        [Required]
        public ulong IdIdioma 
        { 
            get=>_ididioma;
            set => _ididioma = value;
        }
        
    }
}