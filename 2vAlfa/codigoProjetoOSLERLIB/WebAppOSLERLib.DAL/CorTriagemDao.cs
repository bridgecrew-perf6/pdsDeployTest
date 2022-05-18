/*
*	<description>CorTriagemDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
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
    public class CorTriagemDao: BaseDao
    {
        private CorTriagem _currentobj;
        public CorTriagemDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }

        public CorTriagem CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        /// <summary>
        /// verifica se o objeto do tipo "CorTriagem" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="cortriagem"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(CorTriagem cortriagem, Utilizador utilizadorAtivo=null)
        {
            if (!cortriagem.IsPersistent || cortriagem.IsModified()) SaveObj(cortriagem, utilizadorAtivo);
        }
        
        /// <summary>
        /// Ler uma cortriagem da BD
        /// </summary>
        /// <param name="idCorTriagem"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public CorTriagem GetById(ulong idCorTriagem)
        {
            CorTriagem resultado = null;
            string sql1 = "select idcortriagem, descricao, codigocorhex, criadopor, criadoem, modificadopor, modificadoem from osler.cortriagem where idcortriagem = @IdCorTriagem ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdCorTriagem", NpgsqlDbType.Bigint, sizeof(Int64), Convert.ToInt64(idCorTriagem));
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
                    coluna = res1.GetOrdinal("idcortriagem");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    string descricao = res1["descricao"].ToString();
                    string codigocorhex = res1["codigocorhex"].ToString();
                    res1.Close();
                    // criar objeto
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    resultado = new CorTriagem(id, descricao, codigocorhex, 
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
                throw new MyException($"CorTriagemDao.GetById({idCorTriagem})", e);
            }
            // fornecer objeto
            return resultado;
        }

        /// <summary>
        /// Gravar/Atualizar uma cortriagem na BD
        /// </summary>
        /// <param name="corTriagem"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool SaveObj(CorTriagem corTriagem, Utilizador utilizadorAtivo = null)
        {
            if (ReferenceEquals(corTriagem, null)) 
                throw new MyException("CorTriagemDao.SaveObj(null)->recebeu objeto vazio!");
            if (corTriagem.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                CorTriagem temp = GetById(corTriagem.IdCorTriagem);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null) {
                    // INSERT INTO osler.cortriagem(idcortriagem, descricao, codigocorhex, criadopor, criadoem,
                    //     modificadopor, modificadoem)
                    // VALUES (?, ?, ?, ?, ?, ?, ?);
                    sqla = "INSERT INTO osler.cortriagem(idcortriagem, descricao, codigocorhex, criadoem, modificadoem";
                    sqlb = "VALUES (@IdCorTriagem, @Descricao, @CodigoCorHex, @CriadoEm, @ModificadoEm";
                    if (corTriagem.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (corTriagem.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    
                    corTriagem.CriadoPor = utilizadorAtivo ?? corTriagem.CriadoPor;
                } else {
                    // UPDATE osler.cortriagem SET idcortriagem=?, descricao=?, codigocorhex=?, criadoem=?,
                    // modificadoem=?, modificadopor=?
                    // WHERE <condition>;
                    sqla = "UPDATE osler.cortriagem SET descricao=@Descricao, codigocorhex=@CodigoCorHex, "+
                           "criadoem=@CriadoEm, modificadoem=@ModificadoEm";
                    sqlb = " WHERE idcortriagem = @IdCorTriagem ;";
                    if (corTriagem.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (corTriagem.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";
                }

                NpgsqlTransaction tr = null;
                try
                {
                    corTriagem.ModificadoPor = utilizadorAtivo ?? corTriagem.ModificadoPor;
                    if (corTriagem.CriadoPor != null || corTriagem.ModificadoPor != null) {
                        UtilizadorDao uDao = new UtilizadorDao(Db);
                        if (corTriagem.CriadoPor != null) uDao.VerifyPersistent(corTriagem.CriadoPor, utilizadorAtivo);
                        if (corTriagem.ModificadoPor != null) uDao.VerifyPersistent(corTriagem.ModificadoPor, utilizadorAtivo);
                    }
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdCorTriagem", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(corTriagem.IdCorTriagem));
                    com1.Parameters.AddWithValue("@CodigoCorHex", corTriagem.CodigoCorHex);
                    com1.Parameters.AddWithValue("@Descricao", corTriagem.Descricao);
                    com1.Parameters.AddWithValue("@CriadoEm", corTriagem.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", corTriagem.ModificadoEm);
                    if (corTriagem.CriadoPor != null) com1.Parameters.AddWithValue("@CriadoPor", 
                        NpgsqlDbType.Bigint, sizeof(Int64), Convert.ToInt64(corTriagem.CriadoPor.IdUtilizador));
                    if (corTriagem.ModificadoPor != null) com1.Parameters.AddWithValue("@ModificadoPor", 
                        NpgsqlDbType.Bigint, sizeof(Int64), Convert.ToInt64(corTriagem.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    corTriagem.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"CorTriagemDao.SaveObj({corTriagem.IdCorTriagem})", e);
                }
            }
            return false;
        }

        /// <summary>
        /// criar um novo CodTriagem
        /// </summary>
        /// <param name="id"></param>
        /// <param name="descricao"></param>
        /// <param name="corhex"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public CorTriagem NewCorTriagem(ulong id, string descricao, string corhex, Utilizador utilizadorAtivo = null)
        {
            if (!(descricao.Trim().Length > 0)) 
                throw new MyException("CorTriagemDao.NewCorTriagem()-> descricao tem que estar preenchida!");
            if (!(corhex.Trim().Length > 0)) 
                throw new MyException("CorTriagemDao.NewCorTriagem()-> corhex tem que estar preenchida!");
            CorTriagem resultado = GetById(id);
            if (resultado == null) {
                try
                {
                    resultado = new CorTriagem(id, descricao, corhex, DateTime.Now, 
                        utilizadorAtivo, DateTime.Now, utilizadorAtivo);
                    SaveObj(resultado, utilizadorAtivo);
                }
                catch (Exception e)
                {
                    throw new MyException($"CorTriagemDao.NewCorTriagem({descricao})->create", e);
                }
            }
            else {
                try
                {
                    resultado.Descricao = descricao;
                    resultado.CodigoCorHex = corhex;
                    VerifyPersistent(resultado, utilizadorAtivo);
                }
                catch (Exception e)
                {
                    throw new MyException($"CorTriagemDao.NewCorTriagem({descricao})->update", e);
                }
            }
            return resultado;
        }
        
        /// <summary>
        /// devolve uma lista(objeto json) de corTriagem
        /// </summary>
        /// <returns></returns>
        public RecIdDescCor[] GetList()
        {
            List<RecIdDescCor> lista = new List<RecIdDescCor>();
            string sql1 = "select idcortriagem, descricao, codigocorhex from osler.cortriagem order by descricao";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows) {
                    while (res1.Read())
                    {
                        int coluna = res1.GetOrdinal("idcortriagem");
                        ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                        lista.Add(new RecIdDescCor(id, 
                            res1["descricao"].ToString(), 
                            res1["codigocorhex"].ToString()));
                    }
                } 
                res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"CorTriagemDao.GetList()", e);
            }
            //return JsonConvert.SerializeObject(lista);
            return lista.ToArray();
        }
        
    }
}