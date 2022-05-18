/*
*	<description>EpisodioRegistoDadosDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
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
    public class EpisodioRegistoDadosDao: BaseDao
    {
        private EpisodioRegistoDados _currentobj;
        public EpisodioRegistoDadosDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }

        public EpisodioRegistoDados CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        /// <summary>
        /// verifica se o objeto do tipo "EpisodioRegistoDados" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="episodioRegistoDados"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(EpisodioRegistoDados episodioRegistoDados, Utilizador utilizadorAtivo=null)
        {
            if (!episodioRegistoDados.IsPersistent || episodioRegistoDados.IsModified()) SaveObj(episodioRegistoDados, utilizadorAtivo);
        }

        /// <summary>
        /// Ler um EpisodioRegistoDados da BD
        /// </summary>
        /// <param name="idEpisodioRegistoDados"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public EpisodioRegistoDados GetById(ulong idEpisodioRegistoDados)
        {
            EpisodioRegistoDados resultado = null;
            /*
             * SELECT idregistodados, idepisodio, idtipoleitura, datahora, valor, criadopor, criadoem, modificadopor, modificadoem
             * FROM osler.episodioregistodados
             */
            string sql = "SELECT idregistodados, idepisodio, idtipoleitura, datahora, valor, criadopor, criadoem, modificadopor, modificadoem FROM osler.episodioregistodados WHERE idregistodados = @IdEpisodioRegistoDados";
            try
            {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql, Db);
                qry1.Parameters.AddWithValue("@IdEpisodioRegistoDados", NpgsqlDbType.Bigint, sizeof(Int64),
                    Convert.ToInt64(idEpisodioRegistoDados));
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read())
                {
                    int coluna = res1.GetOrdinal("idregistodados");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idepisodio");
                    bool idEpisodioNull = res1.IsDBNull(coluna);
                    ulong idEpisodio = idEpisodioNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idtipoleitura");
                    bool idTipoLeituraNull = res1.IsDBNull(coluna);
                    ulong idTipoLeitura = idTipoLeituraNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("datahora");
                    DateTime dataHora = res1.GetDateTime(coluna);
                    string valor = res1["valor"].ToString();
                    coluna = res1.GetOrdinal("criadopor");
                    bool criadoPorNull = res1.IsDBNull(coluna);
                    ulong criadoPorId = criadoPorNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("criadoem");
                    DateTime? criadoEm = res1.IsDBNull(coluna) ? null : res1.GetDateTime(coluna);
                    coluna = res1.GetOrdinal("modificadopor");
                    bool modificadoPorNull = res1.IsDBNull(coluna);
                    ulong modificadoPorId = modificadoPorNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("modificadoem");
                    DateTime? modificadoEm = res1.IsDBNull(coluna) ? null : res1.GetDateTime(coluna);
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    EpisodioDao eDao = new EpisodioDao(Db);
                    TipoLeituraDao tlDao = new TipoLeituraDao(Db);
                    resultado = new EpisodioRegistoDados(
                        id,
                        idTipoLeituraNull ? null : tlDao.GetById(idTipoLeitura),
                        dataHora, valor,
                        idEpisodioNull ? null : eDao.GetById(idEpisodio),
                        criadoEm, criadoPorNull ? null : uDao.GetById(criadoPorId),
                        modificadoEm, modificadoPorNull ? null : uDao.GetById(modificadoPorId));

                    resultado.DataCheckpointDb();
                }
                if (!res1.IsClosed) res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            }
            catch (Exception e)
            {
                throw new MyException($"EpisodioRegistoDadosDao.GetById({idEpisodioRegistoDados})", e);
            }

            return resultado;
        }
        
        /// <summary>
        /// criar novo EpisodioRegistoDados
        /// </summary>
        /// <param name="episodio"></param>
        /// <param name="tipoLeitura"></param>
        /// <param name="valor"></param>
        /// <param name="dataHora"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public EpisodioRegistoDados NewEpisodioRegistoDados(Episodio episodio, TipoLeitura tipoLeitura, string valor, DateTime dataHora, Utilizador utilizadorAtivo = null)
        {
            if (ReferenceEquals(episodio, null)) 
                throw new MyException("EpisodioHistLocalDao.NewEpisodioHistLocal(episodio)->recebeu objeto vazio!");
            if (ReferenceEquals(tipoLeitura, null)) 
                throw new MyException("EpisodioHistLocalDao.NewEpisodioHistLocal(tipoLeitura)->recebeu objeto vazio!");
            if (ReferenceEquals(valor, null)) 
                throw new MyException("EpisodioHistLocalDao.NewEpisodioHistLocal(valor)->recebeu objeto vazio!");
            EpisodioRegistoDados resultado;
            try
            {
                resultado = new EpisodioRegistoDados(tipoLeitura, dataHora, valor, episodio, utilizadorAtivo);
                SaveObj(resultado, utilizadorAtivo);
            }
            catch (Exception e)
            {
                throw new MyException("EpisodioRegistoDadosDao.NewEpisodioRegistoDados()->create", e);
            }
            return resultado;
        }
        
        /// <summary>
        /// Gravar/Atualizar um EpisodioRegistoDados na BD
        /// </summary>
        /// <param name="episodioRegistoDados"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool SaveObj(EpisodioRegistoDados episodioRegistoDados, Utilizador utilizadorAtivo = null)
        {
            if (ReferenceEquals(episodioRegistoDados, null)) 
                throw new MyException("EpisodioRegistoDadosDao.SaveObj(null)->recebeu objeto vazio!");
            if (episodioRegistoDados.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                EpisodioRegistoDados temp = GetById(episodioRegistoDados.IdEpisodioRegistoDados);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null) {
                    /*
                     * INSERT INTO osler.episodioregistodados(
                     * idregistodados, idepisodio, idtipoleitura, datahora, valor, criadopor, criadoem, modificadopor, modificadoem)
                     * VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?);
                     */
                    sqla = "INSERT INTO osler.episodioregistodados(idregistodados, idepisodio, idtipoleitura, datahora, valor, criadoem, modificadoem";
                    sqlb = "VALUES (@IdRegistoDados, @IdEpisodio, @IdTipoLeitura, @DataHora, @Valor, @CriadoEm, @ModificadoEm";
                    if (episodioRegistoDados.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (episodioRegistoDados.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    
                    episodioRegistoDados.CriadoPor = utilizadorAtivo ?? episodioRegistoDados.CriadoPor;
                } else {
                    /*
                     * UPDATE osler.episodioregistodados
                     * SET idregistodados=?, idepisodio=?, idtipoleitura=?, datahora=?, valor=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?
                     * WHERE <condition>;
                     */
                    sqla = "UPDATE osler.episodioregistodados SET idepisodio=@IdEpisodio, idtipoleitura=@IdTipoLeitura, datahora=@DataHora, valor=@Valor, criadoem=@CriadoEm, modificadoem=@ModificafoEm";
                    sqlb = " WHERE idregistodados=@IdRegistoDados ;";
                    if (episodioRegistoDados.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (episodioRegistoDados.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";
                }

                NpgsqlTransaction tr = null;
                try
                {
                    EpisodioDao eDao = new EpisodioDao(Db);
                    eDao.VerifyPersistent(episodioRegistoDados.Episodio, utilizadorAtivo);
                    TipoLeituraDao tlDao = new TipoLeituraDao(Db);
                    tlDao.VerifyPersistent(episodioRegistoDados.TipoLeitura, utilizadorAtivo);
                    episodioRegistoDados.ModificadoPor = utilizadorAtivo ?? episodioRegistoDados.ModificadoPor;
                    if (episodioRegistoDados.CriadoPor != null || episodioRegistoDados.ModificadoPor != null) {
                        UtilizadorDao uDao = new UtilizadorDao(Db);
                        if (episodioRegistoDados.CriadoPor != null) 
                            uDao.VerifyPersistent(episodioRegistoDados.CriadoPor, utilizadorAtivo);
                        if (episodioRegistoDados.ModificadoPor != null) 
                            uDao.VerifyPersistent(episodioRegistoDados.ModificadoPor, utilizadorAtivo);
                    }
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdRegistoDados", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioRegistoDados.IdEpisodioRegistoDados));
                    com1.Parameters.AddWithValue("@IdEpisodio", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioRegistoDados.Episodio.IdEpisodio));
                    com1.Parameters.AddWithValue("@IdTipoLeitura", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioRegistoDados.TipoLeitura.IdTipoLeitura));
                    com1.Parameters.AddWithValue("@DataHora", episodioRegistoDados.DataHora);
                    com1.Parameters.AddWithValue("@Valor", episodioRegistoDados.Valor);
                    com1.Parameters.AddWithValue("@CriadoEm", episodioRegistoDados.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", episodioRegistoDados.ModificadoEm);
                    if (episodioRegistoDados.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(episodioRegistoDados.CriadoPor.IdUtilizador));
                    if (episodioRegistoDados.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(episodioRegistoDados.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    episodioRegistoDados.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"EpisodioRegistoDadosDao.SaveObj({episodioRegistoDados.IdEpisodioRegistoDados})", e);
                }
            }
            return false;
        }
        
        
        /// <summary>
        /// devolve uma lista(objeto json) de dados num episodio
        /// </summary>
        /// <param name="idEpisodio"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public string GetListByEpisodio(ulong idEpisodio)
        {
            List<RecRegistoDados> lista = new List<RecRegistoDados>();
            /*
             * SELECT idregistodados, idepisodio, idtipoleitura, datahora, valor, criadopor, criadoem, modificadopor, modificadoem
             * FROM osler.episodioregistodados WHERE idepisodio = @IdEpisodio ORDER BY datahora Desc
             */
            string sql = "select idepisodio, idtipoleitura, datahora, valor "+
                          "from osler.episodioregistodados where idepisodio = @IdEpisodio ORDER BY datahora Desc ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql, Db);
                qry1.Parameters.AddWithValue("@IdEpisodio", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idEpisodio));
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows) {
                    while (res1.Read())
                    {
                        int coluna = res1.GetOrdinal("idepisodio");
                        ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                        coluna = res1.GetOrdinal("idtipoleitura");
                        ulong idTipoLeitura = Convert.ToUInt64(res1.GetValue(coluna));
                        string valor = res1["valor"].ToString();
                        coluna = res1.GetOrdinal("datahora");
                        DateTime dataHora = res1.GetDateTime(coluna);
                        lista.Add(new RecRegistoDados(id, idTipoLeitura, valor, dataHora));
                    }
                } 
                res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"EpisodioRegistoDadosDao.GetListByEpisodio({idEpisodio})", e);
            }
            
            return JsonConvert.SerializeObject(lista);
        }
    }
}