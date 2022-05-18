/*
*	<description>UtilizadorDao, objeto da camada "DAL" responsável por todas as operações de acesso à BD</description>
* 	<author>João Carlos Pinto</author>
*   <date>09-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using WebAppOSLERLib.BO;
using WebAppOSLERLib.Consts;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.DAL
{
    struct UserLoggedIn
    {
        public ulong Id;
        public Utilizador UtilizadorObj;
        public string Token;
        public string TokenPayload;
    }
    
    public class UtilizadorDao: BaseDao
    {
        private Utilizador _currentobj;
        private UserLoggedIn _userLoggedIn;
        public UtilizadorDao(NpgsqlConnection db=null):base(db)
        {
            _currentobj = null;
        }

        public Utilizador UtilizadorLoggedIn
        {
            get => _userLoggedIn.UtilizadorObj;
            set => UtilizadorLoggedInNew(value);
        }
        public ulong UtilizadorLoggedInId
        {
            get => _userLoggedIn.Id;
            set => _userLoggedIn.Id = value;
        }
        public string UtilizadorLoggedInToken
        {
            get => _userLoggedIn.Token;
            set => UtilizadorLoggedInTokenNew(value);
        }
        private void UtilizadorLoggedInNew(Utilizador novo)
        {
            _userLoggedIn.UtilizadorObj = novo;
            _userLoggedIn.Id = novo.IdUtilizador;
        }
        private void UtilizadorLoggedInTokenNew(string newToken)
        {
            _userLoggedIn.Token = newToken;
            string[] partes = newToken.Split(".");
            // TODO: ainda falta analisar bem esta funcionalidade...
            // por definição o payload é "registo do meio"
            UtilizadorLoggedInTokenPayload = partes.Length==3 ? partes[1] : newToken;
        }
        public string UtilizadorLoggedInTokenPayload
        {
            get => _userLoggedIn.TokenPayload;
            set => _userLoggedIn.TokenPayload = value;
        }
        public Utilizador CurrentObj
        {
            get => _currentobj;
            set => _currentobj = value;
        }
        
        /// <summary>
        /// criar um novo utilizador
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="password"></param>
        /// <param name="nivelacesso"></param>
        /// <param name="idioma"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public Utilizador NewUtilizador(string nome, string password, int nivelacesso, 
            Idioma idioma=null, Utilizador utilizadorAtivo=null)
        {
            try
            {
                Utilizador uTemp=GetByNome(nome);
                if (uTemp==null)
                {
                    IdiomaDao idaoTemp = new IdiomaDao();
                    Idioma iTemp = idioma==null ? idaoTemp.GetDefaultIdioma() : idioma;
                    uTemp = new Utilizador(nome, password, nivelacesso, iTemp, utilizadorAtivo);
                    SaveObj(uTemp);
                }
                return uTemp;
            }
            catch (Exception e)
            {
                throw new MyException($"NewUtilizador('{nome}')", e);
            }
        }
        /// <summary>
        /// criar um novo utilizador by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nome"></param>
        /// <param name="password"></param>
        /// <param name="nivelacesso"></param>
        /// <param name="idioma"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public Utilizador NewUtilizadorById(ulong id, string nome, string password, int nivelacesso, 
            Idioma idioma=null, Utilizador utilizadorAtivo=null)
        {
            try
            {
                Utilizador uTemp=GetById(id);
                if (uTemp==null)
                {
                    IdiomaDao idaoTemp = new IdiomaDao();
                    Idioma iTemp = idioma==null ? idaoTemp.GetDefaultIdioma() : idioma;
                    uTemp = new Utilizador(id,nome, password, iTemp, nivelacesso, true, DateTime.Now, utilizadorAtivo, DateTime.Now, utilizadorAtivo);
                    SaveObj(uTemp);
                }
                return uTemp;
            }
            catch (Exception e)
            {
                throw new MyException($"NewUtilizadorById('{nome}')", e);
            }
        }
        /// <summary>
        /// Ler um utilizador da BD pesquisado pelo ID
        /// </summary>
        /// <param name="idUtilizador"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public Utilizador GetById(ulong idUtilizador)
        {
            Utilizador resultado = null;
            try {
                DbOpen();
                string sql1 = "select idutilizador, nome, password, nivelacesso, ativo, ididioma, criadoem, modificadoem, criadopor, modificadopor from osler.utilizador where idutilizador = @IdUtilizador;";
                NpgsqlCommand qry1 = new NpgsqlCommand(sql1, Db);
                qry1.Parameters.AddWithValue("@IdUtilizador", NpgsqlDbType.Bigint, sizeof(Int64), Convert.ToInt64(idUtilizador));
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read()) {
                    int coluna = res1.GetOrdinal("criadoem");
                    DateTime? criadoEm = res1.IsDBNull(coluna) ? null : res1.GetDateTime(coluna);
                    coluna = res1.GetOrdinal("modificadoem");
                    DateTime? modificadoEm = res1.IsDBNull(coluna) ? null : res1.GetDateTime(coluna);
                    coluna = res1.GetOrdinal("criadopor");
                    bool criadoPorNull = res1.IsDBNull(coluna);
                    ulong criadoPorId = criadoPorNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("modificadopor");
                    bool modificadoPorNull = res1.IsDBNull(coluna);
                    ulong modificadoPorId = modificadoPorNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("idutilizador");
                    ulong id = Convert.ToUInt64(res1.GetValue(coluna));
                    coluna = res1.GetOrdinal("nome");
                    string nome = res1.GetString(coluna);
                    coluna = res1.GetOrdinal("password");
                    string password = res1.GetString(coluna);
                    coluna = res1.GetOrdinal("nivelacesso");
                    int nivelacesso = res1.GetInt32(coluna);
                    coluna = res1.GetOrdinal("ativo");
                    bool ativo = res1.GetBoolean(coluna);
                    coluna = res1.GetOrdinal("ididioma");
                    bool idiomaNull = res1.IsDBNull(coluna);
                    ulong idiomaId = idiomaNull ? 0 : Convert.ToUInt64(res1.GetValue(coluna));
                    // criar objeto
                    res1.Close();
                    IdiomaDao iDao = new IdiomaDao(Db);
                    resultado = new Utilizador(id, nome, password, 
                        idiomaNull ? null : iDao.GetById(idiomaId), 
                        nivelacesso, ativo, 
                        criadoEm, criadoPorNull ? null : GetById(criadoPorId), 
                        modificadoEm, modificadoPorNull ? null : GetById(modificadoPorId));
                    // marcar objeto acabado de carregar
                    resultado.DataCheckpointDb();
                }
                if (!res1.IsClosed) res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"UtilizadorDao.GetById({idUtilizador})", e);
            }
            // fornecer objeto
            return resultado;
        }
        /// <summary>
        /// Ler um utilizador da BD pesquisado pelo Nome
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public Utilizador GetByNome(string nome)
        {
            Utilizador resultado = null;
            try 
            { 
                DbOpen();
                NpgsqlCommand qry1 = new NpgsqlCommand("select idutilizador from osler.utilizador where nome = @Nome and ativo = true;", Db);
                qry1.Parameters.AddWithValue("@Nome", nome);
                NpgsqlDataReader res1 = qry1.ExecuteReader();
                if (res1.HasRows && res1.Read()) {
                    if (!res1.IsDBNull("idutilizador"))
                    {
                        ulong tempId = Convert.ToUInt64(res1.GetValue("idutilizador"));
                        res1.Close();
                        resultado = GetById(tempId);
                    } else 
                        res1.Close();
                } else 
                    res1.Close();
                res1.Dispose();
                qry1.Dispose();
                DbClose();
            } catch (Exception e) {
                throw new MyException($"UtilizadorDao.GetByNome('{nome}')", e);
            }
            return resultado;
        }
        /// <summary>
        /// verifica se o objeto do tipo "Utilizador" tem alterações pendentes e grava na BD
        /// </summary>
        /// <param name="utilizador"></param>
        /// <param name="utilizadorAtivo"></param>
        public void VerifyPersistent(Utilizador utilizador, Utilizador utilizadorAtivo=null)
        {
            if (!utilizador.IsPersistent || utilizador.IsModified()) SaveObj(utilizador, utilizadorAtivo);
        }
        /// <summary>
        /// Gravar/Atualizar um utilizador na BD
        /// </summary>
        /// <param name="utilizador"></param>
        /// <param name="utilizadorAtivo"></param>
        /// <returns></returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public bool SaveObj(Utilizador utilizador, Utilizador utilizadorAtivo=null)
        {
            if (ReferenceEquals(utilizador, null)) throw new MyException("UtilizadorDao.SaveObj(null)->recebeu objeto vazio!");
            if (utilizador.IsModified())
            {
                string sqla, sqlb;
                // verificar se já existe na BD
                //Utilizador temp = LoadUtilizador(utilizador.IdUtilizador);
                Utilizador tempUtilizador = GetById(utilizador.IdUtilizador);
                // testar se se deve inserir ou atualizar na BD
                if (tempUtilizador == null)
                {
                    // INSERT INTO osler.utilizador(
                    //     idutilizador, ididioma, nome, password, nivelacesso, ativo, criadopor, criadoem, modificadopor, modificadoem)
                    // VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
                    sqla = "INSERT INTO osler.utilizador(idutilizador, ididioma, nome, password, nivelacesso, ativo, criadoem, modificadoem";
                    sqlb = "VALUES (@IdUtilizador, @IdIdioma, @Nome, @Password, @NivelAcesso, @Ativo, @CriadoEm, @ModificadoEm";
                    if (utilizador.CriadoPor != null) {
                        sqla += ", criadopor";
                        sqlb += ", @CriadoPor";
                    }
                    if (utilizador.ModificadoPor != null) {
                        sqla += ", modificadopor";
                        sqlb += ", @ModificadoPor";
                    }
                    // terminar ambas as partes do SQL
                    sqla += ") ";
                    sqlb += ");";
                    utilizador.CriadoPor = utilizadorAtivo ?? utilizador.CriadoPor;
                }
                else
                {
                    // UPDATE osler.utilizador SET idutilizador=?, ididioma=?, nome=?, password=?, nivelacesso=?, ativo=?, criadopor=?, criadoem=?, modificadopor=?, modificadoem=?
                    // WHERE <condition>;                    
                    sqla = "UPDATE osler.utilizador SET ididioma=@IdIdioma, nome=@Nome, password=@Password, nivelacesso=@NivelAcesso, ativo=@Ativo, criadoem=@CriadoEm, modificadoem=@ModificadoEm";
                    sqlb = " WHERE idutilizador=@IdUtilizador ;";
                    if (utilizador.CriadoPor != null) sqla += ", criadopor=@CriadoPor";
                    if (utilizador.ModificadoPor != null) sqla += ", modificadopor=@ModificadoPor";
                }

                NpgsqlTransaction tr = null;
                try
                {
                    IdiomaDao iDao = new IdiomaDao(Db);
                    if (utilizador.Idioma == null) 
                        utilizador.Idioma = iDao.GetDefaultIdioma(utilizadorAtivo);
                    if (utilizador.Idioma != null) 
                        iDao.VerifyPersistent(utilizador.Idioma, utilizadorAtivo);
                    if (utilizador.CriadoPor != null) 
                        VerifyPersistent(utilizador.CriadoPor, utilizadorAtivo);
                    utilizador.ModificadoPor = utilizadorAtivo ?? utilizador.ModificadoPor;
                    if (utilizador.ModificadoPor != null) 
                        VerifyPersistent(utilizador.ModificadoPor, utilizadorAtivo);
                    DbOpen();
                    tr = Db.BeginTransaction();
                    NpgsqlCommand com1 = new NpgsqlCommand(sqla+sqlb, Db);
                    com1.Parameters.AddWithValue("@IdUtilizador", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(utilizador.IdUtilizador));
                    com1.Parameters.AddWithValue("@IdIdioma", NpgsqlDbType.Bigint, sizeof(Int64), 
                        Convert.ToInt64(utilizador.Idioma!.IdIdioma));
                    com1.Parameters.AddWithValue("@Nome", utilizador.Nome);
                    com1.Parameters.AddWithValue("@Password", utilizador.Password);
                    com1.Parameters.AddWithValue("@NivelAcesso", utilizador.NivelAcesso);
                    com1.Parameters.AddWithValue("@Ativo", utilizador.Ativo);
                    com1.Parameters.AddWithValue("@CriadoEm", utilizador.CriadoEm);
                    com1.Parameters.AddWithValue("@ModificadoEm", utilizador.ModificadoEm);
                    if (utilizador.CriadoPor != null) 
                        com1.Parameters.AddWithValue("@CriadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(utilizador.CriadoPor.IdUtilizador));
                    if (utilizador.ModificadoPor != null) 
                        com1.Parameters.AddWithValue("@ModificadoPor", NpgsqlDbType.Bigint, sizeof(Int64), 
                            Convert.ToInt64(utilizador.ModificadoPor.IdUtilizador));
                    com1.ExecuteNonQuery();
                    tr.Commit();
                    tr.Dispose();
                    tr = null;
                    com1.Dispose();
                    utilizador.DataCheckpointDb();
                    DbClose();
                    return true;
                } catch (Exception e) {
                    if (tr!=null)
                    {
                        tr.Rollback();
                        tr.Dispose();
                    }
                    DbClose();
                    throw new MyException($"UtilizadorDao.SaveObj({utilizador.IdUtilizador})", e);
                }
            }
            return false;
        }
        /// <summary>
        /// verificar login no sistema, devolve login com sucesso juntamente com o ID de utilizador...
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="password"></param>
        /// <param name="uId"></param>
        /// <returns>bool</returns>
        /// <exception cref="MyException">produz uma exceção caso haja algum erro no comando SQL</exception>
        public bool Login(string nome, string password, ref ulong uId)
        {
            try
            {
                Utilizador tempUtilizador = GetByNome(nome);
                if (tempUtilizador != null) 
                {
                    // TODO: falta completar a funcionalidade de encriptação da password
                    if (String.Compare(password, tempUtilizador.Password, StringComparison.Ordinal) == 0)
                    {
                        uId = tempUtilizador.IdUtilizador;
                        return true;
                    }
                }
            } catch (Exception e) {
                throw new MyException($"UtilizadorDao.Login('{nome}')", e);
            }
            uId = 0;
            return false;
        }
        /// <summary>
        /// fornece a string para preencher a audiência do token
        /// </summary>
        /// <param name="nivelacesso"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public string TokenAudience(int nivelacesso)
        {
            if (nivelacesso is >= 0 and < Constantes.LibUtilizadorNivelAcesso)
            {
                return Constantes.DefaultNiveisAcessoText[nivelacesso];
            }
            else
            {
                throw new MyException($"UtilizadorDao.TokenAudience({nivelacesso}):Nivel de acesso desconhecido!");
            }
        }
        /// <summary>
        /// devolve uma lista(objeto json) de níveis de acesso
        /// </summary>
        /// <returns></returns>
        public RecIdTexto[] GetNivelAcessoList()
        {
            List<RecIdTexto> lista = new List<RecIdTexto>();
            for (int i = 0; i < Constantes.LibUtilizadorNivelAcesso; i++)
                lista.Add(new RecIdTexto((ulong)i, Constantes.DefaultNiveisAcessoText[i]));
            
            //return JsonConvert.SerializeObject(lista);
            return lista.ToArray();
        }
 
    }
}
