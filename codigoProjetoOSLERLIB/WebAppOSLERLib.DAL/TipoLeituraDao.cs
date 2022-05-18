/*
*	<description>TipoLeituraDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
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
    public class TipoLeituraDao: BaseDao
    {
        private TipoLeitura _currentobj;
        public TipoLeituraDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }

        public TipoLeitura CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        
        /// <summary>
        /// verifica se o objeto do tipo "TipoLeitura" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="tipoLeitura"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(TipoLeitura tipoLeitura, Utilizador utilizadorAtivo=null)
        {
            if (!tipoLeitura.IsPersistent || tipoLeitura.IsModified()) SaveObj(tipoLeitura, utilizadorAtivo);
        }
        /// <summary>
        /// Ler um tipoLeitura da BD
        /// </summary>
        /// <param name="idTipoLeitura"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public TipoLeitura GetById(ulong idTipoLeitura)
        {
            TipoLeitura resultado = null;
            string sql1 = "select idtipoleitura, descricao, medida, criadopor, criadoem, modificadopor, modificadoem from osler.tipoleitura where idtipoleitura = @IdTipoLeitura ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdTipoLeitura", NpgsqlDbType.Bigint, sizeof(Int64), Convert.ToInt64(idTipoLeitura));
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
                    coluna = res1.GetOrdinal("idtipoleitura");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    string descricao = res1["descricao"].ToString();
                    string medida = res1["medida"].ToString();
                    res1.Close();
                    // criar objeto
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    resultado = new TipoLeitura(id, descricao, medida, 
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
                throw new MyException($"TipoLeituraDao.GetById({idTipoLeitura})", e);
            }
            // fornecer objeto
            return resultado;
        }
        
        /// <summary>
        /// Ler um tipoLeitura da BD, pesquisando pela descrição.
        /// </summary>
        /// <param name="descricao"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public TipoLeitura GetByDescricao(string descricao)
        {
            if (!(descricao.Trim().Length > 0)) throw new MyException("TipoLeituraDao.GetByDescricao()-> descricao tem que estar preenchida!");
            TipoLeitura resultado = null;
            string sql1 = "select idtipoleitura from osler.tipoleitura where descricao = @Descricao ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@Descricao", descricao.Trim());
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read()) {
                    int coluna = res1.GetOrdinal("idtipoleitura");
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
                throw new MyException($"TipoLeituraDao.GetByDescricao('{descricao}')", e);
            }
            // devolver objeto
            return resultado;
        }
        
        /// <summary>
        /// criar um novo tipoLeitura
        /// </summary>
        /// <param name="descricao"></param>
        /// <param name="medida"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public TipoLeitura NewTipoLeitura(string descricao, string medida, Utilizador utilizadorAtivo = null)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("TipoLeituraDao.NewTipoLeitura()-> descricao tem que estar preenchida!");
            if (!(medida.Trim().Length > 0)) 
                throw new MyException("TipoLeituraDao.NewTipoLeitura()-> medida tem que estar preenchida!");
            TipoLeitura resultado = GetByDescricao(descricao);
            if (resultado == null) {
                try
                {
                    resultado = new TipoLeitura(descricao.Trim(), medida.Trim(), utilizadorAtivo);
                    SaveObj(resultado);
                }
                catch (Exception e)
                {
                    throw new MyException($"TipoLeituraDao.NewTipoLeitura({descricao})", e);
                }
            }
            return resultado;
        }
        
        /// <summary>
        /// Gravar/Atualizar um tipoLeitura na BD
        /// </summary>
        /// <param name="tipoLeitura"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public bool SaveObj(TipoLeitura tipoLeitura, Utilizador utilizadorAtivo=null)
        {
            if (ReferenceEquals(tipoLeitura, null)) 
                throw new MyException("TipoLeituraDao.SaveObj(null)->recebeu objeto vazio!");
            if (tipoLeitura.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                TipoLeitura temp = GetById(tipoLeitura.IdTipoLeitura);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null)
                {
                    // INSERT INTO osler.local(
                    //     idlocal, descricao, criadopor, criadoem, modificadopor, modificadoem)
                    // VALUES (?, ?, ?, ?, ?, ?);
                    sqla = "INSERT INTO osler.tipoleitura(idtipoleitura, descricao, medida, criadoem, modificadoem";
                    sqlb = "VALUES (@IdTipoLeitura, @Descricao, @Medida, @CriadoEm, @ModificadoEm";
                    if (tipoLeitura.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (tipoLeitura.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";

                    tipoLeitura.CriadoPor = utilizadorAtivo ?? tipoLeitura.CriadoPor;
                } else { 
                    // UPDATE osler.local SET idlocal=?, descricao=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?
                    // WHERE <condition>;
                    sqla = "UPDATE osler.tipoleitura SET descricao=@Descricao, medida=@Medida, criadoem=@CriadoEm, modificadoem=@ModificadoEm";
                    sqlb = " WHERE idtipoleitura=@IdTipoLeitura ;";
                    if (tipoLeitura.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (tipoLeitura.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";

                }

                NpgsqlTransaction tr = null;
                try
                {
                    tipoLeitura.ModificadoPor = utilizadorAtivo ?? tipoLeitura.ModificadoPor;
                    if (tipoLeitura.CriadoPor != null || tipoLeitura.ModificadoPor != null) {
                        UtilizadorDao uDao = new UtilizadorDao(Db);
                        if (tipoLeitura.CriadoPor != null) 
                            uDao.VerifyPersistent(tipoLeitura.CriadoPor, utilizadorAtivo);
                        tipoLeitura.ModificadoPor = utilizadorAtivo;
                        if (tipoLeitura.ModificadoPor != null) 
                            uDao.VerifyPersistent(tipoLeitura.ModificadoPor, utilizadorAtivo);
                    }
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdTipoLeitura", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(tipoLeitura.IdTipoLeitura));
                    com1.Parameters.AddWithValue("@Descricao", tipoLeitura.Descricao);
                    com1.Parameters.AddWithValue("@Medida", tipoLeitura.Medida);
                    com1.Parameters.AddWithValue("@CriadoEm", tipoLeitura.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", tipoLeitura.ModificadoEm);
                    if (tipoLeitura.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(tipoLeitura.CriadoPor.IdUtilizador));
                    if (tipoLeitura.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(tipoLeitura.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    tipoLeitura.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"TipoLeituraDao.SaveObj({tipoLeitura.IdTipoLeitura})", e);
                }
            }
            return false;
        }
        
        /// <summary>
        /// devolve uma lista(objeto json) de tipoLeitura
        /// </summary>
        /// <returns></returns>
        public RecTipoLeitura[] GetList()
        {
            List<RecTipoLeitura> lista = new List<RecTipoLeitura>();
            string sql1 = "select idtipoleitura, descricao, medida from osler.tipoleitura order by descricao ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows) {
                    while (res1.Read())
                    {
                        int coluna = res1.GetOrdinal("idtipoleitura");
                        ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                        lista.Add(new RecTipoLeitura(id, res1["descricao"].ToString(), res1["medida"].ToString()));
                    }
                } 
                res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"LocalDao.GetList()", e);
            }
            
            //return JsonConvert.SerializeObject(lista);
            return lista.ToArray();
        }
        
    }
}