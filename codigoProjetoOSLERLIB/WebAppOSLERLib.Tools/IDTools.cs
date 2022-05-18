/*
*	<description>IdTools, objeto da camada "Tools" responsável gerar um ID</description>
* 	<author>João Carlos Pinto</author>
*   <date>22-03-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.Threading;

namespace WebAppOSLERLib.Tools
{
    /// <summary>
    /// classe responsável por gerar um ID genérico
    /// </summary>
    public sealed class IdTools
    {
        private int _contador;
        private int inc_contador;
        private Random _localRandom;
        /// <summary>
        /// inicializar os contadores
        /// </summary>
        private IdTools()
        {
            _localRandom = new Random();
            _contador = _localRandom.Next(10, 99);
            inc_contador = _localRandom.Next(1, 7);
        }
        /// <summary>
        /// incrementa o contador, devolve o contador
        /// </summary>
        /// <returns>int</returns>
        private int NextContador()
        {
            _contador += inc_contador;
            if (_contador > 99) _contador = 10 + (_contador - 99);
            return _contador;
        }
        /// <summary>
        /// captura os milisegundos, retorna o digito da direita
        /// </summary>
        /// <returns>string</returns>
        private string Delay()
        {
            Thread.Sleep(_localRandom.Next(20,100));
            string dt = DateTime.Now.ToString("fff");
            return dt.Substring(dt.Length-1, 1);
        }
        private class Nested
        {
            static Nested() { }
            internal static readonly Lazy<IdTools> Instance = new Lazy<IdTools>(() => new IdTools());
        }
        /// <summary>
        /// gera um ID único
        /// </summary>
        /// <returns>ulong</returns>
        public static ulong IdGenerate()
        {
            DateTime dt = DateTime.Now;
            string sr = dt.ToString("yyyyMMddHHmmss") + 
                        Nested.Instance.Value.Delay() +
                        Nested.Instance.Value.NextContador().ToString() + 
                        Nested.Instance.Value._localRandom.Next(10, 99).ToString();
            return ulong.Parse(sr);
        }
    }
}
