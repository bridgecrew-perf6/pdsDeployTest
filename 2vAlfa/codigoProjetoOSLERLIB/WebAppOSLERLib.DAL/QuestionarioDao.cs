/*
*	<description>QuestionarioDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
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
    public class QuestionarioDao: BaseDao
    {
        private Questionario _currentobj;
        public QuestionarioDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }

        public Questionario CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        /// <summary>
        /// verifica se o objeto do tipo "Questionario" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="questionario"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(Questionario questionario, Utilizador utilizadorAtivo=null)
        {
            if (!questionario.IsPersistent || questionario.IsModified()) SaveObj(questionario, utilizadorAtivo);
        }

        /// <summary>
        /// Ler um Questionario da BD
        /// </summary>
        /// <param name="idQuestionario"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public Questionario GetById(ulong idQuestionario)
        {
            Questionario resultado = null;
            // SELECT idquestionario, ididioma, descricao, criadopor, criadoem, modificadopor, modificadoem
            // FROM osler.questionario;
            string sql1 = "select idquestionario, ididioma, descricao, criadopor, criadoem, modificadopor, modificadoem "+
                          "from osler.questionario where idquestionario = @IdQuestionario ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdQuestionario", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idQuestionario));
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
                    coluna = res1.GetOrdinal("ididioma");
                    bool idiomaNull = res1.IsDBNull(coluna);
                    ulong idiomaId = idiomaNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idquestionario");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    string descricao = res1["descricao"].ToString();
                    res1.Close();
                    // criar objeto
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    IdiomaDao iDao = new IdiomaDao(Db);
                    resultado = new Questionario(id, descricao, 
                        idiomaNull ? null : iDao.GetById(idiomaId), 
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
                throw new MyException($"QuestionarioDao.GetById({idQuestionario})", e);
            }
            // fornecer objeto
            return resultado;
        }

        /// <summary>
        /// Ler um Questionario da BD pesquisando pela descricao
        /// </summary>
        /// <param name="descricao"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public Questionario GetByDescricao(string descricao)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("QuestionarioDao.GetByDescricao()-> descricao tem que estar preenchida!");
            Questionario resultado = null;
            string sql1 = "select idquestionario from osler.questionario where descricao = @Descricao ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@Descricao", descricao.Trim());
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read()) {
                    int coluna = res1.GetOrdinal("idquestionario");
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
                throw new MyException($"QuestionarioDao.GetByDescricao('{descricao}')", e);
            }
            // devolver objeto
            return resultado;
        }
        
        /// <summary>
        /// criar um novo Questionario
        /// </summary>
        /// <param name="descricao"></param>
        /// <param name="idiomaQuestionario"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public Questionario NewQuestionario(string descricao, Idioma idiomaQuestionario, Utilizador utilizadorAtivo = null)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("QuestionarioDao.NewQuestionario()-> descricao tem que estar preenchida!");
            if (ReferenceEquals(idiomaQuestionario, null)) 
                throw new MyException("QuestionarioDao.NewQuestionario(idioma)->recebeu objeto vazio!");
            Questionario resultado = GetByDescricao(descricao);
            if (resultado == null) {
                try
                {
                    resultado = new Questionario(descricao, idiomaQuestionario, utilizadorAtivo);
                    SaveObj(resultado, utilizadorAtivo);
                }
                catch (Exception e)
                {
                    throw new MyException($"QuestionarioDao.NewQuestionario({descricao})->create", e);
                }
            }
            else
            {
                try
                {
                    resultado.IdiomaQuestionario = idiomaQuestionario;
                    VerifyPersistent(resultado, utilizadorAtivo);
                }
                catch (Exception e)
                {
                    throw new MyException($"QuestionarioDao.NewQuestionario({descricao})->update", e);
                }
            }
            return resultado;
        }
        
        /// <summary>
        /// Gravar/Atualizar uma questionario na BD
        /// </summary>
        /// <param name="questionario"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool SaveObj(Questionario questionario, Utilizador utilizadorAtivo = null)
        {
            if (ReferenceEquals(questionario, null)) 
                throw new MyException("QuestionarioDao.SaveObj(null)->recebeu objeto vazio!");
            if (questionario.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                Questionario temp = GetById(questionario.IdQuestionario);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null) {
                    // INSERT INTO osler.questionario(
                    //     idquestionario, ididioma, descricao, criadopor, criadoem, modificadopor, modificadoem)
                    // VALUES (?, ?, ?, ?, ?, ?, ?);                    
                    sqla = "INSERT INTO osler.questionario(idquestionario, ididioma, descricao, criadoem, modificadoem";
                    sqlb = "VALUES (@IdQuestionario, @IdIdioma, @Descricao, @CriadoEm, @ModificadoEm";
                    if (questionario.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (questionario.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    
                    questionario.CriadoPor = utilizadorAtivo ?? questionario.CriadoPor;
                } else {
                    // UPDATE osler.questionario
                    // SET idquestionario=?, ididioma=?, descricao=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?
                    // WHERE <condition>;
                    sqla = "UPDATE osler.questionario SET ididioma=@IdIdioma, descricao=@Descricao, criadoem=@CriadoEm, modificadoem=@ModificafoEm";
                    sqlb = " WHERE idquestionario=@IdQuestionario ;";
                    if (questionario.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (questionario.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";
                }

                NpgsqlTransaction tr = null;
                try 
                {
                    if (questionario.IdiomaQuestionario != null) {
                        IdiomaDao iDao = new IdiomaDao(Db);
                        iDao.VerifyPersistent(questionario.IdiomaQuestionario, utilizadorAtivo);
                    }
                    questionario.ModificadoPor = utilizadorAtivo ?? questionario.ModificadoPor;
                    if (questionario.CriadoPor != null || questionario.ModificadoPor != null) {
                        UtilizadorDao uDao = new UtilizadorDao(Db);
                        if (questionario.CriadoPor != null) 
                            uDao.VerifyPersistent(questionario.CriadoPor, utilizadorAtivo);
                        if (questionario.ModificadoPor != null) 
                            uDao.VerifyPersistent(questionario.ModificadoPor, utilizadorAtivo);
                    }
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdQuestionario", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(questionario.IdQuestionario));
                    com1.Parameters.AddWithValue("@IdIdioma", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(questionario.IdiomaQuestionario!.IdIdioma));
                    com1.Parameters.AddWithValue("@Descricao", questionario.Descricao);
                    com1.Parameters.AddWithValue("@CriadoEm", questionario.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", questionario.ModificadoEm);
                    if (questionario.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(questionario.CriadoPor.IdUtilizador));
                    if (questionario.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(questionario.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    questionario.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"QuestionarioDao.SaveObj({questionario.IdQuestionario})", e);
                }
            }
            return false;
        }

        /// <summary>
        /// devolve uma lista de questionarios
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public RecIdTexto[] GetList()
        {
            List<RecIdTexto> lista = new List<RecIdTexto>();
            string sql1 = "select idquestionario, descricao from osler.questionario order by descricao ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows) {
                    while (res1.Read())
                    {
                        int coluna = res1.GetOrdinal("idquestionario");
                        ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                        lista.Add(new RecIdTexto(id, res1["descricao"].ToString()));
                    }
                } 
                res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException("QuestionarioDao.GetList()", e);
            }
            //return JsonConvert.SerializeObject(lista);
            return lista.ToArray();
        }

    }
}