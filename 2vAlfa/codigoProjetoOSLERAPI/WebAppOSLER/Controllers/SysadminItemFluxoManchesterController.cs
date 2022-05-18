/*
*	<description>SysadminItemFluxoManchesterController.cs - webcontroller para sysadmin/ItemFluxoManchester</description>
* 	<author>João Carlos Pinto</author>
*   <date>13-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebAppOSLER.Models;
using WebAppOSLERLib.Consts;
using WebAppOSLERLib.CTRL;

namespace WebAppOSLER.Controllers
{
    [ApiController]
    [Route("OSLER/ItemFluxoManchester/")]
    public class SysadminItemFluxoManchesterController : ControllerBase
    {
        /// <summary>
        /// Obter um item do fluxo de manchester
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecItemFluxoManchester), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult GetItemFluxoManchesterById(ulong id)
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
            
            appctrl.ItensFluxoManchester.CurrentObj = appctrl.ItensFluxoManchester.GetById(id);
            if (appctrl.ItensFluxoManchester.CurrentObj != null)
                return new OkObjectResult(new RecItemFluxoManchester(id, 
                    appctrl.ItensFluxoManchester.CurrentObj.Descricao, 
                    appctrl.ItensFluxoManchester.CurrentObj.Classificacao.IdCorTriagem, 
                    appctrl.ItensFluxoManchester.CurrentObj.Classificacao.Descricao, 
                    appctrl.ItensFluxoManchester.CurrentObj.Classificacao.CodigoCorHex));
            else
                return new BadRequestObjectResult(new RecMensagem($"o {id} não existe!"));
        }
        
        /// <summary>
        /// Modificar um item do fluxo de manchester
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("{id}"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecItemFluxoManchester), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult SetItemFluxoManchesterById(ulong id, [FromBody] ItemFluxoManchesterRequest data)
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
            
            if (data.Descricao.Trim().Length < 1) 
                return new BadRequestObjectResult(
                    new RecMensagem("a descricao tem que estar preenchida!"));
            appctrl.ItensFluxoManchester.CurrentObj = appctrl.ItensFluxoManchester.GetById(id);
            if (appctrl.ItensFluxoManchester.CurrentObj != null)
            {
                appctrl.ItensFluxoManchester.CurrentObj.Descricao = data.Descricao.Trim();
                appctrl.ItensFluxoManchester.CurrentObj.Classificacao = appctrl.CoresTriagem.GetById(data.IdClassificacao);
                try
                {
                    appctrl.ItensFluxoManchester.VerifyPersistent(appctrl.ItensFluxoManchester.CurrentObj, 
                        appctrl.Utilizadores.UtilizadorLoggedIn);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(
                        new RecMensagem($"ERRO! ao gravar o ItemFluxoManchester {id} na BD! [{e.Message}]"));
                }
                return new OkObjectResult(new RecItemFluxoManchester(id, appctrl.ItensFluxoManchester.CurrentObj.Descricao, 
                        appctrl.ItensFluxoManchester.CurrentObj.Classificacao.IdCorTriagem, 
                        appctrl.ItensFluxoManchester.CurrentObj.Classificacao.Descricao, 
                        appctrl.ItensFluxoManchester.CurrentObj.Classificacao.CodigoCorHex));
            }
            else 
                return new BadRequestObjectResult(new RecMensagem($"o {id} não existe!"));
        }
        
        /// <summary>
        /// Adicionar um item do fluxo de manchester
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("Novo"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecItemFluxoManchester), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult NewItemFluxoManchester([FromBody] ItemFluxoManchesterRequest data)
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
                appctrl.CoresTriagem.CurrentObj = appctrl.CoresTriagem.GetById(data.IdClassificacao);
                if (appctrl.CoresTriagem.CurrentObj == null)
                    return new BadRequestObjectResult(
                        new RecMensagem($"a classificação {data.IdClassificacao} não existe!"));
                
                appctrl.ItensFluxoManchester.CurrentObj = appctrl.ItensFluxoManchester.GetByDescricao(temp);
                if (appctrl.ItensFluxoManchester.CurrentObj != null)
                {
                    appctrl.ItensFluxoManchester.CurrentObj.Descricao = temp;
                    appctrl.ItensFluxoManchester.CurrentObj.Classificacao = appctrl.CoresTriagem.CurrentObj;
                    try
                    {
                        appctrl.ItensFluxoManchester.VerifyPersistent(
                            appctrl.ItensFluxoManchester.CurrentObj, 
                            appctrl.Utilizadores.UtilizadorLoggedIn);
                    }
                    catch (Exception e)
                    {
                        return new BadRequestObjectResult(
                            new RecMensagem($"ERRO! ao gravar o ItemFluxoManchester \"{data.Descricao}\" na BD! [{e.Message}]"));
                    }
                    return new OkObjectResult(new RecItemFluxoManchester(
                        appctrl.ItensFluxoManchester.CurrentObj.IdItemFluxoManchester,
                        appctrl.ItensFluxoManchester.CurrentObj.Descricao,
                        appctrl.ItensFluxoManchester.CurrentObj.Classificacao.IdCorTriagem,
                        appctrl.ItensFluxoManchester.CurrentObj.Classificacao.Descricao,
                        appctrl.ItensFluxoManchester.CurrentObj.Classificacao.CodigoCorHex));
                }
                else
                {
                    try
                    {
                        appctrl.ItensFluxoManchester.CurrentObj =
                            appctrl.ItensFluxoManchester.NewItemFluxoManchester(temp, 
                                appctrl.CoresTriagem.CurrentObj, 
                                appctrl.Utilizadores.UtilizadorLoggedIn);
                    }
                    catch (Exception e)
                    {
                        return new BadRequestObjectResult(
                            new RecMensagem($"ERRO! ao criar ItemFluxoManchester! [{e.Message}]"));
                    }
                    return new OkObjectResult(new RecItemFluxoManchester(
                        appctrl.ItensFluxoManchester.CurrentObj.IdItemFluxoManchester, 
                        appctrl.ItensFluxoManchester.CurrentObj.Descricao, 
                        appctrl.ItensFluxoManchester.CurrentObj.Classificacao.IdCorTriagem, 
                        appctrl.ItensFluxoManchester.CurrentObj.Classificacao.Descricao, 
                        appctrl.ItensFluxoManchester.CurrentObj.Classificacao.CodigoCorHex));
                }
            } else 
                return new BadRequestObjectResult(new RecMensagem($"tem que preencher a descrição!"));
        }
        
        /// <summary>
        /// Obter lista de itens do fluxo de manchester
        /// </summary>
        /// <returns></returns>
        [HttpGet("Lista"), Authorize(Roles = Constantes.CNATodos)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecItemFluxoManchester[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult ListaFluxoManchester()
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
            
            //return new OkObjectResult(JsonConvert.SerializeObject(appctrl.ItensFluxoManchester.GetList()));
            return new OkObjectResult(appctrl.ItensFluxoManchester.GetList());
        }
        
    }
}