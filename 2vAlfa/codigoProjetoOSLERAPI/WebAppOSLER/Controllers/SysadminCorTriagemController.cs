/*
*	<description>SysadminCorTriagemController.cs - webcontroller para sysadmin/CorTriagem</description>
* 	<author>João Carlos Pinto</author>
*   <date>25-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAppOSLERLib.Consts;
using WebAppOSLERLib.CTRL;

namespace WebAppOSLER.Controllers
{
    [ApiController]
    [Route("OSLER/CorTriagem/")]
    public class SysadminCorTriagemController : ControllerBase
    {
        [HttpGet("Lista"), Authorize(Roles = Constantes.CNATodos)] [Produces("application/json")]
        [ProducesResponseType(typeof(RecIdDescCor[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RecMensagem), StatusCodes.Status400BadRequest)]
        public ActionResult Cores()
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

            return new OkObjectResult(appctrl.CoresTriagem.GetList());
        }

    }
}