/*
*	<description>SysadminLocalController.cs - webcontroller para sysadmin/Local</description>
* 	<author>João Carlos Pinto</author>
*   <date>10-04-2022</date>
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
    [Route("OSLER/Local/")]
    public class SysadminLocalController : ControllerBase
    {
        /// <summary>
        /// Obter um local
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult GetLocalById(ulong id)
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
            
            appctrl.Locais.CurrentObj = appctrl.Locais.GetById(id);
            if (appctrl.Locais.CurrentObj != null)
                return new OkObjectResult(new RecIdTexto(id, appctrl.Locais.CurrentObj.Descricao));
            else
                return new BadRequestObjectResult(new RecMensagem($"o {id} não existe!"));
        }
        
        /// <summary>
        /// Modificar local
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("{id}"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult SetLocalById(ulong id, [FromBody] DescricaoRequest data)
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
            
            if (!(data.Descricao.Trim().Length > 0)) 
                return new BadRequestObjectResult(
                    new RecMensagem("a descricao tem que estar preenchida!"));
            appctrl.Locais.CurrentObj = appctrl.Locais.GetById(id);
            if (appctrl.Locais.CurrentObj != null)
            {
                appctrl.Locais.CurrentObj.Descricao = data.Descricao.Trim();
                try
                {
                    appctrl.Locais.VerifyPersistent(appctrl.Locais.CurrentObj, 
                        appctrl.Utilizadores.UtilizadorLoggedIn);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(
                        new RecMensagem($"ERRO! ao gravar o Local {id} na BD! [{e.Message}]"));
                }
                return new OkObjectResult(new RecIdTexto(id, data.Descricao));
            }
            else 
                return new BadRequestObjectResult(new RecMensagem($"o {id} não existe!"));
        }
        
        /// <summary>
        /// Adicionar um local
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("Novo"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult NewLocal([FromBody] DescricaoRequest data)
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
            string temp = data.Descricao.Trim();
            if (temp.Length > 0)
            {
                appctrl.Locais.CurrentObj = appctrl.Locais.GetByDescricao(temp);
                if (appctrl.Locais.CurrentObj != null)
                    return new OkObjectResult(new RecIdTexto(appctrl.Locais.CurrentObj.IdLocal, 
                        appctrl.Locais.CurrentObj.Descricao));
                else
                {
                    try
                    {
                        appctrl.Locais.CurrentObj =
                            appctrl.Locais.NewLocal(temp, appctrl.Utilizadores.UtilizadorLoggedIn);
                    }
                    catch (Exception e)
                    {
                        return new BadRequestObjectResult(
                            new RecMensagem($"ERRO! ao criar o Local! [{e.Message}]"));
                    }
                    return new OkObjectResult(new RecIdTexto(appctrl.Locais.CurrentObj.IdLocal, 
                        appctrl.Locais.CurrentObj.Descricao));
                }
            } else 
                return new BadRequestObjectResult(
                    new RecMensagem($"tem que preencher a descrição!"));
        }
        
        /// <summary>
        /// Obter lista de locais
        /// </summary>
        /// <returns></returns>
        [HttpGet("Lista"), Authorize(Roles = Constantes.CNATodos)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult ListaLocais()
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

            return new OkObjectResult(appctrl.Locais.GetList());
        }

    }
}