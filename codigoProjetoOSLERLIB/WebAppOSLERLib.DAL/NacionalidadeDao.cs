/*
*	<description>NacionalidadeDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
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
    public class NacionalidadeDao: BaseDao
    {
        private Nacionalidade _currentobj;
        public NacionalidadeDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }

        public Nacionalidade CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        /// <summary>
        /// verifica se o objeto do tipo "Nacionalidade" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="nacionalidade"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(Nacionalidade nacionalidade, Utilizador utilizadorAtivo=null)
        {
            if (!nacionalidade.IsPersistent || nacionalidade.IsModified()) SaveObj(nacionalidade, utilizadorAtivo);
        }
        /// <summary>
        /// Ler uma nacionalidade da BD
        /// </summary>
        /// <param name="idNacionalidade"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public Nacionalidade GetById(ulong idNacionalidade)
        {
            Nacionalidade resultado = null;
            string sql1 = "select idnacionalidade, ididioma, descricao, criadopor, criadoem, modificadopor, modificadoem from osler.nacionalidade where idnacionalidade = @IdNacionalidade ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdNacionalidade", NpgsqlDbType.Bigint, sizeof(Int64), Convert.ToInt64(idNacionalidade));
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
                    coluna = res1.GetOrdinal("idnacionalidade");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    string descricao = res1["descricao"].ToString();
                    res1.Close();
                    // criar objeto
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    IdiomaDao iDao = new IdiomaDao(Db);
                    resultado = new Nacionalidade(id, descricao, 
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
                throw new MyException($"NacionalidadeDao.GetById({idNacionalidade})", e);
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
        public Nacionalidade GetByDescricao(string descricao)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("NacionalidadeDao.GetByDescricao()-> descricao tem que estar preenchida!");
            Nacionalidade resultado = null;
            string sql1 = "select idnacionalidade from osler.nacionalidade where descricao = @Descricao ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@Descricao", descricao.Trim());
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read()) {
                    int coluna = res1.GetOrdinal("idnacionalidade");
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
                throw new MyException($"NacionalidadeDao.GetByDescricao('{descricao}')", e);
            }
            // devolver objeto
            return resultado;
        }
        
        /// <summary>
        /// lê a Nacinalidade(1) "portuguesa" é a nacionalidade por omissão (cria o registo na BD caso não exista)
        /// </summary>
        /// <returns></returns>
        public Nacionalidade GetDefaultNacionalidade(Utilizador utilizadorAtivo=null)
        {
            Nacionalidade resultado = GetById(Nacionalidade.DefaultNacionalidadeId());
            if (resultado == null)
            {
                try
                {
                    IdiomaDao iDao = new IdiomaDao(Db);
                    resultado = new Nacionalidade(Nacionalidade.DefaultNacionalidadeId(), Nacionalidade.DefaultNacionalidadeDescricao(), iDao.GetDefaultIdioma(utilizadorAtivo), DateTime.Now, utilizadorAtivo, DateTime.Now, utilizadorAtivo);
                    SaveObj(resultado);
                }
                catch (Exception e)
                {
                    throw new MyException("NacionalidadeDao.GetDefaultNacionalidade()", e);
                }
            }
            return resultado;
        }
        
        /// <summary>
        /// criar um nova nacionalidade
        /// </summary>
        /// <param name="descricao"></param>
        /// <param name="idioma"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public Nacionalidade NewNacionalidade(string descricao, Idioma idioma, Utilizador utilizadorAtivo = null)
        {
            if (!(descricao.Trim().Length > 0)) return null;
            Nacionalidade resultado = GetByDescricao(descricao);
            if (resultado == null) {
                try
                {
                    resultado = new Nacionalidade(descricao, idioma, utilizadorAtivo);
                    SaveObj(resultado);
                }
                catch (Exception e)
                {
                    throw new MyException($"NacionalidadeDao.NewNacionalidade({descricao})", e);
                }
            }
            return resultado;
        }
        
        /// <summary>
        /// Gravar/Atualizar uma nacionalidade na BD
        /// </summary>
        /// <param name="nacionalidade"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public bool SaveObj(Nacionalidade nacionalidade, Utilizador utilizadorAtivo=null)
        {
            if (ReferenceEquals(nacionalidade, null)) 
                throw new MyException("NacionalidadeDao.SaveObj(null)->recebeu objeto vazio!");
            if (nacionalidade.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                Nacionalidade temp = GetById(nacionalidade.IdNacionalidade);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null) {
                    // INSERT INTO osler.nacionalidade(
                    //     idnacionalidade, ididioma, descricao, criadopor, criadoem, modificadopor, modificadoem)
                    // VALUES (?, ?, ?, ?, ?, ?, ?);
                    sqla = "INSERT INTO osler.nacionalidade(idnacionalidade, ididioma, descricao, criadoem, modificadoem";
                    sqlb = "VALUES (@IdNacionalidade, @IdIdioma, @Descricao, @CriadoEm, @ModificadoEm";
                    if (nacionalidade.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (nacionalidade.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    
                    nacionalidade.CriadoPor = utilizadorAtivo ?? nacionalidade.CriadoPor;
                } else {
                    // UPDATE osler.nacionalidade SET idnacionalidade=?, ididioma=?, descricao=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?
                    // WHERE <condition>;
                    sqla = "UPDATE osler.nacionalidade SET ididioma=@IdIdioma, descricao=@Descricao, criadoem=@CriadoEm, modificadoem=@ModificafoEm";
                    sqlb = " WHERE idnacionalidade=@IdNacionalidade ;";
                    if (nacionalidade.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (nacionalidade.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";
                }

                NpgsqlTransaction tr = null;
                try 
                {
                    if (nacionalidade.IdiomaNacionalidade != null) {
                        IdiomaDao iDao = new IdiomaDao(Db);
                        iDao.VerifyPersistent(nacionalidade.IdiomaNacionalidade, utilizadorAtivo);
                    }
                    nacionalidade.ModificadoPor = utilizadorAtivo ?? nacionalidade.ModificadoPor;
                    if (nacionalidade.CriadoPor != null || nacionalidade.ModificadoPor != null) {
                        UtilizadorDao uDao = new UtilizadorDao(Db);
                        if (nacionalidade.CriadoPor != null) 
                            uDao.VerifyPersistent(nacionalidade.CriadoPor, utilizadorAtivo);
                        if (nacionalidade.ModificadoPor != null) 
                            uDao.VerifyPersistent(nacionalidade.ModificadoPor, utilizadorAtivo);
                    }
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdNacionalidade", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(nacionalidade.IdNacionalidade));
                    com1.Parameters.AddWithValue("@IdIdioma", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(nacionalidade.IdiomaNacionalidade!.IdIdioma));
                    com1.Parameters.AddWithValue("@Descricao", nacionalidade.Descricao);
                    com1.Parameters.AddWithValue("@CriadoEm", nacionalidade.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", nacionalidade.ModificadoEm);
                    if (nacionalidade.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(nacionalidade.CriadoPor.IdUtilizador));
                    if (nacionalidade.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(nacionalidade.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    nacionalidade.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"NacionalidadeDao.SaveObj({nacionalidade.IdNacionalidade})", e);
                }
            }
            return false;
        }
        
        /// <summary>
        /// devolve uma lista(objeto json) de nacionalidades
        /// </summary>
        /// <returns></returns>
        public RecIdTexto[] GetList()
        {
            List<RecIdTexto> lista = new List<RecIdTexto>();
            string sql1 = "select idnacionalidade, descricao from osler.nacionalidade order by descricao";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows) {
                    while (res1.Read())
                    {
                        int coluna = res1.GetOrdinal("idnacionalidade");
                        ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                        lista.Add(new RecIdTexto(id, res1["descricao"].ToString()));
                    }
                } 
                res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"NacionalidadeDao.GetList()", e);
            }
            //return JsonConvert.SerializeObject(lista);
            return lista.ToArray();
        }
    }
}