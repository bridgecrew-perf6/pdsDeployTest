/*
*	<description>Constantes.cs, camada "Consts" tem as constantes para a LIB e API</description>
* 	<author>João Carlos Pinto</author>
*   <date>11-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

namespace WebAppOSLERLib.Consts
{
    /// <summary>
    /// espaço para definir as constantes do projeto
    /// </summary>
    public static class Constantes
    {
        /// <summary>
        /// total de níveis de acesso [0..5]
        /// </summary>
        public const int LibUtilizadorNivelAcesso = 6;
        
        /// <summary>
        /// níveis de acesso em texto
        /// </summary>
        public const string CNAUtente = "utente";
        public const string CNAAcompanhante = "acompanhante";
        public const string CNATriagem = "triagem";
        public const string CNAEnfermeiro = "enfermeiro";
        public const string CNAMedico = "medico";
        public const string CNASysadmin = "sysadmin";
        public const string CNATodos = CNAUtente+","+CNAAcompanhante+","+CNATriagem+","+CNAEnfermeiro+","+CNAMedico+","+CNASysadmin;
        public const string CNAUtentes = CNAUtente+","+CNAAcompanhante;

        /// <summary>
        /// níveis de acesso em formato de array
        /// </summary>
        public static readonly string[] DefaultNiveisAcessoText = CNATodos.Split(",");
        
        /// <summary>
        /// níveis de acesso em ID 
        /// </summary>
        public const int INAUtente = 0;
        public const int INAAcompanhante = 1;
        public const int INATriagem = 2;
        public const int INAEnfermeiro = 3;
        public const int INAMedico = 4;
        public const int INASysadmin = 5;

        /// <summary>
        /// 
        /// </summary>
        public const int IDefaultUtenteId = 0;
        public const int IDefaultAcompanhanteId = 1;
        public const int IDefaultSysadminId = 2;
        
        /// <summary>
        /// número de minutos para a validade do token
        /// </summary>
        public const int MinutosValidadeToken = 480;
        public const string CTKAuth = "Bearer "; // não retirar o espaço

        public const string MyCorsPolicy = "_myCorsPolicy";

    }
}