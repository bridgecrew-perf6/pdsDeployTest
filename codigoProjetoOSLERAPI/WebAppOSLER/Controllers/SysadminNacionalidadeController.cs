/*
*	<description>SysadminNacionalidadeController.cs - webcontroller para sysadmin/Nacionalidade</description>
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
    [Route("OSLER/Nacionalidade/")]
    public class SysadminNacionalidadeController : ControllerBase
    {
        /// <summary>
        /// Obter nacionalidade
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}"), Authorize(Roles = Constantes.CNASysadmin)][Produces("application/json")]
        [ProducesResponseType(typeof(RecIdDescIdioma), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult GetNacionalidadeById(ulong id)
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
            
            appctrl.Nacionalidades.CurrentObj = appctrl.Nacionalidades.GetById(id);
            if (appctrl.Nacionalidades.CurrentObj != null)
                return new OkObjectResult(new RecIdDescIdioma(id, 
                    appctrl.Nacionalidades.CurrentObj.IdiomaNacionalidade.IdIdioma,
                    appctrl.Nacionalidades.CurrentObj.Descricao));
            else
                return new BadRequestObjectResult(new RecMensagem($"o {id} não existe!"));
        }
        
        /// <summary>
        /// Modificar nacionalidade
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("{id}"), Authorize(Roles = Constantes.CNASysadmin)][Produces("application/json")]
        [ProducesResponseType(typeof(RecIdDescIdioma), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult SetNacionalidadeById(ulong id, [FromBody] NacionalidadeRequest data)
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
            
            if (!(data.Descricao.Trim().Length > 0)) 
                return new BadRequestObjectResult(
                    new RecMensagem("a descricao tem que estar preenchida!"));
            appctrl.Nacionalidades.CurrentObj = appctrl.Nacionalidades.GetById(id);
            if (appctrl.Nacionalidades.CurrentObj != null)
            {
                appctrl.Nacionalidades.CurrentObj.Descricao = data.Descricao;
                appctrl.Nacionalidades.CurrentObj.IdiomaNacionalidade = appctrl.Idiomas.GetById(data.IdIdioma);
                try
                {
                    appctrl.Nacionalidades.VerifyPersistent(appctrl.Nacionalidades.CurrentObj, 
                        appctrl.Utilizadores.UtilizadorLoggedIn);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(
                        new RecMensagem($"ERRO! ao gravar o Nacionalidade {id} na BD! [{e.Message}]"));
                }
                return new OkObjectResult(new RecIdDescIdioma(id, 
                    appctrl.Nacionalidades.CurrentObj.IdiomaNacionalidade.IdIdioma, data.Descricao));
            }
            else 
                return new BadRequestObjectResult(new RecMensagem($"o {id} não existe!"));
        }
        
        /// <summary>
        /// Adicionar nacionalidade
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("Novo"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdDescIdioma), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult NewNacionalidade([FromBody] NacionalidadeRequest data)
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
            
            string temp = data.Descricao.Trim();
            if (temp.Length > 0)
            {
                appctrl.Nacionalidades.CurrentObj = appctrl.Nacionalidades.GetByDescricao(temp);
                if (appctrl.Nacionalidades.CurrentObj != null)
                    return new OkObjectResult(new RecIdDescIdioma(appctrl.Nacionalidades.CurrentObj.IdNacionalidade, 
                        appctrl.Nacionalidades.CurrentObj.IdiomaNacionalidade.IdIdioma, 
                        appctrl.Nacionalidades.CurrentObj.Descricao));
                else
                {
                    appctrl.Idiomas.CurrentObj = appctrl.Idiomas.GetById(data.IdIdioma);
                    if (appctrl.Idiomas.CurrentObj == null)
                        return new BadRequestObjectResult(new RecMensagem($"o idioma {data.IdIdioma} não existe!"));
                    try
                    {
                        appctrl.Nacionalidades.CurrentObj =
                            appctrl.Nacionalidades.NewNacionalidade(temp, appctrl.Idiomas.CurrentObj, 
                                appctrl.Utilizadores.UtilizadorLoggedIn);
                    }
                    catch (Exception e)
                    {
                        return new BadRequestObjectResult(
                            new RecMensagem($"ERRO! ao criar Nacionalidade! [{e.Message}]"));
                    }
                    return new OkObjectResult(new RecIdDescIdioma(appctrl.Nacionalidades.CurrentObj.IdNacionalidade, 
                        appctrl.Nacionalidades.CurrentObj.IdiomaNacionalidade.IdIdioma, 
                        appctrl.Nacionalidades.CurrentObj.Descricao));
                }
            } else 
                return new BadRequestObjectResult(new RecMensagem($"tem que preencher a descrição!"));
        }
        
        /// <summary>
        /// Obter lista de nacionalidades
        /// </summary>
        /// <returns></returns>
        [HttpGet("Lista"), Authorize(Roles = Constantes.CNATodos)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult ListaNacionalidades()
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

            return new OkObjectResult(appctrl.Nacionalidades.GetList());
        }

    }
}