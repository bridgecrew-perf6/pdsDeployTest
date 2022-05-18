/*
*	<description>ItemFluxoManchesterDao.cs, objeto da camada "DAL"</description>
* 	<author>João Carlos Pinto</author>
*   <date>13-04-2022</date>
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
    public class ItemFluxoManchesterDao: BaseDao
    {
        private ItemFluxoManchester _currentobj;
        public ItemFluxoManchesterDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }

        public ItemFluxoManchester CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        /// <summary>
        /// verifica se o objeto do tipo "ItemFluxoManchester" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="itemFluxoManchester"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(ItemFluxoManchester itemFluxoManchester, Utilizador utilizadorAtivo=null)
        {
            if (!itemFluxoManchester.IsPersistent || itemFluxoManchester.IsModified()) SaveObj(itemFluxoManchester, utilizadorAtivo);
        }

        /// <summary>
        /// Ler uma ItemFluxoManchester da BD
        /// </summary>
        /// <param name="idItemFluxoManchester"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public ItemFluxoManchester GetById(ulong idItemFluxoManchester)
        {
            ItemFluxoManchester resultado = null;
            // SELECT iditemfluxomanchester, idcortriagem, descricao,
            //        criadopor, criadoem, modificadopor, modificadoem
            // FROM osler.itemfluxomanchester;
            string sql1 = "select iditemfluxomanchester, idcortriagem, descricao, criadopor, criadoem, modificadopor, modificadoem "+
                          "from osler.itemfluxomanchester where iditemfluxomanchester = @IdItemFluxoManchester ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdItemFluxoManchester", 
                    NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idItemFluxoManchester));
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
                    coluna = res1.GetOrdinal("idcortriagem");
                    bool idCodTriagemNull = res1.IsDBNull(coluna);
                    ulong idCodTriagem = Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("iditemfluxomanchester");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    string descricao = res1["descricao"].ToString();
                    res1.Close();
                    // criar objeto
                    CorTriagemDao cDao = new CorTriagemDao();
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    resultado = new ItemFluxoManchester(id, descricao, 
                        idCodTriagemNull ? null : cDao.GetById(idCodTriagem), 
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
                throw new MyException($"ItemFluxoManchesterDao.GetById({idItemFluxoManchester})", e);
            }
            // fornecer objeto
            return resultado;
        }
        
        /// <summary>
        /// Ler uma ItemFluxoManchester da BD pesquisando pela descricao
        /// </summary>
        /// <param name="descricao"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public ItemFluxoManchester GetByDescricao(string descricao)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("ItemFluxoManchesterDao.GetByDescricao()-> descricao tem que estar preenchida!");
            ItemFluxoManchester resultado = null;
            string sql1 = "select iditemfluxomanchester from osler.itemfluxomanchester where descricao = @Descricao ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@Descricao", descricao.Trim());
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read()) {
                    int coluna = res1.GetOrdinal("iditemfluxomanchester");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    res1.Close();
                    // carregar objeto da BD
                    resultado = GetById(id);
                } else 
                    res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"ItemFluxoManchesterDao.GetByDescricao('{descricao}')", e);
            }
            // devolver objeto
            return resultado;
        }
        
        /// <summary>
        /// criar um novo ItemFluxoManchester
        /// </summary>
        /// <param name="descricao"></param>
        /// <param name="classificacao"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public ItemFluxoManchester NewItemFluxoManchester(string descricao, CorTriagem classificacao, Utilizador utilizadorAtivo = null)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("ItemFluxoManchesterDao.NewItemFluxoManchester()-> descricao tem que estar preenchida!");
            if (ReferenceEquals(classificacao, null)) 
                throw new MyException("ItemFluxoManchesterDao.SaveObj()->recebeu objeto vazio!");
            ItemFluxoManchester resultado = GetByDescricao(descricao);
            if (resultado == null) {
                try
                {
                    resultado = new ItemFluxoManchester(descricao, classificacao, utilizadorAtivo);
                    SaveObj(resultado, utilizadorAtivo);
                }
                catch (Exception e)
                {
                    throw new MyException($"ItemFluxoManchesterDao.NewItemFluxoManchester({descricao})->create", e);
                }
            }
            else
            {
                try
                {
                    resultado.Classificacao = classificacao;
                    VerifyPersistent(resultado, utilizadorAtivo);
                }
                catch (Exception e)
                {
                    throw new MyException($"ItemFluxoManchesterDao.NewItemFluxoManchester({descricao})->update", e);
                }
            }
            return resultado;
        }
        
        /// <summary>
        /// Gravar/Atualizar uma ItemFluxoManchester na BD
        /// </summary>
        /// <param name="itemFluxoManchester"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool SaveObj(ItemFluxoManchester itemFluxoManchester, Utilizador utilizadorAtivo=null)
        {
            if (ReferenceEquals(itemFluxoManchester, null)) 
                throw new MyException("ItemFluxoManchesterDao.SaveObj(null)->recebeu objeto vazio!");
            if (itemFluxoManchester.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                ItemFluxoManchester temp = GetById(itemFluxoManchester.IdItemFluxoManchester);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null)
                {
                    // INSERT INTO osler.itemfluxomanchester(
                    //     iditemfluxomanchester, idcortriagem, descricao, criadopor, criadoem, modificadopor, modificadoem)
                    // VALUES (?, ?, ?, ?, ?, ?, ?);
                    sqla = "INSERT INTO osler.itemfluxomanchester(iditemfluxomanchester, idcortriagem, descricao, criadoem, modificadoem";
                    sqlb = "VALUES (@IdItemFluxoManchester, @IdCorTriagem, @Descricao, @CriadoEm, @ModificadoEm";
                    if (itemFluxoManchester.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (itemFluxoManchester.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    
                    itemFluxoManchester.CriadoPor = utilizadorAtivo ?? itemFluxoManchester.CriadoPor;
                } else { 
                    // UPDATE osler.itemfluxomanchester
                    // SET iditemfluxomanchester=?, idcortriagem=?, descricao=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?
                    // WHERE <condition>;
                    sqla = "UPDATE osler.itemfluxomanchester SET idcortriagem=@IdCorTriagem, descricao=@Descricao, criadoem=@CriadoEm, modificadoem=@ModificadoEm";
                    sqlb = " WHERE iditemfluxomanchester=@IdItemFluxoManchester ;";
                    if (itemFluxoManchester.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (itemFluxoManchester.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";

                }

                NpgsqlTransaction tr = null;
                try
                {
                    CorTriagemDao cDao = new CorTriagemDao();
                    cDao.VerifyPersistent(itemFluxoManchester.Classificacao);
                    itemFluxoManchester.ModificadoPor = utilizadorAtivo ?? itemFluxoManchester.ModificadoPor;
                    if (itemFluxoManchester.CriadoPor != null || itemFluxoManchester.ModificadoPor != null) {
                        UtilizadorDao uDao = new UtilizadorDao(Db);
                        if (itemFluxoManchester.CriadoPor != null) 
                            uDao.VerifyPersistent(itemFluxoManchester.CriadoPor, utilizadorAtivo);
                        if (itemFluxoManchester.ModificadoPor != null) 
                            uDao.VerifyPersistent(itemFluxoManchester.ModificadoPor, utilizadorAtivo);
                    }
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdItemFluxoManchester", 
                        NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(itemFluxoManchester.IdItemFluxoManchester));
                    com1.Parameters.AddWithValue("@IdCorTriagem", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(itemFluxoManchester.Classificacao.IdCorTriagem));
                    com1.Parameters.AddWithValue("@Descricao", itemFluxoManchester.Descricao);
                    com1.Parameters.AddWithValue("@CriadoEm", itemFluxoManchester.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", itemFluxoManchester.ModificadoEm);
                    if (itemFluxoManchester.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(itemFluxoManchester.CriadoPor.IdUtilizador));
                    if (itemFluxoManchester.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(itemFluxoManchester.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    itemFluxoManchester.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"ItemFluxoManchesterDao.SaveObj({itemFluxoManchester.IdItemFluxoManchester})", e);
                }
            }
            return false;
        }

        /// <summary>
        /// devolve uma lista(objeto json) de itemFluxoManchester
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public RecItemFluxoManchester[] GetList()
        {
            List<RecItemFluxoManchester> lista = new List<RecItemFluxoManchester>();
            string sql1 = "select iditemfluxomanchester from osler.itemfluxomanchester order by descricao ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows)
                {
                    List<ulong> lisTemp = new List<ulong>();
                    while (res1.Read())
                    {
                        int coluna = res1.GetOrdinal("iditemfluxomanchester");
                        ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                        lisTemp.Add(id);
                    }
                    res1.Close();
                    foreach (var id in lisTemp)
                    {
                        ItemFluxoManchester iTemp = GetById(id);
                        lista.Add(new RecItemFluxoManchester(
                            iTemp.IdItemFluxoManchester,
                            iTemp.Descricao,
                            iTemp.Classificacao.IdCorTriagem,
                            iTemp.Classificacao.Descricao,
                            iTemp.Classificacao.CodigoCorHex));
                    }
                } else 
                    res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"ItemFluxoManchesterDao.GetList()", e);
            }

            return lista.ToArray();
        }
        
    }
}