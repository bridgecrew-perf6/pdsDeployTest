/*
*	<description>EpisodioQuestionarioDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
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
    public class EpisodioQuestionarioDao: BaseDao
    {
        private EpisodioQuestionario _currentobj;
        public EpisodioQuestionarioDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }

        public EpisodioQuestionario CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        /// <summary>
        /// verifica se o objeto do tipo "EpisodioQuestionario" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="episodioQuestionario"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(EpisodioQuestionario episodioQuestionario, Utilizador utilizadorAtivo=null)
        {
            if (!episodioQuestionario.IsPersistent || episodioQuestionario.IsModified()) SaveObj(episodioQuestionario, utilizadorAtivo);
        }
        
        /// <summary>
        /// Ler um EpisodioQuestionario da BD
        /// </summary>
        /// <param name="idEpisodioQuestionario"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public EpisodioQuestionario GetById(ulong idEpisodioQuestionario)
        {
            EpisodioQuestionario resultado = null;
            /*
             * SELECT idepisodioquesstionario, idepisodio, idquestionario, sequenciaquestionario, criadopor, criadoem, modificadopor, modificadoem
             * FROM osler.episodioquestionario 
             */
            string sql = "SELECT idepisodioquestionario, idepisodio, idquestionario, sequenciaquestionario, criadopor, criadoem, modificadopor, modificadoem" +
                         " FROM osler.episodioquestionario WHERE idepisodioquestionario = @IdEpisodioQuestionario";
            try
            {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql, Db);
                qry1.Parameters.AddWithValue("@IdEpisodioQuestionario", NpgsqlDbType.Bigint, sizeof(Int64),
                    Convert.ToInt64(idEpisodioQuestionario));
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read())
                {
                    int coluna = res1.GetOrdinal("idepisodioquestionario");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idepisodio");
                    bool idEpisodioNull = res1.IsDBNull(coluna);
                    ulong idEpisodio = idEpisodioNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idquestionario");
                    bool idQuestionarioNull = res1.IsDBNull(coluna);
                    ulong idQuestionario = idQuestionarioNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("sequenciaquestionario");
                    short sequenciaQuestionario = Convert.ToInt16(res1.GetValue(coluna));
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
                    QuestionarioDao qDao = new QuestionarioDao(Db);
                    res1.Close();
                    resultado = new EpisodioQuestionario(
                        id,
                        idQuestionarioNull ? null : qDao.GetById(idQuestionario),
                        sequenciaQuestionario,
                        idEpisodioNull ? null : eDao.GetById(idEpisodio),
                        criadoEm, criadoPorNull ? null : uDao.GetById(criadoPorId),
                        modificadoEm, modificadoPorNull ? null : uDao.GetById(modificadoPorId));

                    resultado.DataCheckpointDb();
                }
                if (!res1.IsClosed) res1.Close();
                res1.Dispose();
                qry1.Dispose();
                Db.Close();
            }
            catch (Exception e)
            {
                throw new MyException($"EpisodioQuestionarioDao.GetById({idEpisodioQuestionario})", e);
            }

            return resultado;
        }
        
        /// <summary>
        /// criar novo EpisodioQuestionario
        /// </summary>
        /// <param name="episodio"></param>
        /// <param name="questionario"></param>
        /// <param name="sequenciaQuestionario"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public EpisodioQuestionario NewEpisodioRegistoDados(Episodio episodio, Questionario questionario, short sequenciaQuestionario, Utilizador utilizadorAtivo = null)
        {
            if (ReferenceEquals(episodio, null)) 
                throw new MyException("EpisodioQuestionarioDao.NewEpisodioRegistoDados(episodio)->recebeu objeto vazio!");
            if (ReferenceEquals(questionario, null)) 
                throw new MyException("EpisodioQuestionarioDao.NewEpisodioRegistoDados(questionario)->recebeu objeto vazio!");
            EpisodioQuestionario resultado;
            try
            {
                resultado = new EpisodioQuestionario(questionario, episodio, sequenciaQuestionario, utilizadorAtivo);
                SaveObj(resultado, utilizadorAtivo);
            }
            catch (Exception e)
            {
                throw new MyException("EpisodioRegistoDadosDao.NewEpisodioRegistoDados()->create", e);
            }
            return resultado;
        }
        
        /// <summary>
        /// Gravar/Atualizar um EpisodioQuestionario na BD
        /// </summary>
        /// <param name="episodioQuestionario"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool SaveObj(EpisodioQuestionario episodioQuestionario, Utilizador utilizadorAtivo = null)
        {
            if (ReferenceEquals(episodioQuestionario, null)) 
                throw new MyException("EpisodioQuestionarioDao.SaveObj(null)->recebeu objeto vazio!");
            if (episodioQuestionario.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                EpisodioQuestionario temp = GetById(episodioQuestionario.IdEpisodioQuestionario);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null) {
                    /*
                     * INSERT INTO osler.episodioquestionario(
                     * idepisodioquestionario, idepisodio, idquestionario, sequenciaquestionario, criadopor, criadoem, modificadopor, modificadoem)
                     * VALUES (?, ?, ?, ?, ?, ?, ?, ?);
                     */
                    sqla = "INSERT INTO osler.episodioquestionario(idepisodioquestionario, idepisodio, idquestionario, sequenciaquestionario, criadoem, modificadoem";
                    sqlb = "VALUES (@IdEpisodioQuestionario, @IdEpisodio, @IdQuestionario, @SequenciaQuestionario, @CriadoEm, @ModificadoEm";
                    if (episodioQuestionario.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (episodioQuestionario.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    
                    episodioQuestionario.CriadoPor = utilizadorAtivo ?? episodioQuestionario.CriadoPor;
                } else {
                    /*
                     * UPDATE osler.episodioquestionario
                     * SET idepisodioquestionario=?, idepisodio=?, idquestionario=?, sequenciaquestionario=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?
                     * WHERE <condition>;
                     */
                    sqla = "UPDATE osler.episodioquestionario SET idepisodio=@IdEpisodio, idquestionario=@IdQuestionario, sequenciaquestionario=@SequenciaQuestionario, criadoem=@CriadoEm, modificadoem=@ModificafoEm";
                    sqlb = " WHERE idepisodioquestionario=@IdEpisodioQuestionario ;";
                    if (episodioQuestionario.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (episodioQuestionario.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";
                }

                NpgsqlTransaction tr = null;
                try
                {
                    EpisodioDao eDao = new EpisodioDao(Db);
                    eDao.VerifyPersistent(episodioQuestionario.Episodio, utilizadorAtivo);
                    QuestionarioDao qDao = new QuestionarioDao(Db);
                    qDao.VerifyPersistent(episodioQuestionario.Questionario, utilizadorAtivo);
                    episodioQuestionario.ModificadoPor = utilizadorAtivo ?? episodioQuestionario.ModificadoPor;
                    if (episodioQuestionario.CriadoPor != null || episodioQuestionario.ModificadoPor != null) {
                        UtilizadorDao uDao = new UtilizadorDao(Db);
                        if (episodioQuestionario.CriadoPor != null) 
                            uDao.VerifyPersistent(episodioQuestionario.CriadoPor, utilizadorAtivo);
                        if (episodioQuestionario.ModificadoPor != null) 
                            uDao.VerifyPersistent(episodioQuestionario.ModificadoPor, utilizadorAtivo);
                    }
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdEpisodioQuestionario", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioQuestionario.IdEpisodioQuestionario));
                    com1.Parameters.AddWithValue("@IdEpisodio", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioQuestionario.Episodio.IdEpisodio));
                    com1.Parameters.AddWithValue("@IdQuestionario", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioQuestionario.Questionario.IdQuestionario));
                    com1.Parameters.AddWithValue("@SequenciaQuestionario", episodioQuestionario.SequenciaQuestionario);
                    com1.Parameters.AddWithValue("@CriadoEm", episodioQuestionario.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", episodioQuestionario.ModificadoEm);
                    if (episodioQuestionario.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(episodioQuestionario.CriadoPor.IdUtilizador));
                    if (episodioQuestionario.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(episodioQuestionario.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    episodioQuestionario.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"EpisodioQuestionarioDao.SaveObj({episodioQuestionario.IdEpisodioQuestionario})", e);
                }
            }
            return false;
        }
        
        /// <summary>
        /// devolve uma lista(objeto json) de questionarios de um episodio
        /// </summary>
        /// <param name="idEpisodio"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public string GetListByEpisodio(ulong idEpisodio)
        {
            List<RecEpisodioQuestionario> lista = new List<RecEpisodioQuestionario>();
            /*
             * SELECT idepisodioquestionario, idepisodio, idquestionario, sequenciaquestionario FROM osler.episodioquestionario ORDER BY sequenciaquestionario ASC
             */
            string sql = "select idepisodioquestionario, idepisodio, idquestionario, sequenciaquestionario "+
                          "from osler.episodioquestionario where idepisodio = @IdEpisodio ORDER BY sequenciaquestionario Desc ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql, Db);
                qry1.Parameters.AddWithValue("@IdEpisodio", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idEpisodio));
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows) {
                    while (res1.Read())
                    {
                        int coluna = res1.GetOrdinal("idepisodioquestionario");
                        ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                        coluna = res1.GetOrdinal("idepisodio");
                        ulong idEp = Convert.ToUInt64(res1.GetValue(coluna));
                        coluna = res1.GetOrdinal("idquestionario");
                        ulong idTipoLeitura = Convert.ToUInt64(res1.GetValue(coluna));
                        coluna = res1.GetOrdinal("sequenciaquestionario");
                        short sequenciaQuestionario = Convert.ToInt16(res1.GetValue(coluna));
                        lista.Add(new RecEpisodioQuestionario(id, idEp, 
                            idTipoLeitura, sequenciaQuestionario));
                    }
                } 
                res1.Close();
                res1.Dispose();
                Db.Close();
            } catch (Exception e) {
                throw new MyException($"EpisodioQuestionarioDao.GetListByEpisodio({idEpisodio})", e);
            }
            
            return JsonConvert.SerializeObject(lista);
        }
        
        
        /*
        /// <summary>
        /// remove uma lista de questionarios num episodio
        /// </summary>
        /// <param name="idEpisodio"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        private bool EliminarQuestionariosEpisodio(ulong idEpisodio, List<ulong> listaIdQuestionario, Utilizador utilizadorAtivo = null)
        {
            if (listaIdQuestionario.IsNullOrEmpty())
                throw new MyException($"EpisodioQuestionarioDao.ElimiarQuestionarioEpisodios({idEpisodio}) empty list idQuestionario");
            
            /*
             * DELETE FROM osler.episodioquestionario WHERE <condition>;
             #1#
            string whereCondition = " WHERE idepisodio = @IdEpisodio AND (";
            string sql = "DELETE FROM osler.episodioquestionario";
            for (int i = 0; i < listaIdQuestionario.Count; i++)
            {
                if (i > 0)
                    whereCondition += " OR";    
                whereCondition += $" idquestionario = {listaIdQuestionario[i]}";
            }
            sql += $"{whereCondition});";
            NpgsqlTransaction tr = null;
            try {
                DbOpen();
                tr = Db.BeginTransaction();
                NpgsqlCommand queryDelete = new NpgsqlCommand(sql, Db);
                queryDelete.Parameters.AddWithValue("@IdEpisodio", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idEpisodio));
                queryDelete.ExecuteNonQuery();

                
                sql = "UPDATE osler.episodioquestionario SET modificadoem = @ModificadoEm";
                if (utilizadorAtivo != null)
                {
                    sql += ", modificadopor = @ModificadoPor";
                    
                }
                sql += $"{whereCondition});";
                NpgsqlCommand queryUdate = new NpgsqlCommand(sql, Db);
                queryUdate.Parameters.AddWithValue("@IdEpisodio", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idEpisodio));
                queryUdate.Parameters.AddWithValue("@ModificadoEm", DateTime.Now);
                if (utilizadorAtivo != null)
                    queryUdate.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64),
                        Convert.ToInt64(utilizadorAtivo.IdUtilizador));
                queryUdate.ExecuteNonQuery();
                
                tr.Commit();
                return true;
            } catch (Exception e) {
                if (tr!=null) tr.Rollback();
                throw new MyException($"EpisodioQuestionarioDao.ElimiarQuestionarioEpisodios({idEpisodio})", e);
            }
        }
        */
        
        /*
        /// <summary>
        /// devolve uma lista(objeto json) de dados num episodio
        /// </summary>
        /// <param name="idEpisodio"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public string EliminarQuestionarios(ulong idEpisodio, List<ulong> listaIdQuestionario, Utilizador utilizadorAtivo = null)
        {
            EliminarQuestionariosEpisodio(idEpisodio, listaIdQuestionario, utilizadorAtivo);
            return GetListByEpisodio(idEpisodio);
        }
    */
    }
}