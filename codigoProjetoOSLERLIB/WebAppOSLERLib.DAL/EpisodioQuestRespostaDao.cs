using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;
using WebAppOSLERLib.BO;
using WebAppOSLERLib.Consts;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.DAL
{
    public class EpisodioQuestRespostaDao: BaseDao
    {
        private EpisodioQuestResposta _currentObj;
        
        public EpisodioQuestRespostaDao(NpgsqlConnection db=null):base(db)
        {
        }

        public EpisodioQuestResposta CurrentObj
        {
            get => _currentObj;
            set => _currentObj = value;
        }
        
        /// <summary>
        /// verifica se o objeto do tipo "EpisodioQuestResposta" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="episodioQuestResposta"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(EpisodioQuestResposta episodioQuestResposta, Utilizador utilizadorAtivo=null)
        {
            if (!episodioQuestResposta.IsPersistent || episodioQuestResposta.IsModified()) SaveObj(episodioQuestResposta, utilizadorAtivo);
        }
        
        
       /// <summary>
       /// Ler um EpisodioQuestionario da BD
       /// </summary>
       /// <param name="idEpisodioQuestionarioPar"></param>
       /// <param name="sequenciaRespostaPar"></param>
       /// <returns></returns>
       /// <exception cref="MyException"></exception>
        public EpisodioQuestResposta GetByIdEpQuestionaSequeResp(ulong idEpisodioQuestionarioPar, short sequenciaRespostaPar)
        {
            EpisodioQuestResposta resultado = null;
            /*
             * SELECT idepisodioquestionario, sequenciaresposta, idquestionario, idpergunta, resposta, ativo, criadopor, criadoem, modificadopor, modificadoem
             *  FROM osler.episodioquestresposta 
             */
            string sql = "SELECT idepisodioquestionario, sequenciaresposta, idquestionario, idpergunta, resposta, ativo, criadopor, criadoem, modificadopor, modificadoem" +
                         " FROM osler.episodioquestresposta WHERE idepisodioquestionario = @IdEpisodioQuestionario AND sequenciaresposta = @SequenciaResposta";
            try
            {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql, Db);
                qry1.Parameters.AddWithValue("@IdEpisodioQuestionario", NpgsqlDbType.Bigint, sizeof(Int64),
                    Convert.ToInt64(idEpisodioQuestionarioPar));
                qry1.Parameters.AddWithValue("@SequenciaResposta", sequenciaRespostaPar);
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read())
                {
                    int coluna = res1.GetOrdinal("idepisodioquestionario");
                    bool idEpisodioQuestionarioNull = res1.IsDBNull(coluna);
                    ulong idEpisodioQuestionario = Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("sequenciaresposta");
                    short sequenciaResposta = Convert.ToInt16(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idquestionario");
                    bool idQuestionarioNull = res1.IsDBNull(coluna);
                    ulong idQuestionario = idQuestionarioNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idpergunta");
                    bool idPerguntaNull = res1.IsDBNull(coluna);
                    ulong idPergunta = idPerguntaNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    string resposta = res1["resposta"].ToString();
                    coluna = res1.GetOrdinal("ativo");
                    bool ativo = res1.GetBoolean(coluna);
                    
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
                    EpisodioQuestionarioDao eqDao = new EpisodioQuestionarioDao(Db);
                    QuestionarioDao qDao = new QuestionarioDao(Db);
                    PerguntaDao pDao = new PerguntaDao(Db);
                    resultado = new EpisodioQuestResposta(
                        idEpisodioQuestionarioNull ? null : eqDao.GetById(idEpisodioQuestionario),
                        sequenciaResposta,
                        idQuestionarioNull ? null : qDao.GetById(idQuestionario),
                        idPerguntaNull ? null : pDao.GetById(idPergunta),
                        resposta, ativo,
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
                throw new MyException($"EpisodioQuestionarioDao.GetById({idEpisodioQuestionarioPar}, {sequenciaRespostaPar})", e);
            }

            return resultado;
        }
        
        /// <summary>
        /// Criar novo EpisodioQuestResposta
        /// </summary>
        /// <param name="episodioQuestionario"></param>
        /// <param name="questionario"></param>
        /// <param name="pergunta"></param>
        /// <param name="resposta"></param>
        /// <param name="ativo"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public EpisodioQuestResposta NewEpisodioQuestResposta(EpisodioQuestionario episodioQuestionario, Questionario questionario, Pergunta pergunta, string resposta, bool ativo, Utilizador utilizadorAtivo = null)
        {
            if (ReferenceEquals(episodioQuestionario, null)) 
                throw new MyException("EpisodioQuestRespostaDao.NewEpisodioQuestResposta(episodioQuestionario)->recebeu objeto vazio!");
            if (ReferenceEquals(questionario, null)) 
                throw new MyException("EpisodioQuestRespostaDao.NewEpisodioQuestResposta(questionario)->recebeu objeto vazio!");
            if (ReferenceEquals(pergunta, null)) 
                throw new MyException("EpisodioQuestRespostaDao.NewEpisodioQuestResposta(pergunta)->recebeu objeto vazio!");
            EpisodioQuestResposta resultado;
            try
            {
                resultado = new EpisodioQuestResposta(episodioQuestionario, questionario, pergunta, resposta, ativo, utilizadorAtivo);
                SaveObj(resultado, utilizadorAtivo);
            }
            catch (Exception e)
            {
                throw new MyException("EpisodioQuestRespostaDao.NewEpisodioQuestResposta()->create", e);
            }
            return resultado;
        }

        /// <summary>
        /// Gravar/Atualizar um EpisodioQuestResposta na BD
        /// </summary>
        /// <param name="episodioQuestResposta"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool SaveObj(EpisodioQuestResposta episodioQuestResposta, Utilizador utilizadorAtivo = null)
        {
            if (ReferenceEquals(episodioQuestResposta, null)) 
                throw new MyException("EpisodioQuestRespostaDao.SaveObj(null)->recebeu objeto vazio!");
            if (ReferenceEquals(episodioQuestResposta.CriadoPor, null)) 
                throw new MyException("EpisodioQuestRespostaDao.SaveObj(null)->recebeu objeto sem criador!");
            
            if (episodioQuestResposta.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                EpisodioQuestResposta temp = GetByIdEpQuestionaSequeResp(episodioQuestResposta.EpisodioQuestionario.IdEpisodioQuestionario, episodioQuestResposta.SequenciaResposta);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null) {
                    /*
                     *INSERT INTO osler.episodioquestresposta(
                     * idepisodioquestionario, sequenciaresposta, idquestionario, idpergunta, resposta, ativo, criadopor, criadoem, modificadopor, modificadoem)
                     * VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
                     * 
                     */
                    sqla = "INSERT INTO osler.episodioquestresposta(idepisodioquestionario, sequenciaresposta, idquestionario, idpergunta, resposta, ativo, criadoem, modificadoem, criadopor";
                    sqlb = "VALUES (@IdEpisodioQuestionario, @SequenciaResposta, @IdQuestionario, @IdPergunta, @Resposta, @Ativo, @CriadoEm, @ModificadoEm, @CriadoPor";
                    if (episodioQuestResposta.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    
                    episodioQuestResposta.CriadoPor = utilizadorAtivo ?? episodioQuestResposta.CriadoPor;
                } else {
                    /*
                     * UPDATE osler.episodioquestresposta
                     * SET idepisodioquestionario=?, sequenciaresposta=?, idquestionario=?, idpergunta=?, resposta=?, ativo=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?
                     * WHERE <condition>;
                     */
                    sqla = "UPDATE osler.episodioquestresposta SET idquestionario=@IdQuestionario, idpergunta = @IdPergunta, resposta = @Resposta, ativo = @Ativo, criadoem=@CriadoEm, modificadoem=@ModificafoEm, criadopor=@CriadoPor";
                    sqlb = " WHERE idepisodioquestionario = @IdEpisodioQuestionario AND sequenciaresposta = @SequenciaResposta;";
                    if (episodioQuestResposta.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";
                }

                NpgsqlTransaction tr = null;
                try
                {
                    EpisodioQuestionarioDao eqDao = new EpisodioQuestionarioDao(Db);
                    eqDao.VerifyPersistent(episodioQuestResposta.EpisodioQuestionario, utilizadorAtivo);
                    QuestionarioDao qDao = new QuestionarioDao(Db);
                    qDao.VerifyPersistent(episodioQuestResposta.Questionario, utilizadorAtivo);
                    PerguntaDao pDao = new PerguntaDao(Db);
                    pDao.VerifyPersistent(episodioQuestResposta.Pergunta, utilizadorAtivo);
                    episodioQuestResposta.ModificadoPor = utilizadorAtivo ?? episodioQuestResposta.ModificadoPor;
                    if (episodioQuestResposta.CriadoPor != null || episodioQuestResposta.ModificadoPor != null) {
                        UtilizadorDao uDao = new UtilizadorDao(Db);
                        if (episodioQuestResposta.CriadoPor != null) 
                            uDao.VerifyPersistent(episodioQuestResposta.CriadoPor, utilizadorAtivo);
                        if (episodioQuestResposta.ModificadoPor != null) 
                            uDao.VerifyPersistent(episodioQuestResposta.ModificadoPor, utilizadorAtivo);
                    }
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdEpisodioQuestionario", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioQuestResposta.EpisodioQuestionario.IdEpisodioQuestionario));
                    com1.Parameters.AddWithValue("@SequenciaResposta", episodioQuestResposta.SequenciaResposta);
                    com1.Parameters.AddWithValue("@IdQuestionario", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioQuestResposta.Questionario.IdQuestionario));
                    com1.Parameters.AddWithValue("@IdPergunta", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodioQuestResposta.Pergunta.IdPergunta));
                    com1.Parameters.AddWithValue("@Resposta", episodioQuestResposta.Resposta);
                    com1.Parameters.AddWithValue("@Ativo", episodioQuestResposta.Ativo);

                    com1.Parameters.AddWithValue("@CriadoEm", episodioQuestResposta.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", episodioQuestResposta.ModificadoEm);
                    if (episodioQuestResposta.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(episodioQuestResposta.CriadoPor.IdUtilizador));
                    if (episodioQuestResposta.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(episodioQuestResposta.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    episodioQuestResposta.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"EpisodioQuestRespostaDao.SaveObj({episodioQuestResposta.EpisodioQuestionario.IdEpisodioQuestionario}, {episodioQuestResposta.SequenciaResposta})", e);
                }
            }
            return false;
        }


         /// <summary>
         /// Obter respostas de um episodio
         /// </summary>
         /// <param name="idEpisodio"></param>
         /// <returns></returns>
         public List<RecQuestResposta> ObterRespostasByEpisodio(ulong idEpisodio)
         {
             List<RecQuestResposta> returnValue = new List<RecQuestResposta>();
             /*
              * SELECT eqr.idepisodioquestionario, p.textopergunta, eqr.resposta, eqr.criadoPor FROM osler.episodioquestionario eq 
                INNER JOIN osler.episodioquestresposta eqr ON eq.idepisodioquestionario = eqr.idepisodioquestionario
                INNER JOIN osler.pergunta p ON p.idPergunta = eqr.idpergunta
                WHERE idEpisodio = @idEpisodio
              */
              string sql =  "SELECT eqr.idepisodioquestionario, p.textopergunta, eqr.resposta, eqr.criadoPor FROM osler.episodioquestionario eq " + 
                            "INNER JOIN osler.episodioquestresposta eqr ON eq.idepisodioquestionario = eqr.idepisodioquestionario " +
                            "INNER JOIN osler.pergunta p ON p.idPergunta = eqr.idpergunta " +
                            "WHERE idEpisodio = @idEpisodio";
            try
            {
                DbOpen();
                NpgsqlCommand query = new NpgsqlCommand(sql, Db);
                query.Parameters.AddWithValue("@idEpisodio", NpgsqlDbType.Bigint, sizeof(Int64),
                    Convert.ToInt64(idEpisodio));
                NpgsqlDataReader res = query.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        int coluna = res.GetOrdinal("idepisodioquestionario");
                        ulong idEpisodioQuestionario = Convert.ToUInt64(res.GetValue(coluna));
                        string pergunta = res["textopergunta"].ToString();
                        string resposta = res["resposta"].ToString();
                        coluna = res.GetOrdinal("criadoPor");
                        ulong enviadoPor = Convert.ToUInt64(res.GetValue(coluna));
                        returnValue.Add(new RecQuestResposta(idEpisodioQuestionario, pergunta, resposta, enviadoPor));
                    }
                }
                if (!res.IsClosed) res.Close();
                res.Dispose();
                DbClose();
            }
            catch (Exception e)
            {
                DbClose();
                throw new MyException($"EpisodioQuestionarioDao.ObterRespostasByEpisodio({idEpisodio})", e);
            }

            return returnValue;
         }
    }
}
