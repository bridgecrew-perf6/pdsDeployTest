using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebAppOSLER.Models;
using WebAppOSLERLib.Consts;
using WebAppOSLERLib.CTRL;

namespace WebAppOSLER.Controllers
{
    [ApiController]
    [Route("OSLER/[controller]")]
    public class LoginController : ControllerBase
    {
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        /// <remarks>
        ///  POST 
        /// {
        ///  "user": "sysadmintest",
        ///  "password": "sysadmintest"
        /// }
        /// </remarks>
        [HttpPost][Produces("application/json")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status404NotFound)]
        //[EnableCors(Constantes.MyCorsPolicy)]
        public ActionResult Login([FromBody] LoginRequest login)
        {
            AppCtrl appctrl = new AppCtrl();
            if (login == null || login.User.IsNullOrEmpty() || login.Password.IsNullOrEmpty())
                return new BadRequestObjectResult(new RecMensagem("ERRO: sem dados para login!"));
            
            if (!appctrl.Login(login.User, login.Password))
                return new NotFoundObjectResult(
                    new RecMensagem("ERRO: utilizador não existe ou password incorreta!"));
            
            // TODO: falta verificar a melhor forma de identificar o IP do cliente
            string remIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            appctrl.RegistosLogin.CurrentObj = appctrl.RegistosLogin.NewLoginRegisto(
                appctrl.Utilizadores.UtilizadorLoggedIn,
                appctrl.Episodios.CreateDummyEpisodio(appctrl.Utilizadores.UtilizadorLoggedIn),
                DateTime.Now, appctrl.Utilizadores.UtilizadorLoggedInTokenPayload, true,
                DateTime.Now.AddMinutes(Constantes.MinutosValidadeToken),
                remIp,
                appctrl.Utilizadores.UtilizadorLoggedIn);
            HttpContext.Response.Headers.Add("Authorization", 
                "Bearer " + appctrl.Utilizadores.UtilizadorLoggedInToken);
            return new OkObjectResult(new LoginResponse(appctrl.Utilizadores.UtilizadorLoggedInToken));
        }
        /// <summary>
        /// Login/Utente
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <remarks>
        ///  POST  { "codEpisodio": "EP00112233", "pin4": 2022 }
        /// </remarks>
        [HttpPost("Utente")][Produces("application/json")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status404NotFound)]
        public ActionResult LoginUtente([FromBody] LoginUtenteRequest data)
        {
            AppCtrl appctrl = new AppCtrl();
            if (data == null || data.CodEpisodio.IsNullOrEmpty())
                return new BadRequestObjectResult(new RecMensagem("ERRO: sem dados para login!"));
            ulong eId = 0;
            if (!appctrl.LoginUtente(data.CodEpisodio, data.Pin4, true, ref eId))
                return new NotFoundObjectResult(
                    new RecMensagem("ERRO: código de episódio não existe ou pin incorreto!"));
            
            // TODO: falta verificar a melhor forma de identificar o IP do cliente
            string remIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            // NOTA: "appctrl.Episodios.CurrentObj" é inicializado em "appctrl.LoginUtente()"
            appctrl.RegistosLogin.CurrentObj = appctrl.RegistosLogin.NewLoginRegisto(
                appctrl.Utilizadores.UtilizadorLoggedIn,
                appctrl.Episodios.CurrentObj,
                DateTime.Now, appctrl.Utilizadores.UtilizadorLoggedInTokenPayload, true,
                DateTime.Now.AddMinutes(Constantes.MinutosValidadeToken),
                remIp,
                appctrl.Utilizadores.UtilizadorLoggedIn);
            HttpContext.Response.Headers.Add("Authorization", 
                "Bearer " + appctrl.Utilizadores.UtilizadorLoggedInToken);
            return new OkObjectResult(new LoginResponse(appctrl.Utilizadores.UtilizadorLoggedInToken));
        }
        /// <summary>
        /// Login/Acompanhante
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <remarks>
        ///  POST  { "codEpisodio": "EP00112233", "pin4": 2022 }
        /// </remarks>
        [HttpPost("Acompanhante")][Produces("application/json")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status404NotFound)]
        public ActionResult LoginAcompanhante([FromBody] LoginUtenteRequest data)
        {
            AppCtrl appctrl = new AppCtrl();
            if (data == null || data.CodEpisodio.IsNullOrEmpty())
                return new BadRequestObjectResult(new RecMensagem("ERRO: sem dados para login!"));
            ulong eId = 0;
            if (!appctrl.LoginUtente(data.CodEpisodio, data.Pin4, false, ref eId))
                return new NotFoundObjectResult(
                    new RecMensagem("ERRO: código de episódio não existe ou pin incorreto!"));
            
            // TODO: falta verificar a melhor forma de identificar o IP do cliente
            string remIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            // NOTA: "appctrl.Episodios.CurrentObj" é inicializado em "appctrl.LoginUtente()"
            appctrl.RegistosLogin.CurrentObj = appctrl.RegistosLogin.NewLoginRegisto(
                appctrl.Utilizadores.UtilizadorLoggedIn,
                appctrl.Episodios.CurrentObj,
                DateTime.Now, appctrl.Utilizadores.UtilizadorLoggedInTokenPayload, true,
                DateTime.Now.AddMinutes(Constantes.MinutosValidadeToken),
                remIp,
                appctrl.Utilizadores.UtilizadorLoggedIn);
            HttpContext.Response.Headers.Add("Authorization", 
                "Bearer " + appctrl.Utilizadores.UtilizadorLoggedInToken);
            return new OkObjectResult(new LoginResponse(appctrl.Utilizadores.UtilizadorLoggedInToken));
        }
        
        /*[HttpPost("admin")]
        public ActionResult LoginAdmin()
        {
            //string key = "superKeyToBeConnected";
            string key = appctrl.JwtSecret();
            
            //symetric security key
            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            //signing credentials
            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, "admin"));
            
            var token = new JwtSecurityToken(
                "sysadmin",
                "utente",
                expires: DateTime.Now.AddYears(1),
                signingCredentials: signingCredentials,
                claims: claims
            );
            
            return Ok(new JwtSecurityTokenHandler().WriteToken(token)); //TODO: Connect to LIB
        }*/
        
        /*[HttpPost("extra")]
        public ActionResult LoginExtra()
        {
            //string key = "superKeyToBeConnected";
            string key = appctrl.JwtSecret();
            
            //symetric security key
            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            //signing credentials
            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, "extra"));
            
            var token = new JwtSecurityToken(
                "sysadmin",
                "utente",
                expires: DateTime.Now.AddYears(1),
                signingCredentials: signingCredentials,
                claims: claims
            );
            
            return Ok(new JwtSecurityTokenHandler().WriteToken(token)); //TODO: Connect to LIB
        }*/
        
        /*[HttpGet("testutente"), Authorize(Roles = "utente")]
        public ActionResult TEST()
        {
            return Ok();
        }*/

        /// <summary>
        /// Testar o acesso de admin
        /// </summary>
        /// <returns></returns>
        [HttpGet("testeadmin"), Authorize(Roles = Constantes.CNASysadmin)]
        [Produces("application/json")]
        public ActionResult TesteAdmin()
        {
            string myauth = HttpContext.Request.Headers["Authorization"];
            AppCtrl appctrl = new AppCtrl();
            appctrl.Utilizadores.UtilizadorLoggedInToken = myauth;
            appctrl.RegistosLogin.CurrentObj = appctrl.RegistosLogin.GetByTokenPayload(
                appctrl.Utilizadores.UtilizadorLoggedInTokenPayload);
            return new OkObjectResult(new RecMensagem($"Ok ({appctrl.RegistosLogin.CurrentObj.Utilizador.IdUtilizador}) {appctrl.RegistosLogin.CurrentObj.Utilizador.Nome}"));
        }

        /// <summary>
        /// testar o acesso de utente/acompanhante
        /// </summary>
        /// <returns></returns>
        [HttpGet("testeutente"), Authorize(Roles = Constantes.CNAUtentes)]
        [Produces("application/json")]
        public ActionResult TesteUtente()
        {
            string myauth = HttpContext.Request.Headers["Authorization"];
            AppCtrl appctrl = new AppCtrl();
            appctrl.Utilizadores.UtilizadorLoggedInToken = myauth;
            appctrl.RegistosLogin.CurrentObj = appctrl.RegistosLogin.GetByTokenPayload(
                appctrl.Utilizadores.UtilizadorLoggedInTokenPayload);
            return new OkObjectResult(new RecMensagem($"Ok ({appctrl.RegistosLogin.CurrentObj.Utilizador.IdUtilizador}) {appctrl.RegistosLogin.CurrentObj.Episodio.CodEpisodio}, {appctrl.RegistosLogin.CurrentObj.Episodio.Descricao}"));
        }

        /*[HttpGet("testutenteadmin"), Authorize(Roles = "utente,admin")]
        public ActionResult TEST3()
        {
            return Ok();
        }*/
        
        /*[HttpGet("testadminutente"), Authorize(Roles = "admin,utente")]
        public ActionResult TEST4()
        {
            return Ok();
        }*/
    }
}