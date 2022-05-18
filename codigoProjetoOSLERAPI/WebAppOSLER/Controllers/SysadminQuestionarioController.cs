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
    [Route("OSLER/Questionario")]
    public class SysadminQuestionarioController : ControllerBase
    {
        /// <summary>
        /// Obter um questionário
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult GetQuestionarioById(ulong id)
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
        /// Modificar um questionário
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("{id}"), Authorize(Roles = Constantes.CNASysadmin)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult SetQuestionarioById(ulong id, [FromBody] QuestionarioRequest data)
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
                    new RecMensagem("a descrição tem que estar preenchida!"));
            appctrl.Idiomas.CurrentObj = appctrl.Idiomas.GetById(data.IdIdioma);
            if (ReferenceEquals(appctrl.Idiomas.CurrentObj, null))
                return new BadRequestObjectResult(
                    new RecMensagem("a descrição tem que estar preenchida!"));
            appctrl.Questionarios.CurrentObj = appctrl.Questionarios.GetById(id);
            if (appctrl.Questionarios.CurrentObj != null)
            {
                appctrl.Questionarios.CurrentObj.Descricao = data.Descricao.Trim();
                appctrl.Questionarios.CurrentObj.IdiomaQuestionario = appctrl.Idiomas.CurrentObj;
                try
                {
                    appctrl.Questionarios.VerifyPersistent(appctrl.Questionarios.CurrentObj, 
                        appctrl.Utilizadores.UtilizadorLoggedIn);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(
                        new RecMensagem($"ERRO! ao gravar o Questionario {id} na BD! [{e.Message}]"));
                }
                return new OkObjectResult(new RecIdTexto(id, data.Descricao));
            }
            else 
                return new BadRequestObjectResult(new RecMensagem($"o {id} não existe!"));
        }

        /// <summary>
        /// Adicionar questionario
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("Novo"), Authorize(Roles = Constantes.CNASysadmin)][Produces("application/json")]
        [ProducesResponseType(typeof(RecIdDescIdioma), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult AddQuestionario([FromBody] QuestionarioRequest data)
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
            
            if (data.Descricao.IsNullOrEmpty())
                return new BadRequestObjectResult(new RecMensagem("Não tem descrição!"));
            appctrl.Questionarios.CurrentObj = appctrl.Questionarios.NewQuestionario(data.Descricao, 
                appctrl.Idiomas.GetById(data.IdIdioma), 
                appctrl.Utilizadores.UtilizadorLoggedIn);
            return new OkObjectResult(new RecIdDescIdioma(appctrl.Questionarios.CurrentObj.IdQuestionario, 
                appctrl.Questionarios.CurrentObj.IdiomaQuestionario.IdIdioma, 
                appctrl.Questionarios.CurrentObj.Descricao));
        }
        
        /// <summary>
        /// Obter lista de questionarios
        /// </summary>
        /// <returns></returns>
        [HttpGet("Lista"), Authorize(Roles = Constantes.CNATodos)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult GetListaQuestionarios()
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
            
            return new OkObjectResult(appctrl.Questionarios.GetList());
        }
        
        
        #region Perguntas

        /// <summary>
        /// Adicionar Pergunta
        /// </summary>
        /// <param name="idQuestionario"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("{idQuestionario}/Perguntas/Novo")][Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult AdicionarPergunta(ulong idQuestionario, [FromBody] PerguntaRequest data)
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
            if (data.TextoPergunta.IsNullOrEmpty())
                return new BadRequestObjectResult(new RecMensagem($"ERRO: o texto da pergunta tem que estar preenchido!"));
            appctrl.Questionarios.CurrentObj = appctrl.Questionarios.GetById(idQuestionario);
            if (appctrl.Utilizadores.CurrentObj != null)
            {
                try
                {
                    appctrl.Perguntas.CurrentObj = appctrl.Perguntas.NewPergunta(
                        data.TextoPergunta,
                        appctrl.Questionarios.GetById(idQuestionario),
                        appctrl.Utilizadores.UtilizadorLoggedIn);
                    return new OkObjectResult(
                        new RecIdTexto(appctrl.Perguntas.CurrentObj.IdPergunta,
                            appctrl.Perguntas.CurrentObj.TextoPergunta));
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(
                        new RecMensagem($"ERRO! ao criar a pergunta! [{e.Message}]"));
                }
            } else 
                return new BadRequestObjectResult(
                    new RecMensagem($"o questionário {idQuestionario} não existe!"));
        }

        /// <summary>
        /// Adicionar Pergunta
        /// </summary>
        /// <param name="idQuestionario"></param>
        /// <returns></returns>
        [HttpGet("{idQuestionario}/Perguntas/List")][Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult ObterListaPerguntasQuestionario(ulong idQuestionario)
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
            
            //TODO: Verificar resposta
            string listaQuestionario = appctrl.Perguntas.GetListByQuestionario(idQuestionario);
            
            return new OkObjectResult(listaQuestionario);
        }

        #endregion
    }
}