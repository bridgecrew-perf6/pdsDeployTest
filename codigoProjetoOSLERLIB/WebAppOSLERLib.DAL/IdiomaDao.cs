/*
*	<description>IdiomaDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
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
    public class IdiomaDao: BaseDao
    {
        private Idioma _currentobj;
        
        public IdiomaDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }
        public Idioma CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        
        /// <summary>
        /// Ler um idioma da BD utilizando um ID
        /// </summary>
        /// <param name="idIdioma"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public Idioma GetById(ulong idIdioma)
        {
            Idioma resultado = null;
            string sql1 = "select ididioma, descricao, criadopor, criadoem, modificadopor, modificadoem from osler.idioma where ididioma = @IdIdioma ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdIdioma", NpgsqlDbType.Bigint, sizeof(Int64), Convert.ToInt64(idIdioma));
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read())
                {
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
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    string descricao = res1["descricao"].ToString();
                    res1.Close();
                    // criar objeto
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    resultado = new Idioma(id,  descricao, 
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
                throw new MyException($"IdiomaDao.GetById({idIdioma})", e);
            }
            // fornecer objeto
            return resultado;
        }

        /// <summary>
        /// Ler um idioma da BD, pesquisando pela descrição.
        /// </summary>
        /// <param name="descricao"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public Idioma GetByDescricao(string descricao)
        {
            if (!(descricao.Trim().Length > 0)) return null;
            Idioma resultado = null;
            string sql1 = "select ididioma from osler.idioma where descricao = @Descricao ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@Descricao", descricao.Trim());
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read()) {
                    int coluna = res1.GetOrdinal("ididioma");
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
                throw new MyException($"IdiomaDao.GetByDescricao('{descricao}')", e);
            }
            // devolver objeto
            return resultado;
        }
        
        /// <summary>
        /// lê o Idioma(1) "Português" é o idioma por omissão (cria o registo na BD caso não exista)
        /// </summary>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        public Idioma GetDefaultIdioma(Utilizador utilizadorAtivo=null)
        {
            Idioma resultado = GetById(Idioma.DefaultIdiomaId());
            if (resultado == null) {
                try
                {
                    resultado = new Idioma(Idioma.DefaultIdiomaId(), Idioma.DefaultIdiomaDescricao(), DateTime.Now, utilizadorAtivo, DateTime.Now, utilizadorAtivo);
                    SaveObj(resultado, utilizadorAtivo);
                }
                catch (Exception e)
                {
                    throw new MyException("IdiomaDao.GetDefaultIdioma()", e);
                }
            }
            return resultado;
        }

        /// <summary>
        /// criar um novo idioma
        /// </summary>
        /// <param name="descricao"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public Idioma NewIdioma(string descricao, Utilizador utilizadorAtivo = null)
        {
            if (!(descricao.Trim().Length > 0)) return null;
            Idioma resultado = GetByDescricao(descricao);
            if (resultado == null) {
                try
                {
                    resultado = new Idioma(descricao, utilizadorAtivo);
                    SaveObj(resultado, utilizadorAtivo);
                }
                catch (Exception e)
                {
                    throw new MyException($"IdiomaDao.NewIdioma({descricao})", e);
                }
            }
            return resultado;
        }
        
        /// <summary>
        /// Gravar/Atualizar um idioma na BD
        /// </summary>
        /// <param name="idioma"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public bool SaveObj(Idioma idioma, Utilizador utilizadorAtivo=null)
        {
            if (ReferenceEquals(idioma, null)) throw new MyException("IdiomaDao.SaveObj(null)->recebeu objeto vazio!");
            if (idioma.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                Idioma temp = GetById(idioma.IdIdioma);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null)
                {
                    // INSERT INTO osler.idioma(ididioma, descricao, criadopor, criadoem, modificadopor, modificadoem)
                    // VALUES (?, ?, ?, ?, ?, ?);
                    sqla = "INSERT INTO osler.idioma(ididioma, descricao, criadoem, modificadoem";
                    sqlb = "VALUES (@IdIdioma, @Descricao, @CriadoEm, @ModificadoEm";
                    if (idioma.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (idioma.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    
                    idioma.CriadoPor = utilizadorAtivo ?? idioma.CriadoPor;
                } 
                else 
                {
                    // UPDATE osler.idioma SET ididioma=?, descricao=?, criadoem=?, modificadoem=?, criadopor=?, modificadopor=?
                    // WHERE <condition>;
                    sqla = "UPDATE osler.idioma SET descricao=@Descricao, criadoem=@CriadoEm, modificadoem=@ModificadoEm";
                    sqlb = " WHERE ididioma=@IdIdioma ;";
                    if (idioma.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (idioma.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";
                }

                NpgsqlTransaction tr = null;
                try
                {
                    idioma.ModificadoPor = utilizadorAtivo ?? idioma.ModificadoPor;
                    if (idioma.CriadoPor != null || idioma.ModificadoPor != null) {
                        UtilizadorDao uDao = new UtilizadorDao(Db);
                        if (idioma.CriadoPor != null) 
                            uDao.VerifyPersistent(idioma.CriadoPor, utilizadorAtivo);
                        if (idioma.ModificadoPor != null) 
                            uDao.VerifyPersistent(idioma.ModificadoPor, utilizadorAtivo);
                    }
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdIdioma", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(idioma.IdIdioma));
                    com1.Parameters.AddWithValue("@Descricao", idioma.Descricao);
                    com1.Parameters.AddWithValue("@CriadoEm", idioma.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", idioma.ModificadoEm);
                    if (idioma.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(idioma.CriadoPor.IdUtilizador));
                    if (idioma.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(idioma.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    idioma.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"IdiomaDao.SaveObj({idioma.IdIdioma},'{idioma.Descricao}')", e);
                }
            }
            return false;
        }
        
        /// <summary>
        /// verifica se o objeto do tipo "Idioma" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="idioma"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(Idioma idioma, Utilizador utilizadorAtivo=null)
        {
            if (!idioma.IsPersistent || idioma.IsModified()) SaveObj(idioma, utilizadorAtivo);
        }

        /// <summary>
        /// devolve uma lista(objeto json) de idiomas
        /// </summary>
        /// <returns></returns>
        public RecIdTexto[] GetList()
        {
            List<RecIdTexto> lista = new List<RecIdTexto>();
            string sql1 = "select ididioma, descricao from osler.idioma order by descricao";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows) {
                    while (res1.Read())
                    {
                        int coluna = res1.GetOrdinal("ididioma");
                        ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                        lista.Add(new RecIdTexto(id, res1["descricao"].ToString()));
                    }
                } 
                res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"IdiomaDao.GetList()", e);
            }

            return lista.ToArray();
        }
        
    }
}