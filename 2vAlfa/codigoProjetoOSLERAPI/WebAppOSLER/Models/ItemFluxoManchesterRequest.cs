/*
*	<description>ItemFluxoManchesterRequest.cs, para lidar com formulário para um ItemFluxoManchester</description>
* 	<author>João Carlos Pinto</author>
*   <date>13-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLER.Models
{
    [Serializable]
    public class ItemFluxoManchesterRequest
    {
        private string _descricao;
        private ulong _idClassificacao;

        public ItemFluxoManchesterRequest(string descricao, ulong idClassificacao)
        {
            _descricao = descricao;
            _idClassificacao = idClassificacao;
        }

        [Required]
        public string Descricao 
        { 
            get => _descricao;
            set => _descricao = value;
        }
        
        [Required]
        public ulong IdClassificacao 
        { 
            get => _idClassificacao;
            set => _idClassificacao = value;
        }

    }
}