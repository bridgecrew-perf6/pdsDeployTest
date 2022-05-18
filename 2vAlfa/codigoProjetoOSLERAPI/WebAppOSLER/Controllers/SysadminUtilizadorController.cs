/*
*	<description>SysadminUtilizadorController.cs - webcontroller para sysadmin/Utilizador</description>
* 	<author>João Carlos Pinto</author>
*   <date>12-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAppOSLER.Models;
using WebAppOSLERLib.Consts;
using WebAppOSLERLib.CTRL;

namespace WebAppOSLER.Controllers
{
    [ApiController]
    [Route("OSLER/Utilizador/")]
    public class SysadminUtilizadorController : ControllerBase
    {
        /// <summary>
        /// Obter um utilizador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecUtilizador), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult GetUtilizadorById(ulong id)
        {
            // // // //
            // verificar "Authorization"
            string myauth = HttpContext.Request.Headers["Authorization"];
            AppCtrl appctrl = new AppCtrl();
            appctrl.Utilizadores.UtilizadorLoggedInToken = myauth;
            RecAuthorization tk = appctrl.DecodeToken(myauth);
            if (!tk.Valid)
                return new BadRequestObjectResult(new RecMensagem(tk.Errormsg, tk.Errorcod));
            // valores disponíveis:
            // tk.EpisodioId (se tk.Episodio == true)
            // tk.UserId (se tk.User == true)
            // tk.Utente (se o "Utilizador" tiver nivel de acesso == Utente)
            // tk.Acompanhante (se o "Utilizador" tiver nivel de acesso == Acompanhante)
            // tk.Sysadmin (se o "Utilizador" tiver nivel de acesso == Sysadmin)
            // // // //
            
            appctrl.Utilizadores.CurrentObj = appctrl.Utilizadores.GetById(id);
            if (appctrl.Utilizadores.CurrentObj != null)
                return new OkObjectResult(new RecUtilizador(appctrl.Utilizadores.CurrentObj.Nome, 
                    appctrl.Utilizadores.CurrentObj.Password, 
                    appctrl.Utilizadores.CurrentObj.Ativo, 
                    appctrl.Utilizadores.CurrentObj.NivelAcesso, 
                    appctrl.Utilizadores.CurrentObj.Idioma.IdIdioma));
            else
                return new BadRequestObjectResult(new RecMensagem($"o {id} não existe!"));
        }
   
        /// <summary>
        /// Modificar utilizador
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("{id}"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecUtilizador), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult SetUtilizadorById(ulong id, [FromBody] UtilizadorRequest data)
        {
            // // // //
            // verificar "Authorization"
            string myauth = HttpContext.Request.Headers["Authorization"];
            AppCtrl appctrl = new AppCtrl();
            appctrl.Utilizadores.UtilizadorLoggedInToken = myauth;
            RecAuthorization tk = appctrl.DecodeToken(myauth);
            if (!tk.Valid)
                return new BadRequestObjectResult(new RecMensagem(tk.Errormsg, tk.Errorcod));
            // valores disponíveis:
            // tk.EpisodioId (se tk.Episodio == true)
            // tk.UserId (se tk.User == true)
            // tk.Utente (se o "Utilizador" tiver nivel de acesso == Utente)
            // tk.Acompanhante (se o "Utilizador" tiver nivel de acesso == Acompanhante)
            // tk.Sysadmin (se o "Utilizador" tiver nivel de acesso == Sysadmin)
            // // // //
            
            if (data == null) 
                return new BadRequestObjectResult(new RecMensagem($"ERRO: sem dados!"));
            appctrl.Utilizadores.CurrentObj = appctrl.Utilizadores.GetById(id);
            if (appctrl.Utilizadores.CurrentObj != null)
            {
                if (!(data.Nome.Trim().Length > 0)) 
                    return new BadRequestObjectResult(new RecMensagem("o nome tem que estar preenchido!"));
                if (!(data.Password.Trim().Length > 0)) 
                    return new BadRequestObjectResult(new RecMensagem("a password tem que estar preenchida!"));
                appctrl.Idiomas.CurrentObj = appctrl.Idiomas.GetById(data.Idioma);
                if (appctrl.Idiomas.CurrentObj == null)
                {
                    return new BadRequestObjectResult(new RecMensagem($"o idioma {data.Idioma} não existe!"));
                }
                try
                {
                    appctrl.Utilizadores.CurrentObj.Nome = data.Nome.Trim();
                    appctrl.Utilizadores.CurrentObj.Password = data.Password.Trim();
                    appctrl.Utilizadores.CurrentObj.NivelAcesso = data.NivelAcesso;
                    appctrl.Utilizadores.CurrentObj.Ativo = data.Ativo;
                    appctrl.Utilizadores.CurrentObj.Idioma = appctrl.Idiomas.CurrentObj;
                    appctrl.Utilizadores.VerifyPersistent(appctrl.Utilizadores.CurrentObj, 
                        appctrl.Utilizadores.UtilizadorLoggedIn);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(
                        new RecMensagem($"ERRO! ao gravar o Utilizador {id} na BD! [{e.Message}]"));
                }
                // NOTA: não devolver "appctrl.Utilizadores.CurrentObj.Password"
                return new OkObjectResult(new RecUtilizador(appctrl.Utilizadores.CurrentObj.Nome, 
                    "", 
                    appctrl.Utilizadores.CurrentObj.Ativo, 
                    appctrl.Utilizadores.CurrentObj.NivelAcesso, 
                    appctrl.Utilizadores.CurrentObj.Idioma.IdIdioma));
            }
            else
                return new BadRequestObjectResult(new RecMensagem($"o {id} não existe!"));
        }

        /// <summary>
        /// Novo utilizador
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("Novo"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecUtilizador), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult NewUtilizador([FromBody] UtilizadorRequest data)
        {
            // // // //
            // verificar "Authorization"
            string myauth = HttpContext.Request.Headers["Authorization"];
            AppCtrl appctrl = new AppCtrl();
            appctrl.Utilizadores.UtilizadorLoggedInToken = myauth;
            RecAuthorization tk = appctrl.DecodeToken(myauth);
            if (!tk.Valid)
                return new BadRequestObjectResult(new RecMensagem(tk.Errormsg, tk.Errorcod));
            // valores disponíveis:
            // tk.EpisodioId (se tk.Episodio == true)
            // tk.UserId (se tk.User == true)
            // tk.Utente (se o "Utilizador" tiver nivel de acesso == Utente)
            // tk.Acompanhante (se o "Utilizador" tiver nivel de acesso == Acompanhante)
            // tk.Sysadmin (se o "Utilizador" tiver nivel de acesso == Sysadmin)
            // // // //
            
            if (data == null)
                return new BadRequestObjectResult(new RecMensagem($"ERRO: sem dados!"));
            if (!(data.Nome.Trim().Length > 0)) 
                return new BadRequestObjectResult(new RecMensagem("o nome tem que estar preenchido!"));
            if (!(data.Password.Trim().Length > 0)) 
                return new BadRequestObjectResult(new RecMensagem("a password tem que estar preenchida!"));
            appctrl.Idiomas.CurrentObj = appctrl.Idiomas.GetById(data.Idioma);
            if (appctrl.Idiomas.CurrentObj == null)
            {
                return new BadRequestObjectResult(new RecMensagem($"o idioma {data.Idioma} não existe!"));
            }
            try
            {
                appctrl.Utilizadores.CurrentObj = appctrl.Utilizadores.NewUtilizador(
                    data.Nome.Trim(), data.Password.Trim(), data.NivelAcesso, 
                    appctrl.Idiomas.CurrentObj, appctrl.Utilizadores.UtilizadorLoggedIn);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new RecMensagem($"ERRO! ao criar o Utilizador! [{e.Message}]"));
            }
            // NOTA: não devolver "appctrl.Utilizadores.CurrentObj.Password"
            return new OkObjectResult(new RecUtilizador(appctrl.Utilizadores.CurrentObj.Nome, 
                "", 
                appctrl.Utilizadores.CurrentObj.Ativo, 
                appctrl.Utilizadores.CurrentObj.NivelAcesso, 
                appctrl.Utilizadores.CurrentObj.Idioma.IdIdioma));
        }
        
        /// <summary>
        /// Obter lista de níves de acesso
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListaNiveisAcesso"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult ListaNiveisAcesso()
        {
            // // // //
            // verificar "Authorization"
            string myauth = HttpContext.Request.Headers["Authorization"];
            AppCtrl appctrl = new AppCtrl();
            appctrl.Utilizadores.UtilizadorLoggedInToken = myauth;
            RecAuthorization tk = appctrl.DecodeToken(myauth);
            if (!tk.Valid)
                return new BadRequestObjectResult(new RecMensagem(tk.Errormsg, tk.Errorcod));
            // valores disponíveis:
            // tk.EpisodioId (se tk.Episodio == true)
            // tk.UserId (se tk.User == true)
            // tk.Utente (se o "Utilizador" tiver nivel de acesso == Utente)
            // tk.Acompanhante (se o "Utilizador" tiver nivel de acesso == Acompanhante)
            // tk.Sysadmin (se o "Utilizador" tiver nivel de acesso == Sysadmin)
            // // // //

            return new OkObjectResult(appctrl.Utilizadores.GetNivelAcessoList());
        }

    }
}