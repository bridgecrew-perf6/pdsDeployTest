/*
*	<description>DbConnectionObj, objeto da camada "DB" responsável pela ligação com a BD </description>
* 	<author>João Carlos Pinto</author>
*   <date>24-03-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.Data;
using Npgsql;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.DB
{
    /// <summary>
    /// classe responsável pela conexão com a BD
    /// </summary>
    public class DbConnectionObj
    {
        private NpgsqlConnection _db;
        /// <summary>
        /// construtor
        /// </summary>
        public DbConnectionObj(string ligacao="")
        {
            _db = null;
            if (ligacao.Length > 0)
                _db = Ligacao(ligacao);
        }
        ~DbConnectionObj()
        {
            if (Connection.State != ConnectionState.Closed)
                Connection.Close();
            Connection.Dispose();
        }
        /// <summary>
        /// inicializa e devolve a instância
        /// </summary>
        public static DbConnectionObj Instancia => new DbConnectionObj(DataConfig.Instancia.DbConnectString);
        /// <summary>
        /// fornece o acesso direto da BD
        /// </summary>
        public NpgsqlConnection Connection => _db;
        /// <summary>
        /// inicializa e devolve a ligação da BD
        /// </summary>
        /// <param name="ligacao"></param>
        /// <returns>NpgsqlConnection</returns>
        public NpgsqlConnection Ligacao(string ligacao)
        {
            try
            {
                if (_db != null) _db.Close();
                _db = new NpgsqlConnection(ligacao);
                return _db;
            }
            catch (Exception e)
            {
                throw new MyException($"DbConnectionObj.Ligacao(\"{ligacao}\")", e);
            }
        }
    }
}
