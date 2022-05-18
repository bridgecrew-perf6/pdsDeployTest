/*
*	<description>SysadminTipoLeituraController.cs - webcontroller para sysadmin/TipoLeitura</description>
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
    [Route("OSLER/TipoLeitura/")]
    public class SysadminTipoLeituraController : ControllerBase
    {
        /// <summary>
        /// Obter um local
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecTipoLeitura), StatusCodes.Status200OK)]
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
            
            appctrl.TiposLeitura.CurrentObj = appctrl.TiposLeitura.GetById(id);
            if (appctrl.TiposLeitura.CurrentObj != null)
                return new OkObjectResult(new RecTipoLeitura(id, appctrl.TiposLeitura.CurrentObj.Descricao, 
                        appctrl.TiposLeitura.CurrentObj.Medida));
            else
                return new BadRequestObjectResult(new RecMensagem($"o {id} não existe!"));
        }
        
        /// <summary>
        /// Modificar um local
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("{id}"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecTipoLeitura), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult SetLocalById(ulong id, [FromBody] TipoLeituraRequest data)
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
                return new BadRequestObjectResult(new RecMensagem("a descricao tem que estar preenchida!"));
            if (!(data.Medida.Trim().Length > 0)) 
                return new BadRequestObjectResult(new RecMensagem("a medida tem que estar preenchida!"));
            appctrl.TiposLeitura.CurrentObj = appctrl.TiposLeitura.GetById(id);
            if (appctrl.TiposLeitura.CurrentObj != null)
            {
                appctrl.TiposLeitura.CurrentObj.Descricao = data.Descricao.Trim();
                appctrl.TiposLeitura.CurrentObj.Medida = data.Medida.Trim();
                try
                {
                    appctrl.TiposLeitura.VerifyPersistent(appctrl.TiposLeitura.CurrentObj, 
                        appctrl.Utilizadores.UtilizadorLoggedIn);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(new RecMensagem($"ERRO! ao gravar o TipoLeitura {id} na BD! [{e.Message}]"));
                }
                return new OkObjectResult(new RecTipoLeitura(id, data.Descricao, data.Medida));
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
        [ProducesResponseType(typeof(RecTipoLeitura), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult NewLocal([FromBody] TipoLeituraRequest data)
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
                return new BadRequestObjectResult(new RecMensagem("a descricao tem que estar preenchida!"));
            if (!(data.Medida.Trim().Length > 0)) 
                return new BadRequestObjectResult(new RecMensagem("a medida tem que estar preenchida!"));
            
            appctrl.TiposLeitura.CurrentObj = appctrl.TiposLeitura.GetByDescricao(data.Descricao.Trim());
            if (appctrl.TiposLeitura.CurrentObj != null)
                return new OkObjectResult(
                    new RecTipoLeitura(appctrl.TiposLeitura.CurrentObj.IdTipoLeitura, 
                        appctrl.TiposLeitura.CurrentObj.Descricao, 
                        appctrl.TiposLeitura.CurrentObj.Medida));
            else
            {
                try
                {
                    appctrl.TiposLeitura.CurrentObj =
                        appctrl.TiposLeitura.NewTipoLeitura(data.Descricao.Trim(), data.Medida.Trim(), 
                            appctrl.Utilizadores.UtilizadorLoggedIn);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(new RecMensagem($"ERRO! ao criar o TipoLeitura! [{e.Message}]"));
                }
                return new OkObjectResult(new RecTipoLeitura(appctrl.TiposLeitura.CurrentObj.IdTipoLeitura, 
                        appctrl.TiposLeitura.CurrentObj.Descricao, appctrl.TiposLeitura.CurrentObj.Medida));
            }
        }
        
        /// <summary>
        /// Obter uma lista de tipos de leitura
        /// </summary>
        /// <returns></returns>
        [HttpGet("Lista"), Authorize(Roles = Constantes.CNATodos)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecTipoLeitura[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult TiposLeitura()
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

            return new OkObjectResult(appctrl.TiposLeitura.GetList());
        }

    }
}