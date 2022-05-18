/*
*	<description>LocalDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
* 	<author>João Carlos Pinto</author>
*   <date>09-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using WebAppOSLERLib.BO;
using WebAppOSLERLib.Consts;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.DAL
{
    public class LocalDao: BaseDao
    {
        private Local _currentobj;
        public LocalDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }

        public Local CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        /// <summary>
        /// verifica se o objeto do tipo "Local" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="local"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(Local local, Utilizador utilizadorAtivo=null)
        {
            if (!local.IsPersistent || local.IsModified()) SaveObj(local, utilizadorAtivo);
        }
        /// <summary>
        /// Ler um local da BD
        /// </summary>
        /// <param name="idLocal"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public Local GetById(ulong idLocal)
        {
            Local resultado = null;
            string sql1 = "select idlocal, descricao, criadopor, criadoem, modificadopor, modificadoem from osler.local where idlocal = @IdLocal ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdLocal", NpgsqlDbType.Bigint, sizeof(Int64), Convert.ToInt64(idLocal));
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read()) {
                    int coluna = res1.GetOrdinal("criadoem");
                    DateTime? criadoEm = res1.IsDBNull(coluna) ? null : res1.GetDateTime(coluna);
                    coluna = res1.GetOrdinal("modificadoem");
                    DateTime? modificadoEm = res1.IsDBNull(coluna) ? null : res1.GetDateTime(coluna);
                    coluna = res1.GetOrdinal("criadopor");
                    bool criadoPorNull = res1.IsDBNull(coluna);
                    ulong criadoPorId =  criadoPorNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("modificadopor");
                    bool modificadoPorNull = res1.IsDBNull(coluna);
                    ulong modificadoPorId = modificadoPorNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idlocal");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    string descricao = res1["descricao"].ToString();
                    res1.Close();
                    // criar objeto
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    resultado = new Local(id, descricao, 
                        criadoEm, criadoPorNull ? null : uDao.GetById(criadoPorId), 
                        modificadoEm, modificadoPorNull ? null : uDao.GetById(modificadoPorId));
                    // marcar objeto acabado de carregar
                    resultado.DataCheckpointDb();
                }
                if (!res1.IsClosed) res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"LocalDao.GetById({idLocal})", e);
            }
            // fornecer objeto
            return resultado;
        }
        
        /// <summary>
        /// Ler um local da BD, pesquisando pela descrição.
        /// </summary>
        /// <param name="descricao"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public Local GetByDescricao(string descricao)
        {
            if (!(descricao.Trim().Length > 0)) throw new MyException("LocalDao.GetByDescricao()-> descricao tem que estar preenchida!");
            Local resultado = null;
            string sql1 = "select idlocal from osler.local where descricao = @Descricao ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@Descricao", descricao.Trim());
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read()) {
                    int coluna = res1.GetOrdinal("idlocal");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    res1.Close();
                    // carregar objeto da BD
                    resultado = GetById(id);
                } 
                res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"LocalDao.GetByDescricao('{descricao}')", e);
            }
            // devolver objeto
            return resultado;
        }
        
        /// <summary>
        /// lê o Local(1) "Sala de espera" é o local por omissão (cria o registo na BD caso não exista)
        /// </summary>
        /// <returns></returns>
        public Local GetDefaultLocal(Utilizador utilizadorAtivo = null)
        {
            Local resultado = GetById(Local.DefaultLocalId());
            if (resultado == null)
            {
                try
                {
                    resultado = new Local(Local.DefaultLocalId(), Local.DefaultLocalDescricao(), DateTime.Now, utilizadorAtivo, DateTime.Now, utilizadorAtivo);
                    SaveObj(resultado);
                }
                catch (Exception e)
                {
                    throw new MyException("LocalDao.GetDefaultLocal()", e);
                }
            }
            return resultado;

        }

        /// <summary>
        /// criar um novo local
        /// </summary>
        /// <param name="descricao"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public Local NewLocal(string descricao, Utilizador utilizadorAtivo = null)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("LocalDao.NewLocal()-> descricao tem que estar preenchida!");
            Local resultado = GetByDescricao(descricao);
            if (resultado == null) {
                try
                {
                    resultado = new Local(descricao, utilizadorAtivo);
                    SaveObj(resultado, utilizadorAtivo);
                }
                catch (Exception e)
                {
                    throw new MyException($"LocalDao.NewLocal({descricao})", e);
                }
            }
            return resultado;
        }
        
        /// <summary>
        /// Gravar/Atualizar um local na BD
        /// </summary>
        /// <param name="local"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public bool SaveObj(Local local, Utilizador utilizadorAtivo=null)
        {
            if (ReferenceEquals(local, null)) throw new MyException("LocalDao.SaveObj(null)->recebeu objeto vazio!");
            if (local.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                Local temp = GetById(local.IdLocal);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null)
                {
                    // INSERT INTO osler.local(
                    //     idlocal, descricao, criadopor, criadoem, modificadopor, modificadoem)
                    // VALUES (?, ?, ?, ?, ?, ?);
                    sqla = "INSERT INTO osler.local(idlocal, descricao, criadoem, modificadoem";
                    sqlb = "VALUES (@IdLocal, @Descricao, @CriadoEm, @ModificadoEm";
                    if (local.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (local.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    
                    local.CriadoPor = utilizadorAtivo ?? local.CriadoPor;
                } else { 
                    // UPDATE osler.local SET idlocal=?, descricao=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?
                    // WHERE <condition>;
                    sqla = "UPDATE osler.local SET descricao=@Descricao, criadoem=@CriadoEm, modificadoem=@ModificadoEm";
                    sqlb = " WHERE idlocal=@IdLocal ;";
                    if (local.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (local.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";

                }

                NpgsqlTransaction tr = null;
                try
                {
                    local.ModificadoPor = utilizadorAtivo ?? local.ModificadoPor;
                    if (local.CriadoPor != null || local.ModificadoPor != null) {
                        UtilizadorDao uDao = new UtilizadorDao(Db);
                        if (local.CriadoPor != null) 
                            uDao.VerifyPersistent(local.CriadoPor, utilizadorAtivo);
                        if (local.ModificadoPor != null) 
                            uDao.VerifyPersistent(local.ModificadoPor, utilizadorAtivo);
                    }
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdLocal", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(local.IdLocal));
                    com1.Parameters.AddWithValue("@Descricao", local.Descricao);
                    com1.Parameters.AddWithValue("@CriadoEm", local.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", local.ModificadoEm);
                    if (local.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(local.CriadoPor.IdUtilizador));
                    if (local.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(local.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    local.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"LocalDao.SaveObj({local.IdLocal})", e);
                }
            }
            return false;
        }
        
        /// <summary>
        /// devolve uma lista(objeto json) de locais
        /// </summary>
        /// <returns></returns>
        public RecIdTexto[] GetList()
        {
            List<RecIdTexto> lista = new List<RecIdTexto>();
            string sql1 = "select idlocal, descricao from osler.local order by descricao ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows) {
                    while (res1.Read())
                    {
                        int coluna = res1.GetOrdinal("idlocal");
                        ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                        lista.Add(new RecIdTexto(id, res1["descricao"].ToString()));
                    }
                } 
                res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"LocalDao.GetList()", e);
            }
            return lista.ToArray();
        }
        
    }
}