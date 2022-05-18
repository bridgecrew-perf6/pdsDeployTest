/*
*	<description>LoginRegistoDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
* 	<author>João Carlos Pinto</author>
*   <date>09-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using Npgsql;
using NpgsqlTypes;
using WebAppOSLERLib.BO;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.DAL
{
    public class LoginRegistoDao: BaseDao
    {
        private LoginRegisto _currentobj;
        public LoginRegistoDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }

        public LoginRegisto CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        /// <summary>
        /// verifica se o objeto do tipo "LoginRegisto" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="loginRegisto"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(LoginRegisto loginRegisto, Utilizador utilizadorAtivo=null)
        {
            if (!loginRegisto.IsPersistent || loginRegisto.IsModified()) SaveObj(loginRegisto, utilizadorAtivo);
        }
        /// <summary>
        /// Ler um LoginRegisto da BD 
        /// </summary>
        /// <param name="idLogin"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public LoginRegisto GetById(ulong idLogin)
        {
            LoginRegisto resultado = null;
            // SELECT idlogin, idutilizador, idepisodio, datahora, validadetoken, ativo, infodispositivo, criadopor,
            //        criadoem, modificadopor, modificadoem, tokenpayload
            // FROM osler.loginregisto;
            string sql1 = "select idlogin, idutilizador, idepisodio, datahora, validadetoken, ativo, infodispositivo, "+
                          "criadopor, criadoem, modificadopor, modificadoem, tokenpayload "+
                          "from osler.loginregisto where idlogin = @IdLogin ;";
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdLogin", NpgsqlDbType.Bigint, sizeof(Int64), 
                    Convert.ToInt64(idLogin));
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
                    coluna = res1.GetOrdinal("idutilizador");
                    ulong idutilizador = Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idepisodio");
                    ulong idepisodio = Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("datahora");
                    DateTime datahora = res1.GetDateTime(coluna);
                    coluna = res1.GetOrdinal("validadetoken");
                    DateTime validadetoken = res1.GetDateTime(coluna);
                    coluna = res1.GetOrdinal("ativo");
                    bool ativo = res1.GetBoolean(coluna);
                    coluna = res1.GetOrdinal("infodispositivo");
                    string infodispositivo = res1.GetString(coluna);
                    coluna = res1.GetOrdinal("tokenpayload");
                    string tokenpayload = res1.GetString(coluna);
                    res1.Close();
                    // criar objeto
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    EpisodioDao eDao = new EpisodioDao(Db);
                    resultado = new LoginRegisto(idLogin, 
                        uDao.GetById(idutilizador), 
                        eDao.GetById(idepisodio), 
                        datahora, tokenpayload, ativo, 
                        validadetoken, infodispositivo, criadoEm, 
                        criadoPorNull ? null : uDao.GetById(criadoPorId), 
                        modificadoEm, 
                        modificadoPorNull ? null : uDao.GetById(modificadoPorId));
                    // marcar objeto acabado de carregar
                    resultado.DataCheckpointDb();
                }
                if (!res1.IsClosed) res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"LoginRegistoDao.GetById({idLogin})", e);
            }
            // fornecer objeto
            return resultado;
        }
        /// <summary>
        /// criar um novo LoginRegisto
        /// </summary>
        /// <param name="utilizador"></param>
        /// <param name="episodio"></param>
        /// <param name="dataHora"></param>
        /// <param name="tokenPayload"></param>
        /// <param name="ativo"></param>
        /// <param name="validadeToken"></param>
        /// <param name="infoDispositivo"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public LoginRegisto NewLoginRegisto(Utilizador utilizador, Episodio episodio, DateTime dataHora, string tokenPayload,
            bool ativo, DateTime validadeToken, string infoDispositivo, Utilizador utilizadorAtivo=null)
        {
            if (ReferenceEquals(utilizador, null)) 
                throw new MyException("LoginRegistoDao.NewLoginRegisto(utilizador:null)->recebeu objeto Utilizador vazio!");
            if (ReferenceEquals(episodio, null)) 
                throw new MyException("LoginRegistoDao.NewLoginRegisto(episodio:null)->recebeu objeto Episodio vazio!");
            if (!(tokenPayload.Trim().Length > 0)) 
                throw new MyException("LoginRegistoDao.NewLoginRegisto()-> tokenpayload tem que estar preenchido!");
            LoginRegisto resultado;
            try {
                    resultado = new LoginRegisto( 
                        utilizador, episodio, dataHora, tokenPayload, ativo, 
                        validadeToken, infoDispositivo, utilizadorAtivo);
                    VerifyPersistent(resultado, utilizadorAtivo);
            } catch (Exception e) {
                throw new MyException($"LoginRegistoDao.NewLoginRegisto()", e);
            }
            // fornecer objeto
            return resultado;
        }
        /// <summary>
        /// Ler um LoginRegisto da BD, pesquisando por TokenPayload
        /// </summary>
        /// <param name="tokenPayload"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public LoginRegisto GetByTokenPayload(string tokenPayload)
        {
            if (!(tokenPayload.Trim().Length > 0)) 
                throw new MyException("LoginRegistoDao.GetByTokenPayload()-> TokenPayload tem que estar preenchido!");
            LoginRegisto resultado = null;
            string sql1 = "select idlogin from osler.loginregisto where tokenpayload = @TokenPayload ;";
            string[] partes = tokenPayload.Split(".");
            try {
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@TokenPayload", partes[1]);
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read()) {
                    int coluna = res1.GetOrdinal("idlogin");
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
                throw new MyException($"LoginRegistoDao.GetByTokenPayload('{tokenPayload}')", e);
            }
            // devolver objeto
            return resultado;
        }
        /// <summary>
        /// Gravar/Atualizar um LoginRegisto na BD
        /// </summary>
        /// <param name="loginRegisto"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool SaveObj(LoginRegisto loginRegisto, Utilizador utilizadorAtivo=null)
        {
            if (ReferenceEquals(loginRegisto, null)) 
                throw new MyException("LoginRegistoDao.SaveObj(null)->recebeu objeto vazio!");
            if (loginRegisto.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                LoginRegisto temp = GetById(loginRegisto.IdLogin);
                // testar se se deve inserir ou atualizar na BD
                if (temp == null)
                {
                    // INSERT INTO osler.loginregisto(
                    //     idlogin, idutilizador, idepisodio, datahora, validadetoken, ativo, infodispositivo, criadopor, criadoem, modificadopor, modificadoem, tokenpayload)
                    // VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
                    sqla = "INSERT INTO osler.loginregisto(idlogin, idutilizador, idepisodio, tokenpayload, datahora, validadetoken, ativo, infodispositivo, criadoem, modificadoem";
                    sqlb = "VALUES (@IdLogin, @IdUtilizador, @IdEpisodio, @TokenPayload, @DataHora, @ValidadeToken, @Ativo, @InfoDispositivo, @CriadoEm, @ModificadoEm";
                    if (loginRegisto.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (loginRegisto.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    
                    loginRegisto.CriadoPor = utilizadorAtivo ?? loginRegisto.CriadoPor;
                } else { 
                    // UPDATE osler.loginregisto
                    // SET idlogin=?, idutilizador=?, idepisodio=?, datahora=?, validadetoken=?, ativo=?, infodispositivo=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?, tokenpayload=?
                    // WHERE <condition>;
                    sqla = "UPDATE osler.loginregisto SET idutilizador=@IdUtilizador, idepisodio=@IdEpisodio, tokenpayload=@TokenPayload, datahora=@DataHora, validadetoken=@ValidadeToken, ativo=@Ativo, infodispositivo=@InfoDispositivo, criadoem=@CriadoEm, modificadoem=@ModificadoEm";
                    sqlb = " WHERE idlogin=@IdLogin ;";
                    if (loginRegisto.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (loginRegisto.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";

                }

                NpgsqlTransaction tr = null;
                try
                {
                    loginRegisto.ModificadoPor = utilizadorAtivo ?? loginRegisto.ModificadoPor;
                    EpisodioDao eDao = new EpisodioDao(Db);
                    eDao.VerifyPersistent(loginRegisto.Episodio);
                    UtilizadorDao uDao = new UtilizadorDao(Db);
                    uDao.VerifyPersistent(loginRegisto.Utilizador);
                    if (loginRegisto.CriadoPor != null) 
                        uDao.VerifyPersistent(loginRegisto.CriadoPor, utilizadorAtivo);
                    if (loginRegisto.ModificadoPor != null) 
                        uDao.VerifyPersistent(loginRegisto.ModificadoPor, utilizadorAtivo);
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdLogin", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(loginRegisto.IdLogin));
                    // @IdLogin, @IdUtilizador, @IdEpisodio, @TokenPayload, @DataHora, @ValidadeToken, @Ativo, @InfoDispositivo
                    com1.Parameters.AddWithValue("@IdUtilizador", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(loginRegisto.Utilizador.IdUtilizador));
                    com1.Parameters.AddWithValue("@IdEpisodio", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(loginRegisto.Episodio.IdEpisodio));
                    com1.Parameters.AddWithValue("@TokenPayload", loginRegisto.TokenPayload);
                    com1.Parameters.AddWithValue("@InfoDispositivo", loginRegisto.InfoDispositivo);
                    com1.Parameters.AddWithValue("@Ativo", loginRegisto.Ativo);
                    com1.Parameters.AddWithValue("@DataHora", loginRegisto.DataHora);
                    com1.Parameters.AddWithValue("@ValidadeToken", loginRegisto.ValidadeToken);
                    com1.Parameters.AddWithValue("@CriadoEm", loginRegisto.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", loginRegisto.ModificadoEm);
                    if (loginRegisto.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(loginRegisto.CriadoPor.IdUtilizador));
                    if (loginRegisto.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(loginRegisto.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    loginRegisto.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"LoginRegistoDao.SaveObj({loginRegisto.IdLogin})", e);
                }
            }
            return false;
        }
    }
}