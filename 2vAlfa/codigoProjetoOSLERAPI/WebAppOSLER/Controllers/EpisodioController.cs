using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebAppOSLER.Models;
using WebAppOSLERLib.CTRL;
using WebAppOSLERLib.Consts;

namespace WebAppOSLER.Controllers
{
    [ApiController]
    [Route("OSLER/[controller]")]
    public class EpisodioController : ControllerBase
    {
        [HttpPost("Novo")][Produces("application/json")]
        [ProducesResponseType(typeof(RecIdTexto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult NewEpisodio([FromBody] EpisodioRequest data)
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
            
            appctrl.CoresTriagem.CurrentObj = appctrl.CoresTriagem.GetById(data.IdCor);
            if (appctrl.CoresTriagem.CurrentObj == null) return new BadRequestObjectResult(new RecMensagem("id de cor invalido")); 

            appctrl.Idiomas.CurrentObj = appctrl.Idiomas.GetById(data.IdIdioma);
            if (appctrl.Idiomas.CurrentObj == null) return new BadRequestObjectResult(new RecMensagem("id de idioma invalido"));

            appctrl.Nacionalidades.CurrentObj =  appctrl.Nacionalidades.GetById(data.IdNacionalidade);
            if (appctrl.Nacionalidades.CurrentObj == null) return new BadRequestObjectResult(new RecMensagem("id de nacionalidade invalido"));
            
            if (!(data.CodEpisodio.Trim().Length > 0)) return new BadRequestObjectResult(new RecMensagem("O codigo tem de estar preenchido"));
            if (!(data.Descricao.Trim().Length > 0)) return new BadRequestObjectResult(new RecMensagem("A desciçao tem de estar preenchido"));
            if (!(data.IdSns.Trim().Length > 0)) return new BadRequestObjectResult(new RecMensagem("O idSns tem de estar preenchido"));
            
            //estadoTxt-> quando feixa o que aconteceu
            //estado numero para o estado 
            appctrl.Episodios.CurrentObj = appctrl.Episodios.NewEpisodio(data.CodEpisodio, data.Descricao, data.DataNascimento, data.IdSns, 0, "", appctrl.CoresTriagem.CurrentObj, appctrl.Idiomas.CurrentObj, appctrl.Nacionalidades.CurrentObj, appctrl.Utilizadores.UtilizadorLoggedIn);
            appctrl.Episodios.VerifyPersistent(appctrl.Episodios.CurrentObj);
            return new OkObjectResult(new RecIdTexto(appctrl.Episodios.CurrentObj.IdEpisodio,"O id do episodio criado"));
        }
        
        #region Estado
        
        [HttpPost("{idEpisodio}/MudarEstado")][Produces("application/json")]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult ModificarEstado(ulong idEpisodio,[FromBody] MudarEstadoRequest data)
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
            
            appctrl.Episodios.CurrentObj = appctrl.Episodios.GetById(idEpisodio);
            if (appctrl.Episodios.CurrentObj == null) 
                return new BadRequestObjectResult(new RecMensagem("id de episodio invalido"));
            //TODO: verificar estado quando/se existir enum
            appctrl.Episodios.CurrentObj.Estado = data.Estado;
            appctrl.Episodios.CurrentObj.EstadoTxt = data.Descricao;
            
            return new OkObjectResult(new RecMensagem("Estado do episodio alterado com sucesso"));
        }
        #endregion
        
        #region historico
        
        [HttpGet("Historico/{SnSUtente}")][Produces("application/json")]
        [ProducesResponseType(typeof(List<RecEpisodio>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult HistoricoEpisodios(string snsUtente)
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
            
            List<RecEpisodio> list = appctrl.Episodios.GetListEpisodioByIdSns(snsUtente);

            if (!(list.Count > 0))
                return new BadRequestObjectResult(new RecMensagem("o utente nao tem historico de episodios"));
            
            return new OkObjectResult(list);
        }
        
        #endregion
        
        
        #region Local 

        /// <summary>
        /// Função para obter localização do utente
        /// </summary>
        /// <param name="idEpisodio">Episodio a pesquisar</param>
        /// <returns></returns>
        [HttpGet("{idEpisodio}/Local")][Produces("application/json")]
        [ProducesResponseType(typeof(RecLocal), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult ObterLocalizacaoUtente(ulong idEpisodio)
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
            
            appctrl.EpisodioHistLocal.CurrentObj = appctrl.EpisodioHistLocal.GetLastLocationFromEpisode(idEpisodio);
            RecLocal local = new RecLocal(
                appctrl.EpisodioHistLocal.CurrentObj.Local.IdLocal,
                appctrl.EpisodioHistLocal.CurrentObj.DataHora,
                appctrl.EpisodioHistLocal.CurrentObj.Episodio.IdEpisodio);
            return new OkObjectResult(local);
        }
        
        /// <summary>
        /// Função para modificar localização do utente
        /// </summary>
        /// <param name="idEpisodio">Episodio onde irá ser feita alteração</param>
        /// <param name="idLocal">Local novo do utente</param>
        /// <returns></returns>
        [HttpPost("{idEpisodio}/Local/{idLocal}")][Produces("application/json")]
        [ProducesResponseType(typeof(RecLocal), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult ModificarLocalizaçãoUtente(ulong idEpisodio, ulong idLocal)
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
            
            appctrl.EpisodioHistLocal.CurrentObj = appctrl.EpisodioHistLocal.NewEpisodioHistLocal(
                appctrl.Locais.GetById(idLocal), DateTime.Now,
                appctrl.Episodios.GetById(idEpisodio));

            RecLocal local = new RecLocal(
                appctrl.EpisodioHistLocal.CurrentObj.Local.IdLocal,
                appctrl.EpisodioHistLocal.CurrentObj.DataHora,
                appctrl.EpisodioHistLocal.CurrentObj.Episodio.IdEpisodio
            );
            return new OkObjectResult(local);
        }

        #endregion

        #region Registo Dados

        /// <summary>
        /// Função para atribuir novo dado ao utente
        /// </summary>
        /// <param name="idEpisodio">Episodio onde ira ser inserido novo dado</param>
        /// <param name="dadosRequest">Dados para enviar</param>
        /// <returns></returns>
        [HttpPost("{idEpisodio}/Dados")][Produces("application/json")]
        [ProducesResponseType(typeof(RecRegistoDados), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult AdicionarDados(ulong idEpisodio, [FromBody] DadosRequest dadosRequest)
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
            appctrl.EpisodioRegistoDados.CurrentObj = appctrl.EpisodioRegistoDados.NewEpisodioRegistoDados(
                appctrl.Episodios.GetById(idEpisodio),
                appctrl.TiposLeitura.GetById(dadosRequest.IdTipoDado),
                dadosRequest.Valor,
                DateTime.Now
            );

            RecRegistoDados registoDados = new RecRegistoDados(
                appctrl.EpisodioRegistoDados.CurrentObj.Episodio.IdEpisodio,
                appctrl.EpisodioRegistoDados.CurrentObj.TipoLeitura.IdTipoLeitura,
                appctrl.EpisodioRegistoDados.CurrentObj.Valor,
                appctrl.EpisodioRegistoDados.CurrentObj.DataHora
            );
            return new OkObjectResult(registoDados);
        }
        
        /// <summary>
        /// Obter a lista de dados inseridos
        /// </summary>
        /// <param name="idEpisodio">Episodio a pesquisar</param>
        /// <returns></returns>
        [HttpGet("{idEpisodio}/Dados")][Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult ObterDados(ulong idEpisodio)
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
            
            //TODO: Verificar dados recebidos
            string dados = appctrl.EpisodioRegistoDados.GetListByEpisodio(idEpisodio);
            
            return new OkObjectResult(dados);
        }

        #endregion

        #region Questionario

        /// <summary>
        /// Função para adicionar questionarios a um episodio
        /// </summary>
        /// <param name="idEpisodio">episodio que ira receber o questionario</param>
        /// <param name="episodioQuestionarioRequest"></param>
        /// <returns></returns>
        [HttpPost("{idEpisodio}/Questionario/Add")][Produces("application/json")]
        [ProducesResponseType(typeof(RecEpisodioQuestionario), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult AddQuestionario(ulong idEpisodio, [FromBody] EpisodioQuestionarioRequest episodioQuestionarioRequest)
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
            
            appctrl.EpisodioQuestionario.CurrentObj = appctrl.EpisodioQuestionario.NewEpisodioRegistoDados(
                appctrl.Episodios.GetById(idEpisodio),
                appctrl.Questionarios.GetById(episodioQuestionarioRequest.IdQuestionario),
                episodioQuestionarioRequest.SequenciaQuestionario
            );

            RecEpisodioQuestionario recEpisodioQuestionario = new RecEpisodioQuestionario(
                appctrl.EpisodioQuestionario.CurrentObj.IdEpisodioQuestionario,
                appctrl.EpisodioQuestionario.CurrentObj.Episodio.IdEpisodio,
                appctrl.EpisodioQuestionario.CurrentObj.Questionario.IdQuestionario,
                appctrl.EpisodioQuestionario.CurrentObj.SequenciaQuestionario
            );
            
            return new OkObjectResult(recEpisodioQuestionario);
        }
        
        /// <summary>
        /// Função para obter questionarios de um episodio
        /// </summary>
        /// <param name="idEpisodio"></param>
        /// <returns></returns>
        [HttpGet("{idEpisodio}/Questionario/List")][Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult ObterListaQuestionario(ulong idEpisodio)
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
            string listaQuestionario = appctrl.EpisodioQuestionario.GetListByEpisodio(idEpisodio);
            
            return new OkObjectResult(listaQuestionario);
        }
        
        /// <summary>
        /// Adicionar resposta a questionario
        /// </summary>
        /// <param name="idEpisodio"></param>
        /// <param name="episodioQuestRepostaRequest"></param>
        /// <returns></returns>
        [HttpPost("{idEpisodio}/Questionario/Responder")][Produces("application/json")]
        [ProducesResponseType(typeof(RecQuestResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult AdicionarResposta(ulong idEpisodio, [FromBody] EpisodioQuestRepostaRequest episodioQuestRepostaRequest)
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
            
            if (episodioQuestRepostaRequest == null)
                return new BadRequestObjectResult(new RecMensagem($"ERRO: sem dados!"));
            if(episodioQuestRepostaRequest.Resposta.IsNullOrEmpty())
                return new BadRequestObjectResult(new RecMensagem($"ERRO: Resposta tem de estar preeenchida"));

            //TODO: verificar autenticação
            //TODO: verificar existencia do episodio

            appctrl.EpisodioQuestResposta.CurrentObj = appctrl.EpisodioQuestResposta.NewEpisodioQuestResposta(
                appctrl.EpisodioQuestionario.GetById(episodioQuestRepostaRequest.IdEpisodioQuestionario),
                appctrl.Questionarios.GetById(episodioQuestRepostaRequest.IdQuestionario),
                appctrl.Perguntas.GetById(episodioQuestRepostaRequest.IdPergunta),
                episodioQuestRepostaRequest.Resposta,
                episodioQuestRepostaRequest.Ativo
            );

            RecQuestResposta recQuestResposta = new RecQuestResposta(
                appctrl.EpisodioQuestResposta.CurrentObj.EpisodioQuestionario.IdEpisodioQuestionario,
                appctrl.EpisodioQuestResposta.CurrentObj.Pergunta.TextoPergunta,
                appctrl.EpisodioQuestResposta.CurrentObj.Resposta,
                appctrl.EpisodioQuestResposta.CurrentObj.CriadoPor.IdUtilizador
            );
            
            return new OkObjectResult(recQuestResposta);
        }

        /// <summary>
        /// Obter respostas do episodio
        /// </summary>
        /// <param name="idEpisodio"></param>
        /// <returns></returns>
        [HttpGet("{idEpisodio}/Respostas")][Produces("application/json")]
        [ProducesResponseType(typeof(List<RecQuestResposta>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult ObterRespostas(ulong idEpisodio)
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
            
            try
            {
                List<RecQuestResposta> recQuestRespostas = appctrl.EpisodioQuestResposta.ObterRespostasByEpisodio(idEpisodio);
                return new OkObjectResult(recQuestRespostas);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new RecMensagem($"ERRO! ao obter respostas! [{e.Message}]"));
            }
        }

        /// <summary>
        /// Alterar respostas de um questionário
        /// </summary>
        /// <param name="idEpisodio"></param>
        /// <param name="episodioQuestRepostaRequest"></param>
        /// <returns></returns>
        [HttpPost("{idEpisodio}/Questionario/AlterarResposta")]
        [ProducesResponseType(typeof(RecQuestResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult AlterarResposta(ulong idEpisodio, [FromBody] EpisodioQuestRepostaRequest episodioQuestRepostaRequest)
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
            
            if (episodioQuestRepostaRequest == null)
                return new BadRequestObjectResult(JsonConvert.SerializeObject(
                    new RecMensagem($"ERRO: sem dados!")));
            if(episodioQuestRepostaRequest.Resposta.IsNullOrEmpty())
                return new BadRequestObjectResult(JsonConvert.SerializeObject(
                    new RecMensagem($"ERRO: Resposta tem de estar preeenchida")));

            appctrl.EpisodioQuestResposta.CurrentObj = appctrl.EpisodioQuestResposta.GetByIdEpQuestionaSequeResp(
                episodioQuestRepostaRequest.IdEpisodioQuestionario, episodioQuestRepostaRequest.SequenciaResposta);
            
            appctrl.EpisodioQuestResposta.CurrentObj.Resposta = episodioQuestRepostaRequest.Resposta;
           
            if (appctrl.EpisodioQuestResposta.CurrentObj != null)
            {
                try
                {
                    appctrl.EpisodioQuestResposta.VerifyPersistent(appctrl.EpisodioQuestResposta.CurrentObj, 
                        appctrl.Utilizadores.UtilizadorLoggedIn);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(
                        new RecMensagem($"ERRO! ao gravar as respostas {idEpisodio} na BD! [{e.Message}]"));
                }
                return new OkObjectResult(new RecMensagem($"As respostas {idEpisodio} foram alteradas com sucesso!"));
            }
            else 
                return new BadRequestObjectResult(new RecMensagem($"o {idEpisodio} não existe!"));
        }
        
        /// <summary>
        /// Função que recebe dados pessoais de um utente
        /// </summary>
        /// <param name="idEpisodio"></param>
        /// <returns></returns>
        [HttpGet("{idEpisodio}/DadosPessoais"), Authorize(Roles = Constantes.CNATodos)][Produces("application/json")]
        [ProducesResponseType(typeof(RecEpisodio), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult ReceberDados([Required] ulong idEpisodio)
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
            
            try
            {
                appctrl.Episodios.CurrentObj = appctrl.Episodios.GetById(idEpisodio);
                if (appctrl.Episodios.CurrentObj != null)
                    return new OkObjectResult(new RecEpisodio(appctrl.Episodios.CurrentObj.IdEpisodio,
                        appctrl.Episodios.CurrentObj.EstadoTxt,
                        appctrl.Episodios.CurrentObj.Descricao,
                        appctrl.Episodios.CurrentObj.DataNascimento,
                        appctrl.Episodios.CurrentObj.IdSns));
                else
                    return new BadRequestObjectResult(new RecMensagem($"o {idEpisodio} não existe!"));
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new RecMensagem($"ERRO! ao obter dados pessoais! [{e.Message}]"));
            }
        }

        #endregion
        
    }
}