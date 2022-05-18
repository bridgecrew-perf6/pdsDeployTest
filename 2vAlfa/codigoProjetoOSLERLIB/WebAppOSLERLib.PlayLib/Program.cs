/*
*	<description>Program.cs, projeto para testar a LIB e produzir dados base e de teste</description>
* 	<author>João Carlos Pinto</author>
*   <date>25-03-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.Threading;
using System.Threading.Tasks;
using WebAppOSLERLib.BO;
using WebAppOSLERLib.CTRL;
using WebAppOSLERLib.Tools;

namespace WebAppOSLERLib.PlayLib
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("[inicio]>>>");
            VerificarUtilizadorSistemaTodos();
            VerificarCriarCoresTriagemTodos();
            Parallel.Invoke(
                () => ShowInfo(), 
                () => Show10IDs(),
                () => TestarUserTeste(), 
                () => VerificarCriarUserTesteTodos(), 
                () => VerificarItemFluxoManchesterTodos(),
                () => Show10IDs(),
                () => VerificarLocalSistemaTodos(),
                () => VerificarCriarEpisodioTesteTodos()
            );
            Console.WriteLine("<<<[fim]");
            Console.WriteLine("premir ENTER para terminar...");
            Console.ReadLine();
        }

        static void VerificarLocalSistemaTodos()
        {
            string descricao = Local.DefaultLocalDescricao();
            Console.WriteLine($"VerificarLocalOmissão({Local.DefaultLocalId()}, {descricao})");
            
            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) verificar se existe o local: {descricao}");
            appctrl.Locais.CurrentObj=appctrl.Locais.GetById(Local.DefaultLocalId());
            if (appctrl.Locais.CurrentObj == null)
            {
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) local \"{descricao}\" não existe na base de dados!");
                appctrl.Locais.CurrentObj = appctrl.Locais.GetDefaultLocal(appctrl.Utilizadores.GetById(2));
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) o local \"{descricao}\" foi criado com o id {appctrl.Locais.CurrentObj.IdLocal}");
            } else 
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) o local \"{descricao}\" já existe na base de dados com o id {appctrl.Locais.CurrentObj.IdLocal}!");
        }
        
        static void VerificarUtilizadorSistemaTodos()
        {
            Console.WriteLine("VerificarUtilizadorSistemaTodos()");

            // verificar utilizadores do sistema
            VerificarUtilizadorSistema(2, "sysadmin", "sysadmin", 5);
            VerificarUtilizadorSistema(0, "utente", "utente", 0);
            VerificarUtilizadorSistema(1, "acompanhante", "acompanhante", 1);
        }
        
        static void VerificarCriarUserTesteTodos()
        {
            Console.WriteLine("VerificarCriarUserTesteTodos()");
            
            // verificar os utilizadores dos diferentes perfis para testes
            VerificarCriarUserTeste("triagemtest", 2);
            VerificarCriarUserTeste("enfermeirotest", 3);
            VerificarCriarUserTeste("medicotest", 4);
            VerificarCriarUserTeste("sysadmintest", 5);
        }
        
        static void VerificarCriarCoresTriagemTodos()
        {
            Console.WriteLine("VerificarCriarCoresTriagemTodos()");

            // verificar a lista de cores de triagem
            VerificarCriarCoresTriagem(1, "DOENTE EMERGENTE", "#e2112e");
            VerificarCriarCoresTriagem(2, "DOENTE MUITO URGENTE", "#f39433");
            VerificarCriarCoresTriagem(3, "DOENTE URGENTE", "#f7db35");
            VerificarCriarCoresTriagem(4, "DOENTE POUCO URGENTE", "#3eab62");
            VerificarCriarCoresTriagem(5, "DOENTE NÃO URGENTE", "#3d99d5");
        }
        
        static void VerificarItemFluxoManchesterTodos()
        {
            Console.WriteLine($"VerificarItemFluxoManchesterTodos()");

            // verificar lista de ItensFluxoManchester
            // vermelho
            VerificarItemFluxoManchester("Compromisso da via aérea", 1);
            VerificarItemFluxoManchester("Respiração ineficaz", 1);
            VerificarItemFluxoManchester("Choque", 1);
            VerificarItemFluxoManchester("Criança que não responde", 1);
            VerificarItemFluxoManchester("Convulsão atual", 1);
            // laranja
            VerificarItemFluxoManchester("Grande hemorragia incontrolável", 2);
            VerificarItemFluxoManchester("Alteração do estado de consciência de novo", 2);
            VerificarItemFluxoManchester("Criança muito quente", 2);
            VerificarItemFluxoManchester("Adulto muito quente", 2);
            VerificarItemFluxoManchester("Dor severa", 2);
            VerificarItemFluxoManchester("Convulsão atual", 2);
            // amarelo
            VerificarItemFluxoManchester("Pequena hemorragia incontrolável", 3);
            VerificarItemFluxoManchester("História inapropriada", 3);
            VerificarItemFluxoManchester("Vómitos persistentes", 3);
            VerificarItemFluxoManchester("Criança quente", 3);
            VerificarItemFluxoManchester("Adulto quente", 3);
            VerificarItemFluxoManchester("Dor moderada", 3);
            // verde
            VerificarItemFluxoManchester("Subfebril (Febrícula)", 4);
            VerificarItemFluxoManchester("Vómitos", 4);
            VerificarItemFluxoManchester("Dor ligeira <7 dias", 4);
            VerificarItemFluxoManchester("Problema recente", 4);
        }
        
        static void VerificarItemFluxoManchester(string descricao, int classificacao)
        {           
            Console.WriteLine($"VerificarItemFluxoManchester({descricao}, {classificacao})");

            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) verificar o item de fluxo de manchester: \"{descricao}\"");
            appctrl.ItensFluxoManchester.CurrentObj = appctrl.ItensFluxoManchester.GetByDescricao(descricao);
            if (appctrl.ItensFluxoManchester.CurrentObj == null)
            {
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) o item \"{descricao}\" não existe na base de dados!");
                appctrl.ItensFluxoManchester.CurrentObj =
                    appctrl.ItensFluxoManchester.NewItemFluxoManchester(descricao, 
                        appctrl.CoresTriagem.GetById((ulong)classificacao), 
                        appctrl.Utilizadores.GetById(2));
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) o item \"{descricao}\" foi criado com o id {appctrl.ItensFluxoManchester.CurrentObj.IdItemFluxoManchester}");
            } else 
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) o item \"{descricao}\" já existe na base de dados com o id {appctrl.ItensFluxoManchester.CurrentObj.IdItemFluxoManchester}!");
        }
        
        static void VerificarUtilizadorSistema(ulong id, string nome, string password, int nivelacesso)
        {
            Console.WriteLine($"VerificarUtilizadorSistema({id}, {nome}, {password}, {nivelacesso})");
            
            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) verificar se existe o utilizador: {nome}");
            appctrl.Utilizadores.CurrentObj=appctrl.Utilizadores.GetById(id);
            if (appctrl.Utilizadores.CurrentObj == null)
            {
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) utilizador \"{nome}\" não existe na base de dados!");
                appctrl.Utilizadores.CurrentObj =
                    appctrl.Utilizadores.NewUtilizadorById(id, nome, password, nivelacesso, appctrl.Idiomas.GetDefaultIdioma(), appctrl.Utilizadores.GetById(2));
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) o utilizador \"{nome}\" foi criado com o id {appctrl.Utilizadores.CurrentObj.IdUtilizador}");
            } else 
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) o utilizador \"{nome}\" já existe na base de dados com o id {appctrl.Utilizadores.CurrentObj.IdUtilizador}!");
        }
        
        static void VerificarCriarCoresTriagem(ulong id, string descricao, string corhex)
        {
            Console.WriteLine($"VerificarCriarCoresTriagem({id}, {descricao}, {corhex})");
            
            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) verificar se existe objeto para: {descricao}");
            appctrl.CoresTriagem.CurrentObj=appctrl.CoresTriagem.GetById(id);
            if (appctrl.CoresTriagem.CurrentObj == null)
            {
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) CorTriagem \"{descricao}\" não existe na base de dados!");
                appctrl.CoresTriagem.CurrentObj =
                    appctrl.CoresTriagem.NewCorTriagem(id, descricao, corhex, appctrl.Utilizadores.GetById(2));
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) objeto CorTriagem \"{descricao}\" foi criado com o id {appctrl.CoresTriagem.CurrentObj.IdCorTriagem}");
            } else 
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) objeto CorTriagem \"{descricao}\" já existe na base de dados com o id {appctrl.CoresTriagem.CurrentObj.IdCorTriagem}!");
        }
        
        static void VerificarCriarUserTeste(string uTemp, int uNivelAcesso)
        {
            Console.WriteLine($"VerificarCriarUserTeste({uTemp}, {uNivelAcesso})");
            
            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) verificar se existe o utilizador: {uTemp}");
            appctrl.Utilizadores.CurrentObj=appctrl.Utilizadores.GetByNome(uTemp);
            if (appctrl.Utilizadores.CurrentObj == null)
            {
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) utilizador \"{uTemp}\" não existe na base de dados!");
                appctrl.Utilizadores.CurrentObj =
                    appctrl.Utilizadores.NewUtilizador(uTemp, uTemp, uNivelAcesso, appctrl.Idiomas.GetDefaultIdioma(), appctrl.Utilizadores.GetById(2));
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) o utilizador \"{uTemp}\" foi criado com o id {appctrl.Utilizadores.CurrentObj.IdUtilizador}");
            } else 
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) o utilizador \"{uTemp}\" já existe na base de dados com o id {appctrl.Utilizadores.CurrentObj.IdUtilizador}!");
        }
        
        static void VerificarCriarEpisodioTesteTodos()
        {
            Console.WriteLine("VerificarCriarEpisodioTesteTodos()");
            // verificar os utilizadores dos diferentes perfis para testes
            VerificarCriarEpisodioTeste("EP00112233");
            VerificarCriarEpisodioTeste("9988-AABB");
        }
        
        static void VerificarCriarEpisodioTeste(string eTemp)
        {
            Console.WriteLine($"VerificarCriarEpisodioTeste({eTemp})");
            
            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) verificar se existe o episodio: {eTemp}");
            appctrl.Episodios.CurrentObj = appctrl.Episodios.GetByCodEpisodio(eTemp);
            if (appctrl.Episodios.CurrentObj == null)
            {
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) episodio \"{eTemp}\" não existe na base de dados!");
                appctrl.CoresTriagem.CurrentObj = appctrl.CoresTriagem.GetById(5);
                appctrl.Idiomas.CurrentObj = appctrl.Idiomas.GetDefaultIdioma(appctrl.Utilizadores.GetById(2));
                appctrl.Nacionalidades.CurrentObj = appctrl.Nacionalidades.GetDefaultNacionalidade(appctrl.Utilizadores.GetById(2));
                if (!ReferenceEquals(appctrl.CoresTriagem.CurrentObj, null) &&
                    !ReferenceEquals(appctrl.Idiomas.CurrentObj, null) &&
                    !ReferenceEquals(appctrl.Nacionalidades.CurrentObj, null))
                {
                    appctrl.Episodios.CurrentObj = appctrl.Episodios.NewEpisodio(eTemp,
                        $"Teste {eTemp},{appctrl.CoresTriagem.CurrentObj.Descricao}", DateTime.Now,
                        "123456789", 0, "estado 0(zero)", 
                        appctrl.CoresTriagem.CurrentObj, 
                        appctrl.Idiomas.CurrentObj, appctrl.Nacionalidades.CurrentObj, 
                        appctrl.Utilizadores.GetById(2));
                    Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) o episódio \"{eTemp}\" foi criado com o id {appctrl.Episodios.CurrentObj.IdEpisodio}");
                }
                else
                    Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) ALERTA: não foi possível criar o episódio \"{eTemp}\" porque faltam pré-requisitos na BD, VOLTE a EXECUTAR este PROGRAMA !");
            } else 
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) o episódio \"{eTemp}\" já existe com o id {appctrl.Episodios.CurrentObj.IdEpisodio}");
        }
        
        static void TestarUserTeste()
        {
            Console.WriteLine("TestarUserTeste()");
            
            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) testar login");
            if (appctrl.Login("teste", "teste"))
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) o id recebido foi o {appctrl.Utilizadores.UtilizadorLoggedInId}");
            else
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) falhou o login!...");
        }
        
        static void ShowInfo()
        {
            Console.WriteLine("ShowInfo()");
            
            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) Hello World!");
            Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) DbConnection(): {appctrl.DbConnection}");
            Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) JwtSecret(): {appctrl.JwtSecret}");
            Thread.Sleep(10);
        }
        
        static void Show10IDs()
        {
            Console.WriteLine("Show10IDs");
            
            for(int i=0; i<10; i++)
            {
                Console.WriteLine($"(Thread:{Thread.CurrentThread.ManagedThreadId}) novo ID({i}): {IdTools.IdGenerate()}");
                Thread.Sleep(10);
            }
        }
    }
}