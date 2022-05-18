/*
*	<description>EpisodioHistLocalDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
* 	<author>João Carlos Pinto</author>
*   <date>16-04-2022</date>
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
    public class EpisodioHistLocalDao: BaseDao
    {
        private EpisodioHistLocal _currentobj;
        public EpisodioHistLocalDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }

        public EpisodioHistLocal CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        /// <summary>
        /// verifica se o objeto do tipo "EpisodioHistLocal" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="episodioHistLocal"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(EpisodioHistLocal episodioHistLocal, Utilizador utilizadorAtivo=null)
        {
            if (!episodioHistLocal.IsPersistent || episodioHistLocal.IsModified()) SaveObj(episodioHistLocal, utilizadorAtivo);
        }

        /// <summary>
        /// Ler um EpisodioHistLocal da BD
        /// </summary>
        /// <param name="idEpisodioHistLocal"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public EpisodioHistLocal GetById(ulong idEpisodioHistLocal)
        {
            EpisodioHistLocal resultado = null;
            // SELECT idhistoricolocal, idlocal, idepisodio, datahora, criadopor, criadoem, modificadopor, modificadoem
            // FROM osler.episodiohistlocal;
            string sql1 = "select idhistoricolocal, idlocal, idepisodio, datahora, criadopor, criadoem, modificadopor, modificadoem from osler.episodiohistlocal where idhistoricolocal = @IdHistoricoLocal ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdHistoricoLocal", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idEpisodioHistLocal));
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
                    bool idLocalNull = res1.IsDBNull(coluna);
                    ulong idLocalId = idLocalNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idhistoricolocal");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idepisodio");
                    bool idEpisodioNull = res1.IsDBNull(coluna);
                    ulong idEpisodio = idEpisodioNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("datahora");
                    DateTime dataHora = res1.GetDateTime(coluna);
                    res1.Close();
                    // criar objeto
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    EpisodioDao eDao = new EpisodioDao(Db);
                    LocalDao lDao = new LocalDao(Db);
                    resultado = new EpisodioHistLocal(id, idLocalNull ? null : lDao.GetById(idLocalId),
                        dataHora, idEpisodioNull ? null : eDao.GetById(idEpisodio),
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
                throw new MyException($"EpisodioHistLocalDao.GetById({idEpisodioHistLocal})", e);
            }
            // fornecer objeto
            return resultado;
        }
        
        
        /// <summary>
        /// Ler um EpisodioHistLocal da BD
        /// </summary>
        /// <param name="idEpisodioProcurar"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public EpisodioHistLocal GetLastLocationFromEpisode(ulong idEpisodioProcurar)
        {
            EpisodioHistLocal resultado = null;
            // SELECT idhistoricolocal, idlocal, idepisodio, datahora, criadopor, criadoem, modificadopor, modificadoem
            // FROM osler.episodiohistlocal;
            string sql1 = "select idhistoricolocal, idlocal, idepisodio, datahora, criadopor, criadoem, modificadopor, modificadoem from osler.episodiohistlocal where idepisodio = @IdEpisodioAProcurar order by datahora desc LIMIT 1;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdEpisodioAProcurar", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idEpisodioProcurar));
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
                    bool idLocalNull = res1.IsDBNull(coluna);
                    ulong idLocalId = idLocalNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idhistoricolocal");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idepisodio");
                    bool idEpisodioNull = res1.IsDBNull(coluna);
                    ulong idEpisodio = idEpisodioNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("datahora");
                    DateTime dataHora = res1.GetDateTime(coluna);
                    res1.Close();
                    // criar objeto
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    EpisodioDao eDao = new EpisodioDao(Db);
                    LocalDao lDao = new LocalDao(Db);
                    resultado = new EpisodioHistLocal(id, 
                        idLocalNull ? null : lDao.GetById(idLocalId),
                        dataHora, idEpisodioNull ? null : eDao.GetById(idEpisodio),
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
                throw new MyException($"EpisodioHistLocalDao.GetLastLocationFromEpisode({idEpisodioProcurar})", e);
            }
            // fornecer objeto
            return resultado;
        }
        

        /// <summary>
        /// criar novo EpisodioHistLocal
        /// </summary>
        /// <param name="local"></param>
        /// <param name="dataHora"></param>
        /// <param name="episodio"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public EpisodioHistLocal NewEpisodioHistLocal(Local local, DateTime dataHora, Episodio episodio, Utilizador utilizadorAtivo = null)
        {
            if (ReferenceEquals(local, null)) 
                throw new MyException("EpisodioHistLocalDao.NewEpisodioHistLocal(local)->recebeu objeto vazio!");
            if (ReferenceEquals(episodio, null)) 
                throw new MyException("EpisodioHistLocalDao.NewEpisodioHistLocal(episodio)->recebeu objeto vazio!");
            EpisodioHistLocal resultado;
            try
            {
                resultado = new EpisodioHistLocal(local, dataHora, episodio, utilizadorAtivo);
                SaveObj(resultado, utilizadorAtivo);
            }
            catch (Exception e)
            {
                throw new MyException("EpisodioHistLocalDao.NewEpisodioHistLocal()->create", e);
            }
            return resultado;
        }
        
        /// <summary>
        /// Gravar/Atualizar um EpisodioHistLocal na BD
        /// </summary>
        /// <param name="episodioHistLocal"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool SaveObj(EpisodioHistLocal episodioHistLocal, Utilizador utilizadorAtivo = null)
        {
            if (ReferenceEquals(episodioHistLocal, null)) 
                throw new MyException("EpisodioHistLocalDao.SaveObj(null)->recebeu objeto vazio!");
            if (episodioHistLocal.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                EpisodioHistLocal temp = GetById(episodioHistLocal.IdHistoricoLocal);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null) {
                    // INSERT INTO osler.episodiohistlocal(
                    //     idhistoricolocal, idlocal, idepisodio, datahora, criadopor, criadoem, modificadopor, modificadoem)
                    // VALUES (?, ?, ?, ?, ?, ?, ?, ?);                   
                    sqla = "INSERT INTO osler.episodiohistlocal(idhistoricolocal, idlocal, idepisodio, datahora, criadoem, modificadoem";
                    sqlb = "VALUES (@IdHistoricoLocal, @IdLocal, @IdEpisodio, @DataHora, @CriadoEm, @ModificadoEm";
                    if (episodioHistLocal.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (episodioHistLocal.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    
                    episodioHistLocal.CriadoPor = utilizadorAtivo ?? episodioHistLocal.CriadoPor;
                } else {
                    // UPDATE osler.episodiohistlocal
                    // SET idhistoricolocal=?, idlocal=?, idepisodio=?, datahora=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?
                    // WHERE <condition>;
                    sqla = "UPDATE osler.pergunta SET idlocal=@IdLocal, idepisodio=@IdEpisodio, datahora=@DataHora, criadoem=@CriadoEm, modificadoem=@ModificafoEm";
                    sqlb = " WHERE idhistoricolocal=@IdHistoricoLocal ;";
                    if (episodioHistLocal.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (episodioHistLocal.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";
                }

                NpgsqlTransaction tr = null;
                try 
                {
                    LocalDao lDao = new LocalDao(Db);
                    lDao.VerifyPersistent(episodioHistLocal.Local, utilizadorAtivo);
                    EpisodioDao eDao = new EpisodioDao(Db);
                    eDao.VerifyPersistent(episodioHistLocal.Episodio, utilizadorAtivo);
                    episodioHistLocal.ModificadoPor = utilizadorAtivo ?? episodioHistLocal.ModificadoPor;
                    if (episodioHistLocal.CriadoPor != null || episodioHistLocal.ModificadoPor != null) {
                        UtilizadorDao uDao = new UtilizadorDao(Db);
                        if (episodioHistLocal.CriadoPor != null) 
                            uDao.VerifyPersistent(episodioHistLocal.CriadoPor, utilizadorAtivo);
                        if (episodioHistLocal.ModificadoPor != null) 
                            uDao.VerifyPersistent(episodioHistLocal.ModificadoPor, utilizadorAtivo);
                    }
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdHistoricoLocal", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioHistLocal.IdHistoricoLocal));
                    com1.Parameters.AddWithValue("@IdLocal", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioHistLocal.Local.IdLocal));
                    com1.Parameters.AddWithValue("@IdEpisodio", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioHistLocal.Episodio.IdEpisodio));
                    com1.Parameters.AddWithValue("@DataHora", episodioHistLocal.DataHora);
                    com1.Parameters.AddWithValue("@CriadoEm", episodioHistLocal.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", episodioHistLocal.ModificadoEm);
                    if (episodioHistLocal.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(episodioHistLocal.CriadoPor.IdUtilizador));
                    if (episodioHistLocal.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(episodioHistLocal.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    episodioHistLocal.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"EpisodioHistLocalDao.SaveObj({episodioHistLocal.IdHistoricoLocal})", e);
                }
            }
            return false;
        }

        /// <summary>
        /// devolve uma lista(objeto json) de episódios num Local
        /// </summary>
        /// <param name="idLocal"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public string GetListByLocal(ulong idLocal)
        {
            List<RecIdTexto> lista = new List<RecIdTexto>();
            // SELECT idhistoricolocal, idlocal, idepisodio, datahora, criadopor, criadoem, modificadopor, modificadoem
            // FROM osler.episodiohistlocal;
            // ---------------
            // select A.idhistoricolocal, A.idepisodio, B.codepisodiotxt, B.descricao 
            // from osler.episodiohistlocal A inner join osler.episodio B using (idepisodio)
            // where (A.idlocal = @IdLocal) and (B.estado=0) order by B.dataaberto
            // ---------------
            //string sql1 = "select idhistoricolocal, idepisodio from osler.episodiohistlocal where idlocal = @IdLocal order by datahora ;";
            string sql1 = "select A.idhistoricolocal, A.idepisodio, B.codepisodiotxt, B.descricao "+
                          "from osler.episodiohistlocal A inner join osler.episodio B using (idepisodio) "+
                          "where (A.idlocal = @IdLocal) and (B.estado=0) order by B.dataaberto ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdLocal", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idLocal));
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows) {
                    while (res1.Read())
                    {
                        int coluna = res1.GetOrdinal("idepisodio");
                        ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                        string txt=res1["codepisodiotxt"]+", "+res1["descricao"];
                        lista.Add(new RecIdTexto(id, txt));
                    }
                } 
                res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"EpisodioHistLocalDao.GetListByLocal({idLocal})", e);
            }
            
            return JsonConvert.SerializeObject(lista);
        }

        /// <summary>
        /// devolve uma lista(objeto json) de locais por Episodio
        /// </summary>
        /// <param name="idEpisodio"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public string GetListByEpisodio(ulong idEpisodio)
        {
            List<RecIdTexto> lista = new List<RecIdTexto>();
            // SELECT idhistoricolocal, idlocal, idepisodio, datahora, criadopor, criadoem, modificadopor, modificadoem
            // FROM osler.episodiohistlocal;
            // ---------------
            string sql1 = "select A.idhistoricolocal, A.idlocal, B.descricao "+
                          "from osler.episodiohistlocal A inner join osler.local B using (idlocal) "+
                          "where (A.idepisodio = @IdEpisodio) order by A.datahora ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdEpisodio", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idEpisodio));
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
                throw new MyException($"EpisodioHistLocalDao.GetListByEpisodio({idEpisodio})", e);
            }
            
            return JsonConvert.SerializeObject(lista);
        }

    }
}