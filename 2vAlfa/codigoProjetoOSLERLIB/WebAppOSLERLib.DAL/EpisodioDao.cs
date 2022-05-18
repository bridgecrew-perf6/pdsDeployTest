/*
*	<description>EpisodioDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
* 	<author>João Carlos Pinto</author>
*   <date>09-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;
using WebAppOSLERLib.BO;
using WebAppOSLERLib.Consts;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.DAL
{
    public class EpisodioDao: BaseDao
    {
        private Episodio _currentobj;
        public EpisodioDao(NpgsqlConnection db=null): base(db)
        {
            _currentobj = null;
        }

        public Episodio CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        /// <summary>
        /// verifica se o objeto do tipo "Episodio" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="episodio"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(Episodio episodio, Utilizador utilizadorAtivo=null)
        {
            if (!episodio.IsPersistent || episodio.IsModified()) SaveObj(episodio, utilizadorAtivo);
        }
        /// <summary>
        /// Ler um episodio da BD
        /// </summary>
        /// <param name="idEpisodio"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public Episodio GetById(ulong idEpisodio)
        {
            Episodio resultado = null;
            // SELECT idepisodio, idcortriagem, idnacionalidade, ididioma, idsns, estado, dataaberto, datafechado, criadopor, criadoem, modificadopor, modificadoem, descricao, codepisodiotxt, dtnascimento, pin4, estadotxt
            // FROM osler.episodio;
            string sql1 = "select idepisodio, descricao, idcortriagem, idnacionalidade, ididioma, idsns, estado, "+
                          "dataaberto, datafechado, codepisodiotxt, dtnascimento, estadotxt, criadopor, criadoem, "+
                          "modificadopor, modificadoem from osler.episodio where idepisodio = @idEpisodio ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@idEpisodio", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idEpisodio));
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
                    bool idCorTriagemNull = res1.IsDBNull(coluna);
                    ulong idCorTriagemId = idCorTriagemNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idnacionalidade");
                    bool idNacionalidadeNull = res1.IsDBNull(coluna);
                    ulong idNacionalidadeId = idNacionalidadeNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("ididioma");
                    bool idIdiomaNull = res1.IsDBNull(coluna);
                    ulong idIdiomaId = idIdiomaNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("dataaberto");
                    DateTime? dataAberto = res1.IsDBNull(coluna) ? null : res1.GetDateTime(coluna);
                    coluna = res1.GetOrdinal("datafechado");
                    DateTime? dataFechado = res1.IsDBNull(coluna) ? null : res1.GetDateTime(coluna);
                    string idSns = res1["idsns"].ToString();
                    string descricao = res1["descricao"].ToString();
                    // codepisodiotxt, dtnascimento, pin4, estadotxt
                    string codepisodiotxt = res1["codepisodiotxt"].ToString();
                    string estadotxt = res1["estadotxt"].ToString();
                    coluna = res1.GetOrdinal("dtnascimento");
                    DateTime dataNascimento = res1.GetDateTime(coluna);
                    coluna = res1.GetOrdinal("estado");
                    int estado = res1.GetInt32(coluna);
                    coluna = res1.GetOrdinal("idepisodio");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    res1.Close();
                    // criar objeto
                    CorTriagemDao cDao = new CorTriagemDao(Db);
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    IdiomaDao iDao = new IdiomaDao(Db);
                    NacionalidadeDao nDao = new NacionalidadeDao(Db);
                    resultado = new Episodio(id, codepisodiotxt, descricao, dataNascimento, idSns, 
                        estado, estadotxt, dataAberto, dataFechado, 
                        idCorTriagemNull ? null : cDao.GetById(idCorTriagemId),
                        idIdiomaNull ? null : iDao.GetById(idIdiomaId),
                        idNacionalidadeNull ? null : nDao.GetById(idNacionalidadeId),
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
                throw new MyException($"EpisodioDao.GetById({idEpisodio})", e);
            }
            // fornecer objeto
            return resultado;
        }
        /// <summary>
        /// Ler um episodio da BD usando o codEpisodio
        /// </summary>
        /// <param name="codepisodio"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public Episodio GetByCodEpisodio(string codepisodio)
        {
            Episodio resultado = null;
            string sql1 = "select idepisodio from osler.episodio where codepisodiotxt = @codEpisodio ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@codEpisodio", codepisodio);
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read()) {
                    int coluna = res1.GetOrdinal("idepisodio");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    res1.Close();
                    resultado = GetById(id);
                }
                if (!res1.IsClosed) res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"EpisodioDao.GetByCodEpisodio(\"{codepisodio}\")", e);
            }
            // fornecer objeto
            return resultado;
        }
        /// <summary>
        /// Verificar o login de um utente/acompanhante no sistema
        /// </summary>
        /// <param name="codEpisodio"></param>
        /// <param name="pin4"></param>
        /// <param name="eId">devolve o ID do episódio</param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool LoginUtente(string codEpisodio, short pin4, ref ulong eId)
        {
            try
            {
                Episodio temp = GetByCodEpisodio(codEpisodio);
                if (temp != null) 
                {
                    if (String.Compare(codEpisodio, temp.CodEpisodio, StringComparison.Ordinal) == 0 &&
                        pin4 == temp.Pin4 && temp.Estado == 0)
                    {
                        eId = temp.IdEpisodio;
                        return true;
                    }
                }
            } catch (Exception e) {
                throw new MyException($"EpisodioDao.LoginUtente('{codEpisodio}')", e);
            }
            eId = 0;
            return false;
        }
        /// <summary>
        /// Gravar/Atualizar um episodio na BD
        /// </summary>
        /// <param name="episodio"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool SaveObj(Episodio episodio, Utilizador utilizadorAtivo=null)
        {
            if (ReferenceEquals(episodio, null)) throw new MyException("EpisodioDao.SaveObj(null)->recebeu objeto vazio!");
            if (episodio.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                Episodio temp = GetById(episodio.IdEpisodio);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null)
                {
                    // INSERT INTO osler.episodio(
                    //     idepisodio, idcortriagem, idnacionalidade, ididioma, idsns, estado, dataaberto, datafechado,
                    //     criadopor, criadoem, modificadopor, modificadoem, descricao, codepisodiotxt, dtnascimento,
                    //     pin4, estadotxt)
                    // VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
                    sqla = "INSERT INTO osler.episodio(idepisodio, codepisodiotxt, descricao, idcortriagem, "+
                           "idnacionalidade, ididioma, dtnascimento, pin4, idsns, estado, estadotxt, criadoem, modificadoem";
                    sqlb = "VALUES (@IdEpisodio, @CodEpisodiotxt, @Descricao, @IdCorTriagem, @IdNacionalidade, @IdIdioma, "+
                           "@DtNascimento, @Pin4, @IdSns, @Estado, @EstadoTxt, @CriadoEm, @ModificadoEm";
                    if (episodio.DataAberto != null) {
                        sqla += ", dataaberto";
                        sqlb += ", @DataAberto";
                        
                    }
                    if (episodio.DataFechado != null) {
                        sqla += ", datafechado";
                        sqlb += ", @DataFechado";
                        
                    }
                    if (episodio.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (episodio.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";

                    episodio.CriadoPor = utilizadorAtivo ?? episodio.CriadoPor;
                } 
                else 
                { 
                    // UPDATE osler.episodio
                    // SET idepisodio=?, idcortriagem=?, idnacionalidade=?, ididioma=?, idsns=?, estado=?, dataaberto=?,
                    //     datafechado=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?, descricao=?,
                    //     codepisodiotxt=?, dtnascimento=?, pin4=?, estadotxt=?
                    // WHERE <condition>;
                    sqla = "UPDATE osler.episodio "+
                           "SET codepisodiotxt=@CodEpisodioTxt, dtnascimento=@DtNascimento, pin4=@Pin4, "+
                           "estadotxt=@EstadoTxt, descricao=@Descricao, idcortriagem=@IdCorTriagem, "+
                           "idnacionalidade=@IdNacionalidade, ididioma=@IdIdioma, idsns=@IdSns, estado=@Estado, "+
                           "criadoem=@CriadoEm, modificadoem=@ModificadoEm";
                    sqlb = " WHERE idepisodio=@IdEpisodio ;";
                    if (episodio.DataAberto != null) sqla += ", dataaberto=@DataAberto";
                    if (episodio.DataFechado != null) sqla += ", datafechado=@DataFechado";
                    if (episodio.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (episodio.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";
                }

                NpgsqlTransaction tr = null;
                try {
                    CorTriagemDao cDao = new CorTriagemDao(Db);
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    IdiomaDao iDao = new IdiomaDao(Db);
                    NacionalidadeDao nDao = new NacionalidadeDao(Db);
                    cDao.VerifyPersistent(episodio.CorTriagem, utilizadorAtivo);
                    iDao.VerifyPersistent(episodio.Idioma, utilizadorAtivo);
                    nDao.VerifyPersistent(episodio.Nacionalidade, utilizadorAtivo);
                    if (episodio.CriadoPor != null) uDao.VerifyPersistent(episodio.CriadoPor, utilizadorAtivo);
                    episodio.ModificadoPor = utilizadorAtivo ?? episodio.ModificadoPor;
                    if (episodio.ModificadoPor != null) uDao.VerifyPersistent(episodio.ModificadoPor, utilizadorAtivo);

                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdEpisodio", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodio.IdEpisodio));
                    com1.Parameters.AddWithValue("@IdCorTriagem", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodio.CorTriagem.IdCorTriagem));
                    com1.Parameters.AddWithValue("@IdNacionalidade", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodio.Nacionalidade.IdNacionalidade));
                    com1.Parameters.AddWithValue("@IdIdioma", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(episodio.Idioma.IdIdioma));
                    com1.Parameters.AddWithValue("@Descricao", episodio.Descricao);
                    com1.Parameters.AddWithValue("@CodEpisodioTxt", episodio.CodEpisodio);
                    com1.Parameters.AddWithValue("@IdSns", episodio.IdSns);
                    com1.Parameters.AddWithValue("@Estado", episodio.Estado);
                    com1.Parameters.AddWithValue("@EstadoTxt", episodio.EstadoTxt);
                    com1.Parameters.AddWithValue("@DtNascimento", episodio.DataNascimento);
                    com1.Parameters.AddWithValue("@Pin4", episodio.Pin4);
                    if (episodio.DataAberto != null) com1.Parameters.AddWithValue("@DataAberto", episodio.DataAberto);
                    if (episodio.DataFechado != null) com1.Parameters.AddWithValue("@DataFechado", episodio.DataFechado);
                    com1.Parameters.AddWithValue("@CriadoEm", episodio.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", episodio.ModificadoEm);
                    if (episodio.CriadoPor != null) com1.Parameters.AddWithValue("@CriadoPor", 
                        NpgsqlDbType.Bigint, sizeof(Int64), Convert.ToInt64(episodio.CriadoPor.IdUtilizador));
                    if (episodio.ModificadoPor != null) com1.Parameters.AddWithValue("@ModificadoPor", 
                        NpgsqlDbType.Bigint, sizeof(Int64), Convert.ToInt64(episodio.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    episodio.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"EpisodioDao.SaveObj({episodio.IdEpisodio})", e);
                }
            }
            return false;
        }
        /// <summary>
        /// criar um novo episodio na BD
        /// </summary>
        /// <param name="codEpisodio"></param>
        /// <param name="descricao"></param>
        /// <param name="dataNascimento"></param>
        /// <param name="idSns"></param>
        /// <param name="estado"></param>
        /// <param name="estadoTxt"></param>
        /// <param name="corTriagem"></param>
        /// <param name="idioma"></param>
        /// <param name="nacionalidade"></param>
        /// <param name="utilizadorAtual"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public Episodio NewEpisodio(string codEpisodio, string descricao, DateTime dataNascimento, string idSns, int estado, string estadoTxt, CorTriagem corTriagem, Idioma idioma, Nacionalidade nacionalidade, Utilizador utilizadorAtual)
        {
            if (!(codEpisodio.Length>0)) 
                throw new MyException("EpisodioDao.NewEpisodio()->codEpisodio deve estar preenchido!");
            if (ReferenceEquals(corTriagem, null))
                throw new MyException("EpisodioDao.NewEpisodio()->CorTriagem não pode ser null!");
            if (ReferenceEquals(idioma, null))
                throw new MyException("EpisodioDao.NewEpisodio()->Idioma não pode ser null!");
            if (ReferenceEquals(nacionalidade, null))
                throw new MyException("EpisodioDao.NewEpisodio()->Nacionalidade não pode ser null!");
            // TODO: decidir o que fazer caso já exista um episódio com o código recebido
            Episodio resultado = GetByCodEpisodio(codEpisodio);
            if (resultado==null)
            {
                try
                {
                    resultado = new Episodio(codEpisodio, descricao, dataNascimento, idSns, estado, estadoTxt, corTriagem,
                        idioma, nacionalidade, utilizadorAtual);
                    SaveObj(resultado, utilizadorAtual);
                }
                catch (Exception e)
                {
                    throw new MyException($"EpisodioDao.NewEpisodio(\"{codEpisodio}\")->create", e);
                }
            }
            else
            {
                try
                {
                    resultado.Descricao = descricao;
                    resultado.DataNascimento = dataNascimento;
                    resultado.IdSns = idSns;
                    resultado.Estado = estado;
                    resultado.EstadoTxt = estadoTxt;
                    resultado.CorTriagem = corTriagem;
                    resultado.Idioma = idioma;
                    resultado.Nacionalidade = nacionalidade;
                    VerifyPersistent(resultado, utilizadorAtual);
                }
                catch (Exception e)
                {
                    throw new MyException($"EpisodioDao.NewEpisodio(\"{codEpisodio}\")->update", e);
                }
            }            
            return resultado;
        }
        /// <summary>
        /// criar um episodio vazio porque é obrigatorio adicionar um episódio no registo de login
        /// </summary>
        /// <param name="utilizadorAtual"></param>
        /// <returns></returns>
        public Episodio CreateDummyEpisodio(Utilizador utilizadorAtual)
        {
            Episodio resultado = GetById(Episodio.DummyEpisodioId());
            if (ReferenceEquals(resultado, null))
            {
                NacionalidadeDao nDao = new NacionalidadeDao(Db);
                IdiomaDao iDao = new IdiomaDao(Db);
                CorTriagemDao cDao = new CorTriagemDao(Db);
                // TODO: é necessário verificar o estado deste DummyEpisodio
                resultado = new Episodio("0", "0", DateTime.Now, "0", 0, "0", 
                    cDao.GetById(5), 
                    iDao.GetDefaultIdioma(utilizadorAtual), 
                    nDao.GetDefaultNacionalidade(utilizadorAtual), 
                    utilizadorAtual);
                if (!resultado.OverrideIdEpisodioDummyId())
                {
                    throw new MyException("EpisodioDao.CreateDummyEpisodio()->OverrideIdEpisodioDummyId() ERRO!!");
                }
                SaveObj(resultado, utilizadorAtual);
            }
            return resultado;
        }
        /// <summary>
        /// Devolve uma lista de episódios ordenado por data descendente (i.e. a data mais recente em primeiro)
        /// </summary>
        /// <param name="idSns"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public List<RecEpisodio> GetListEpisodioByIdSns(string idSns)
        {
            List<RecEpisodio> resultado = new List<RecEpisodio>();
            // SELECT idepisodio, idcortriagem, idnacionalidade, ididioma, idsns, estado, dataaberto, datafechado, criadopor, criadoem, modificadopor, modificadoem, descricao, codepisodiotxt, dtnascimento, pin4, estadotxt
            // FROM osler.episodio;
            string sql1 = "SELECT idepisodio, idsns, dataaberto, descricao, codepisodiotxt "+
                          "FROM osler.episodio "+
                          "WHERE idsns = @IdSns "+
                          "ORDER BY dataaberto DESC ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdSns", idSns);
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows) {
                    while (res1.Read())
                    {
                        int coluna = res1.GetOrdinal("idepisodio");
                        ulong idEpisodio = Convert.ToUInt64(res1.GetValue(coluna));
                        coluna = res1.GetOrdinal("descricao");
                        string descricao = res1.GetString(coluna);
                        coluna = res1.GetOrdinal("codepisodiotxt");
                        string episodioTxt = res1.GetString(coluna);
                        coluna = res1.GetOrdinal("dataaberto");
                        DateTime data = res1.GetDateTime(coluna);
                        resultado.Add(new RecEpisodio(idEpisodio, episodioTxt, descricao, data, idSns));
                    }
                }
                if (!res1.IsClosed) res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"EpisodioDao.GetListEpisodioByIdSns(\"{idSns}\")", e);
            }
            // fornecer objeto
            return resultado;
        }
    }
}