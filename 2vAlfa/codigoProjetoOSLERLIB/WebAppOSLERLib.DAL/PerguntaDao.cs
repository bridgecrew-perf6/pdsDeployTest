/*
*	<description>PerguntaDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
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
    public class PerguntaDao: BaseDao
    {
        private Pergunta _currentobj;
        public PerguntaDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }

        public Pergunta CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        /// <summary>
        /// verifica se o objeto do tipo "Pergunta" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="pergunta"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(Pergunta pergunta, Utilizador utilizadorAtivo=null)
        {
            if (!pergunta.IsPersistent || pergunta.IsModified()) SaveObj(pergunta, utilizadorAtivo);
        }
        /// <summary>
        /// Ler uma pergunta da BD
        /// </summary>
        /// <param name="idPergunta"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public Pergunta GetById(ulong idPergunta)
        {
            Pergunta resultado = null;
            // SELECT idpergunta, idquestionario, sequenciapergunta, textopergunta, criadopor, criadoem, modificadopor, modificadoem
            // FROM osler.pergunta;
            string sql1 = "select idpergunta, idquestionario, sequenciapergunta, textopergunta, criadopor, criadoem, modificadopor, modificadoem from osler.pergunta where idpergunta = @IdPergunta ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdPergunta", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idPergunta));
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
                    coluna = res1.GetOrdinal("idquestionario");
                    bool questionarioNull = res1.IsDBNull(coluna);
                    ulong questionarioId = questionarioNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idpergunta");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    string textoPergunta = res1["textopergunta"].ToString();
                    coluna = res1.GetOrdinal("sequenciapergunta");
                    int sequenciaPergunta = Convert.ToInt32(res1.GetValue(coluna));
                    res1.Close();
                    // criar objeto
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    QuestionarioDao qDao = new QuestionarioDao(Db);
                    resultado = new Pergunta(id, textoPergunta, 
                        questionarioNull ? null : qDao.GetById(questionarioId),
                        sequenciaPergunta,
                        criadoEm, criadoPorNull ? null : uDao.GetById(criadoPorId), 
                        modificadoEm, modificadoPorNull ? null : uDao.GetById(modificadoPorId));
                    // marcar objeto acabado de carregar
                    resultado.DataCheckpointDb();
                }
                if (!res1.IsClosed) res1.Close();
            } catch (Exception e) {
                throw new MyException($"PerguntaDao.GetById({idPergunta})", e);
            }
            // fornecer objeto
            return resultado;
        }
        /// <summary>
        /// criar nova pergunta
        /// </summary>
        /// <param name="textoPergunta"></param>
        /// <param name="questionario"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public Pergunta NewPergunta(string textoPergunta, Questionario questionario, Utilizador utilizadorAtivo = null)
        {
            if (!(textoPergunta.Trim().Length > 0)) 
                throw new MyException("PerguntaDao.NewPergunta()-> descricao tem que estar preenchida!");
            if (ReferenceEquals(questionario, null)) 
                throw new MyException("QuestionarioDao.NewPergunta(questionario)->recebeu objeto vazio!");
            Pergunta resultado;
            try
            {
                resultado = new Pergunta(textoPergunta, questionario, utilizadorAtivo);
                SaveObj(resultado, utilizadorAtivo);
            }
            catch (Exception e)
            {
                throw new MyException($"PerguntaDao.NewQuestionario({textoPergunta})->create", e);
            }
            return resultado;
        }
        
        /// <summary>
        /// Gravar/Atualizar uma pergunta na BD
        /// </summary>
        /// <param name="pergunta"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool SaveObj(Pergunta pergunta, Utilizador utilizadorAtivo = null)
        {
            if (ReferenceEquals(pergunta, null)) 
                throw new MyException("PerguntaDao.SaveObj(null)->recebeu objeto vazio!");
            if (pergunta.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                Pergunta temp = GetById(pergunta.IdPergunta);
                bool tempNull = (temp == null);
                // testar se se deve inserir ou atualizar na BD
                if (tempNull) {
                    // INSERT INTO osler.pergunta(
                    //     idpergunta, idquestionario, sequenciapergunta, textopergunta, criadopor, criadoem, modificadopor, modificadoem)
                    // VALUES (?, ?, ?, ?, ?, ?, ?, ?);                   
                    sqla = "INSERT INTO osler.pergunta(idpergunta, idquestionario, sequenciapergunta, textopergunta, criadoem, modificadoem";
                    sqlb = "VALUES (@IdPergunta, @IdQuestionario, @SequenciaPergunta, @TextoPergunta, @CriadoEm, @ModificadoEm";
                    if (pergunta.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (pergunta.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    
                    pergunta.CriadoPor = utilizadorAtivo ?? pergunta.CriadoPor;
                } else {
                    // UPDATE osler.pergunta
                    // SET idpergunta=?, idquestionario=?, sequenciapergunta=?, textopergunta=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?
                    // WHERE <condition>;
                    sqla = "UPDATE osler.pergunta SET idquestionario=@IdQuestionario, sequenciapergunta=@SequenciaPergunta, textopergunta=@TextoPergunta, criadoem=@CriadoEm, modificadoem=@ModificafoEm";
                    sqlb = " WHERE idpergunta=@IdPergunta ;";
                    if (pergunta.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (pergunta.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";
                }

                NpgsqlTransaction tr = null;
                try 
                {
                    QuestionarioDao qDao = new QuestionarioDao(Db);
                    qDao.VerifyPersistent(pergunta.Questionario, utilizadorAtivo);
                    pergunta.ModificadoPor = utilizadorAtivo ?? pergunta.ModificadoPor;
                    if (pergunta.CriadoPor != null || pergunta.ModificadoPor != null) {
                        UtilizadorDao uDao = new UtilizadorDao(Db);
                        if (pergunta.CriadoPor != null) 
                            uDao.VerifyPersistent(pergunta.CriadoPor, utilizadorAtivo);
                        if (pergunta.ModificadoPor != null) 
                            uDao.VerifyPersistent(pergunta.ModificadoPor, utilizadorAtivo);
                    }
                    DbOpen();
                    tr = Db.BeginTransaction();
                    
                    //TODO:verificar/companhar gestao de sequencia
                    // determinar o número de sequência dentro da mesma transação...
                    if (tempNull)
                    {
                        NpgsqlCommand com2 = new NpgsqlCommand(
                            "select MAX(sequenciapergunta) as ultimo from osler.pergunta where idquestionario = @IdQuestionario ;", Db);
                        com2.Parameters.AddWithValue("@IdQuestionario", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(pergunta.Questionario.IdQuestionario));
                        NpgsqlDataReader res2 = com2.ExecuteReader();
                        if (res2.HasRows && res2.Read())
                        {
                            int coluna = res2.GetOrdinal("ultimo");
                            pergunta.SequenciaPergunta = Convert.ToInt32(res2.GetValue(coluna)) + 1;
                        }
                        res2.Close();
                        res2.Dispose();
                        com2.Dispose();
                    }
                    
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdPergunta", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(pergunta.IdPergunta));
                    com1.Parameters.AddWithValue("@IdQuestionario", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(pergunta.Questionario.IdQuestionario));
                    com1.Parameters.AddWithValue("@SequenciaPergunta", pergunta.SequenciaPergunta);
                    com1.Parameters.AddWithValue("@TextoPergunta", pergunta.TextoPergunta);
                    com1.Parameters.AddWithValue("@CriadoEm", pergunta.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", pergunta.ModificadoEm);
                    if (pergunta.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(pergunta.CriadoPor.IdUtilizador));
                    if (pergunta.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(pergunta.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    pergunta.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"PerguntaDao.SaveObj({pergunta.IdPergunta})", e);
                }
            }
            return false;
        }

        /// <summary>
        /// devolve uma lista(objeto json) de perguntas de um questionário
        /// </summary>
        /// <param name="idQuestionario"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public string GetListByQuestionario(ulong idQuestionario)
        {
            List<RecIdTexto> lista = new List<RecIdTexto>();
            // SELECT idpergunta, idquestionario, sequenciapergunta, textopergunta, criadopor, criadoem, modificadopor, modificadoem
            // FROM osler.pergunta;
            string sql1 = "select idpergunta, textopergunta from osler.pergunta where idquestionario = @IdQuestionario order by sequenciapergunta ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdQuestionario", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idQuestionario));
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows) {
                    while (res1.Read())
                    {
                        int coluna = res1.GetOrdinal("idpergunta");
                        ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                        lista.Add(new RecIdTexto(id, res1["textopergunta"].ToString()));
                    }
                } 
                res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"PerguntaDao.GetList({idQuestionario})", e);
            }
            
            return JsonConvert.SerializeObject(lista);
        }

   }
}