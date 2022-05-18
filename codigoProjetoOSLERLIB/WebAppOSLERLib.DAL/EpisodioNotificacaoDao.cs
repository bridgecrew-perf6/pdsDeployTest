/*
*	<description>EpisodioNotificacaoDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
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
    public class EpisodioNotificacaoDao: BaseDao
    {
        private EpisodioNotificacao _currentobj;
        public EpisodioNotificacaoDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }

        public EpisodioNotificacao CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        /// <summary>
        /// verifica se o objeto do tipo "EpisodioNotificacao" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="episodioNotificacao"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(EpisodioNotificacao episodioNotificacao, Utilizador utilizadorAtivo=null)
        {
            if (!episodioNotificacao.IsPersistent || episodioNotificacao.IsModified()) SaveObj(episodioNotificacao, utilizadorAtivo);
        }

        /// <summary>
        /// Ler um EpisodioNotificacao da BD
        /// </summary>
        /// <param name="idNotificacao"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public EpisodioNotificacao GetById(ulong idNotificacao)
        {
            EpisodioNotificacao resultado = null;
            // SELECT idnotificacao, idepisodio, datahora, mensagem, datahoraleitura, criadopor, criadoem, modificadopor, modificadoem
            // FROM osler.episodionotificacao;
            string sql1 = "select idnotificacao, idepisodio, datahora, mensagem, datahoraleitura, criadopor, criadoem, modificadopor, modificadoem from osler.episodionotificacao where idnotificacao = @IdNotificacao ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdNotificacao", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idNotificacao));
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
                    coluna = res1.GetOrdinal("idepisodio");
                    bool idEpisodioNull = res1.IsDBNull(coluna);
                    ulong idEpisodioId = idEpisodioNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idnotificacao");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("datahora");
                    DateTime dataHora = res1.GetDateTime(coluna);
                    coluna = res1.GetOrdinal("datahoraleitura");
                    DateTime? dataHoraLeitura = res1.IsDBNull(coluna) ? null : res1.GetDateTime(coluna);
                    coluna = res1.GetOrdinal("mensagem");
                    string mensagem = res1.GetString(coluna);
                    res1.Close();
                    // criar objeto
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    EpisodioDao eDao = new EpisodioDao(Db);
                    resultado = new EpisodioNotificacao(id, mensagem,
                        dataHora, dataHoraLeitura, 
                        idEpisodioNull ? null : eDao.GetById(idEpisodioId),
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
                throw new MyException($"EpisodioNotificacaoDao.GetById({idNotificacao})", e);
            }
            // fornecer objeto
            return resultado;
        }

        /// <summary>
        /// criar novo EpisodioNotificacao
        /// </summary>
        /// <param name="mensagem"></param>
        /// <param name="dataHora"></param>
        /// <param name="episodio"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public EpisodioNotificacao NewEpisodioNotificacao(string mensagem, DateTime dataHora, Episodio episodio, Utilizador utilizadorAtivo = null)
        {
            if (ReferenceEquals(episodio, null)) 
                throw new MyException("EpisodioNotificacaoDao.NewEpisodioNotificacao(episodio)->recebeu objeto vazio!");
            EpisodioNotificacao resultado;
            try
            {
                resultado = new EpisodioNotificacao(mensagem, dataHora, episodio, utilizadorAtivo);
                SaveObj(resultado, utilizadorAtivo);
            }
            catch (Exception e)
            {
                throw new MyException("EpisodioNotificacaoDao.NewEpisodioNotificacao()->create", e);
            }
            return resultado;
        }
        
        /// <summary>
        /// Gravar/Atualizar um EpisodioNotificacao na BD
        /// </summary>
        /// <param name="episodioNotificacao"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool SaveObj(EpisodioNotificacao episodioNotificacao, Utilizador utilizadorAtivo = null)
        {
            if (ReferenceEquals(episodioNotificacao, null)) 
                throw new MyException("EpisodioNotificacaoDao.SaveObj(null)->recebeu objeto vazio!");
            if (episodioNotificacao.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                EpisodioNotificacao temp = GetById(episodioNotificacao.IdNotificacao);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null) {
                    // INSERT INTO osler.episodionotificacao(
                    //     idnotificacao, idepisodio, datahora, mensagem, datahoraleitura, criadopor, criadoem, modificadopor, modificadoem)
                    // VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?);                   
                    sqla = "INSERT INTO osler.episodionotificacao(idnotificacao, idepisodio, datahora, mensagem, criadoem, modificadoem";
                    sqlb = "VALUES (@IdNotificacao, @IdEpisodio, @DataHora, @Mensagem, @CriadoEm, @ModificadoEm";
                    if (episodioNotificacao.DataHoraLeitura != null) {
                        sqla += ", datahoraleitura";
                        sqlb += ", @DataHoraLeitura";
                    }
                    if (episodioNotificacao.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (episodioNotificacao.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    
                    episodioNotificacao.CriadoPor = utilizadorAtivo ?? episodioNotificacao.CriadoPor;
                } else {
                    // UPDATE osler.episodionotificacao
                    // SET idnotificacao=?, idepisodio=?, datahora=?, mensagem=?, datahoraleitura=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?
                    // WHERE <condition>;
                    sqla = "UPDATE osler.episodionotificacao SET idlocal=@IdLocal, idepisodio=@IdEpisodio, datahora=@DataHora, criadoem=@CriadoEm, modificadoem=@ModificafoEm";
                    sqlb = " WHERE idnotificacao=@IdNotificacao ;";
                    if (episodioNotificacao.DataHoraLeitura != null) 
                        sqla += ", datahoraleitura=@DataHoraLeitura";
                    if (episodioNotificacao.CriadoPor != null) 
                        sqla += ", criadopor=@CriadoPor";
                    if (episodioNotificacao.ModificadoPor != null) 
                        sqla += ", modificadopor=@ModificadoPor";
                }

                NpgsqlTransaction tr = null;
                try 
                {
                    EpisodioDao eDao = new EpisodioDao(Db);
                    eDao.VerifyPersistent(episodioNotificacao.Episodio, utilizadorAtivo);
                    episodioNotificacao.ModificadoPor = utilizadorAtivo ?? episodioNotificacao.ModificadoPor;
                    if (episodioNotificacao.CriadoPor != null || episodioNotificacao.ModificadoPor != null) {
                        UtilizadorDao uDao = new UtilizadorDao(Db);
                        if (episodioNotificacao.CriadoPor != null) 
                            uDao.VerifyPersistent(episodioNotificacao.CriadoPor, utilizadorAtivo);
                        if (episodioNotificacao.ModificadoPor != null) 
                            uDao.VerifyPersistent(episodioNotificacao.ModificadoPor, utilizadorAtivo);
                    }
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdNotificacao", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioNotificacao.IdNotificacao));
                    com1.Parameters.AddWithValue("@IdEpisodio", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioNotificacao.Episodio.IdEpisodio));
                    com1.Parameters.AddWithValue("@DataHora", episodioNotificacao.DataHora);
                    com1.Parameters.AddWithValue("@CriadoEm", episodioNotificacao.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", episodioNotificacao.ModificadoEm);
                    if (episodioNotificacao.DataHoraLeitura != null) 
                        com1.Parameters.AddWithValue("@DataHoraLeitura", episodioNotificacao.DataHoraLeitura);
                    if (episodioNotificacao.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(episodioNotificacao.CriadoPor.IdUtilizador));
                    if (episodioNotificacao.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(episodioNotificacao.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    episodioNotificacao.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"EpisodioNotificacaoDao.SaveObj({episodioNotificacao.IdNotificacao})", e);
                }
            }
            return false;
        }

        /// <summary>
        /// devolve uma lista(objeto json) de notificações por episodio 
        /// </summary>
        /// <param name="idEpisodio"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public string GetListByEpisodio(ulong idEpisodio)
        {
            List<RecIdTexto> lista = new List<RecIdTexto>();
            // SELECT idnotificacao, idepisodio, datahora, mensagem, datahoraleitura, criadopor, criadoem, modificadopor, modificadoem
            // FROM osler.episodionotificacao;
            // ---------------
            //string sql1 = "select idhistoricolocal, idepisodio from osler.episodiohistlocal where idlocal = @IdLocal order by datahora ;";
            string sql1 = "select A.idnotificacao, A.idepisodio, A.datahora, A.mensagem, A.datahoraleitura "+
                          "from osler.episodionotificacao A "+
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
                        int coluna = res1.GetOrdinal("idnotificacao");
                        ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                        coluna = res1.GetOrdinal("datahora");
                        DateTime dataHora = res1.GetDateTime(coluna);
                        coluna = res1.GetOrdinal("datahoraleitura");
                        string txt = $"({dataHora}){(res1.IsDBNull(coluna) ? "" : "[V]")} {res1["mensagem"]}";
                        lista.Add(new RecIdTexto(id, txt));
                    }
                } 
                res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"EpisodioNotificacaoDao.GetListByEpisodio({idEpisodio})", e);
            }
            
            return JsonConvert.SerializeObject(lista);
        }
        
    }
}