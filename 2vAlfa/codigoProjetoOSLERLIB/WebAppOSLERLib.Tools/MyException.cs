/*
*	<description>MyException, objeto da camada "Tools" responsável pela personalização das exceções</description>
* 	<author>João Carlos Pinto</author>
*   <date>25-03-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;

namespace WebAppOSLERLib.Tools
{
    /// <summary>
    /// classe personalizada para gestão de erros
    /// </summary>
    public class MyException : ApplicationException
    {
        private MyException() : base("ERRO: n/d !!!") {}
        public MyException(string txt) : base(txt) { }
        public MyException(string txt, System.Exception e) : base(txt, e) { }
    }
}