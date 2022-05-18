/*
*	<description>BaseDao, objeto principal da camada "DAL"</description>
* 	<author>João Carlos Pinto</author>
*   <date>12-05-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System.Data;
using Npgsql;
using WebAppOSLERLib.DB;

namespace WebAppOSLERLib.DAL
{
    public class BaseDao
    {
        private NpgsqlConnection _db;
        private bool _manterligacao;
        private bool _dbnewinstace;
        
        public BaseDao(NpgsqlConnection db=null)
        {
            _dbnewinstace = (db == null);
            if (_dbnewinstace)
                _db = DbConnectionObj.Instancia.Connection;
            else
                _db = db;
            ManterLigacao = false;
        }

        ~BaseDao()
        {
            if (!_dbnewinstace) return;
            if (Db.State == ConnectionState.Open) Db.Close();
            Db.Dispose();
        }
        
        public NpgsqlConnection Db => _db;
        
        /// <summary>
        /// flag para evitar a criação contínua de ligações para a base de dados
        /// </summary>
        public bool ManterLigacao
        {
            get => _manterligacao;
            set => _manterligacao = value;
        }

        public void DbOpen()
        {
            if (Db.State != ConnectionState.Open) 
                Db.Open();
        }

        public void DbClose(bool forceClose=false)
        {
            if (ManterLigacao && !forceClose) return;
            if (Db.State == ConnectionState.Open) 
                Db.Close();
        }
    }
}