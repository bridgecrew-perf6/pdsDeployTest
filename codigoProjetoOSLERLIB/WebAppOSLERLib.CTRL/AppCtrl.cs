/*
*	<description>AppCtrl.cs, objeto da camada "CTRL" é acessivel a partir do API</description>
* 	<author>João Carlos Pinto</author>
*   <date>22-03-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebAppOSLERLib.BO;
using WebAppOSLERLib.Consts;
using WebAppOSLERLib.DAL;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.CTRL
{
    /// <summary>
    /// classe de controlo principal do projeto
    /// </summary>
    public class AppCtrl
    {
        #region interno
        /// <summary>
        /// method: thread-safe without using locks (Using .NET 4's Lazy<T> type)
        /// </summary>
        private readonly Lazy<DataConfig> _dataconfig = 
            new Lazy<DataConfig>(() => DataConfig.Instancia);
        private readonly Lazy<QuestionarioDao> _questionarios =
            new Lazy<QuestionarioDao>(() => new QuestionarioDao());
        private readonly Lazy<IdiomaDao> _idiomas = 
            new Lazy<IdiomaDao>(() => new IdiomaDao());
        private readonly Lazy<LocalDao> _locais = 
            new Lazy<LocalDao>(() => new LocalDao());
        private readonly Lazy<NacionalidadeDao> _nacionalidades =
            new Lazy<NacionalidadeDao>(() => new NacionalidadeDao());
        private readonly Lazy<CorTriagemDao> _corestriagem = 
            new Lazy<CorTriagemDao>(() => new CorTriagemDao());
        private readonly Lazy<UtilizadorDao> _utilizadores = 
            new Lazy<UtilizadorDao>(() => new UtilizadorDao());
        private readonly Lazy<TipoLeituraDao> _tiposleitura = 
            new Lazy<TipoLeituraDao>(() => new TipoLeituraDao());
        private readonly Lazy<EpisodioDao> _episodios = 
            new Lazy<EpisodioDao>(() => new EpisodioDao());
        private readonly Lazy<ItemFluxoManchesterDao> _itensfluxomanchester =
            new Lazy<ItemFluxoManchesterDao>(() => new ItemFluxoManchesterDao());
        private readonly Lazy<EpisodioHistLocalDao> _episodioHistLocalDao =
            new Lazy<EpisodioHistLocalDao>(() => new EpisodioHistLocalDao());
        private readonly Lazy<EpisodioRegistoDadosDao> _episodioRegistoDadosDao =
            new Lazy<EpisodioRegistoDadosDao>(() => new EpisodioRegistoDadosDao());
        private readonly Lazy<EpisodioQuestionarioDao> _episodioQuestionarioDao =
            new Lazy<EpisodioQuestionarioDao>(() => new EpisodioQuestionarioDao());
        private readonly Lazy<EpisodioQuestRespostaDao> _episodioQuestRespostaDao =
            new Lazy<EpisodioQuestRespostaDao>(() => new EpisodioQuestRespostaDao());
        private readonly Lazy<PerguntaDao> _perguntaDao = 
            new Lazy<PerguntaDao>(() => new PerguntaDao());
        private readonly Lazy<LoginRegistoDao> _registoLoginDao = 
            new Lazy<LoginRegistoDao>(() => new LoginRegistoDao());
        public AppCtrl() { }
        #endregion

        #region core_LIB_attributes
        public string DbConnection => _dataconfig.Value.DbConnectString;
        public string JwtSecret => _dataconfig.Value.JwtSecret;
        public string CORSlist => _dataconfig.Value.CORSList;
        public IdiomaDao Idiomas => _idiomas.Value;
        public LocalDao Locais => _locais.Value;
        public NacionalidadeDao Nacionalidades => _nacionalidades.Value;
        public CorTriagemDao CoresTriagem => _corestriagem.Value;
        public UtilizadorDao Utilizadores => _utilizadores.Value;
        public TipoLeituraDao TiposLeitura => _tiposleitura.Value;
        public EpisodioDao Episodios => _episodios.Value;
        public ItemFluxoManchesterDao ItensFluxoManchester => _itensfluxomanchester.Value;
        public EpisodioHistLocalDao EpisodioHistLocal => _episodioHistLocalDao.Value;
        public EpisodioRegistoDadosDao EpisodioRegistoDados => _episodioRegistoDadosDao.Value;
        public EpisodioQuestionarioDao EpisodioQuestionario => _episodioQuestionarioDao.Value;
        public QuestionarioDao Questionarios => _questionarios.Value;
        public EpisodioQuestRespostaDao EpisodioQuestResposta => _episodioQuestRespostaDao.Value;
        public PerguntaDao Perguntas => _perguntaDao.Value;
        public LoginRegistoDao RegistosLogin => _registoLoginDao.Value;
        #endregion

        #region core_LIB_methods
        private JwtSecurityToken GenerateToken(string role)
        {
            //string key = "superKeyToBeConnected";
            string key = JwtSecret;
            
            //symetric security key
            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            //signing credentials
            var signingCredentials = new SigningCredentials(symetricSecurityKey, 
                SecurityAlgorithms.HmacSha256Signature);
            
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, role));
            
            return new JwtSecurityToken(
                issuer: "sysadmin",
                audience: role,
                expires: DateTime.Now.AddMinutes(Constantes.MinutosValidadeToken),
                signingCredentials: signingCredentials,
                claims: claims
            );

        }
        
        /// <summary>
        /// Login "normal"
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool Login(string nome, string password)
        {
            try
            {
                ulong id = 0;
                bool resultado = Utilizadores.Login(nome, password, ref id);
                if (resultado)
                {
                    Utilizadores.UtilizadorLoggedInId = id;
                    Utilizadores.UtilizadorLoggedIn = Utilizadores.GetById(id);
                    JwtSecurityToken token = GenerateToken(
                        Constantes.DefaultNiveisAcessoText[Utilizadores.UtilizadorLoggedIn.NivelAcesso]);
                    string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                    Utilizadores.UtilizadorLoggedInToken = tokenString;
                    // a verificação do histórico de login é feita no login controller
                }
                return resultado;
            }
            catch (Exception e)
            {
                throw new MyException($"AppCtrl.Login({nome})", e);
            }
        }
        
        /// <summary>
        /// Login "utente/acompanhante"
        /// </summary>
        /// <param name="codEpisodio"></param>
        /// <param name="pin4"></param>
        /// <param name="utente">true=utente, false=acompanhante</param>
        /// <param name="eId">devolve o ID do episódio associado</param>
        /// <returns></returns>
        /// <exception cref="MyException"></exception>
        public bool LoginUtente(string codEpisodio, short pin4, bool utente, ref ulong eId)
        {
            try
            {
                eId = 0;
                bool resultado = Episodios.LoginUtente(codEpisodio, pin4, ref eId);
                if (resultado)
                {
                    Episodios.CurrentObj = Episodios.GetById(eId);
                    Utilizadores.UtilizadorLoggedIn = Utilizadores.GetById((ulong)(utente ? 0 : 1));
                    JwtSecurityToken token = GenerateToken(
                        Constantes.DefaultNiveisAcessoText[Utilizadores.UtilizadorLoggedIn.NivelAcesso]);
                    string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                    Utilizadores.UtilizadorLoggedInToken = tokenString;
                    // a verificação do histórico de login é feita no login controller
                }
                return resultado;
            }
            catch (Exception e)
            {
                throw new MyException($"AppCtrl.LoginUtente({codEpisodio})", e);
            }
        }

        public RecAuthorization DecodeToken(string token1)
        {
            RecAuthorization tk = new RecAuthorization();
            string tkString = token1 ?? Utilizadores.UtilizadorLoggedInToken;
            if (tkString.StartsWith(Constantes.CTKAuth))
            {
                JwtSecurityToken token = new JwtSecurityToken(tkString.Substring(Constantes.CTKAuth.Length));
                Dictionary<string, string> claimVals = token.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currTime = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                tk.Role = claimVals[ClaimTypes.Role];
                tk.Expired = (token.Payload.Exp < currTime);
                RegistosLogin.CurrentObj = RegistosLogin.GetByTokenPayload(tkString);
                Utilizadores.UtilizadorLoggedIn = RegistosLogin.CurrentObj.Utilizador;
                Utilizadores.UtilizadorLoggedInId = Utilizadores.UtilizadorLoggedIn.IdUtilizador;
                tk.Username = Utilizadores.UtilizadorLoggedIn.Nome;
                tk.UserId = Utilizadores.UtilizadorLoggedInId;
                tk.EpisodioId = RegistosLogin.CurrentObj.Episodio.IdEpisodio;
                tk.Episodio = (tk.EpisodioId != Episodio.DummyEpisodioId());
                tk.User = !tk.Episodio;
                tk.Valid = ((tk.Episodio || tk.User) && !tk.Expired);
            }
            if (!tk.Valid)
            {
                tk.Errorcod = 401;
                tk.Errormsg = $"Token inválido! (exp={tk.Expired}, unauthorized/unauthenticated)";
            }
            return tk;
        }
        #endregion
        
    }
}
