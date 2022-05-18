/*
*	<description>SysadminIdiomaController.cs - webcontroller para sysadmin/Idioma</description>
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
    [Route("OSLER/Idioma/")]
    public class IdiomaController : ControllerBase
    {
        /// <summary>
        /// Obter um idioma
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}"), Authorize(Roles = Constantes.CNASysadmin)][Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult GetIdiomaById(ulong id)
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
            
            appctrl.Idiomas.CurrentObj = appctrl.Idiomas.GetById(id);
            if (appctrl.Idiomas.CurrentObj != null)
            {
                return new OkObjectResult(new RecIdTexto(id, appctrl.Idiomas.CurrentObj.Descricao));
            }
            else
                return new BadRequestObjectResult(new RecMensagem($"o {id} não existe!"));
        }
        
        /// <summary>
        /// Modificar um idioma
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("{id}"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult SetIdiomaById(ulong id, [FromBody] DescricaoRequest data)
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
            appctrl.Idiomas.CurrentObj = appctrl.Idiomas.GetById(id);
            if (appctrl.Idiomas.CurrentObj != null)
            {
                appctrl.Idiomas.CurrentObj.Descricao = data.Descricao.Trim();
                try
                {
                    appctrl.Idiomas.VerifyPersistent(appctrl.Idiomas.CurrentObj, 
                        appctrl.Utilizadores.UtilizadorLoggedIn);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(
                        new RecMensagem($"ERRO! ao gravar o Idioma {id} na BD! [{e.Message}]"));
                }
                return new OkObjectResult(new RecIdTexto(id, data.Descricao));
            }
            else 
                return new BadRequestObjectResult(new RecMensagem($"o {id} não existe!"));
        }
        
        /// <summary>
        /// Adicionar um idioma
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("Novo"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult NewIdioma([FromBody] DescricaoRequest data)
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
                return new BadRequestObjectResult(new RecMensagem("ERRO: sem dados!"));
            string temp = data.Descricao.Trim();
            if (temp.Length > 0)
            {
                appctrl.Idiomas.CurrentObj = appctrl.Idiomas.GetByDescricao(temp);
                if (appctrl.Idiomas.CurrentObj != null)
                    return new OkObjectResult(new RecIdTexto(appctrl.Idiomas.CurrentObj.IdIdioma, 
                        appctrl.Idiomas.CurrentObj.Descricao));
                else
                {
                    try
                    {
                        appctrl.Idiomas.CurrentObj =
                            appctrl.Idiomas.NewIdioma(temp, appctrl.Utilizadores.UtilizadorLoggedIn);
                    }
                    catch (Exception e)
                    {
                        return new BadRequestObjectResult(
                            new RecMensagem($"ERRO! ao criar o Idioma! [{e.Message}]"));
                    }
                    return new OkObjectResult(new RecIdTexto(appctrl.Idiomas.CurrentObj.IdIdioma, 
                        appctrl.Idiomas.CurrentObj.Descricao));
                }
            } else 
                return new BadRequestObjectResult(new RecMensagem("tem que preencher a descrição!"));
        }
        
        /// <summary>
        /// Obter a lista de idiomas
        /// </summary>
        /// <returns></returns>
        [HttpGet("Lista"), Authorize(Roles = Constantes.CNATodos)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult ListaIdiomas()
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
            
            return new OkObjectResult(appctrl.Idiomas.GetList());
        }

    }
}