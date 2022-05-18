using System;
using System.Threading;
using NUnit.Framework;
using WebAppOSLERLib.CTRL;

namespace WebAppOSLERLib.UnitTest
{
    public class OslerTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void TestIdioma()
        {
            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine("Testar(ORM): Idioma (Thread:{0})", Thread.CurrentThread.ManagedThreadId);
            //Console.WriteLine("Ligação:");
            //Console.WriteLine(AppCtrl.Db.ConnectionString);
            
            Console.WriteLine("Testar: o idioma 'português' (é o idioma por omissão");
            Assert.AreEqual(appctrl.Idiomas.GetDefaultIdioma().DefaultId, appctrl.Idiomas.GetDefaultIdioma().IdIdioma);
        }
        
        [Test]
        public void TestLocal()
        {
            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine("Testar(ORM): Local (Thread:{0})", Thread.CurrentThread.ManagedThreadId);
            //Console.WriteLine("Ligação:");
            //Console.WriteLine(AppCtrl.Db.ConnectionString);
            
            Console.WriteLine("Testar: o local 'Sala de espera' (é o local por omissão");
            Assert.AreEqual(  appctrl.Locais.GetDefaultLocal().DefaultId, appctrl.Locais.GetDefaultLocal().IdLocal);
        }
        
        [Test]
        public void TestNacionalidade()
        {
            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine("Testar(ORM): Nacionalidade (Thread:{0})", Thread.CurrentThread.ManagedThreadId);
            // Console.WriteLine("Ligação:");
            // Console.WriteLine(AppCtrl.Db.ConnectionString);
            
            Console.WriteLine("Testar: a nacionalidade 'Sala de espera' (é a nacionalidade por omissão)");
            Assert.AreEqual(appctrl.Nacionalidades.GetDefaultNacionalidade().DefaultId, appctrl.Nacionalidades.GetDefaultNacionalidade().IdNacionalidade);
        }
        
        [Test]
        public void TestLogin()
        {
            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine("Testar(ORM): Login (Thread:{0})", Thread.CurrentThread.ManagedThreadId);
            // Console.WriteLine("Ligação:");
            // Console.WriteLine(AppCtrl.Db.ConnectionString);
            
            // Idioma idiomaTeste = AppCtrl.LoadDefaultIdioma();
            const string nome = "sysadmintest";
            const string password = "sysadmintest";
            //     
            // var utilizador = new Utilizador(nome, password, 0, idiomaTeste);
            Console.WriteLine("TestLogin(): login do utilizador... (Thread:{0})", Thread.CurrentThread.ManagedThreadId);
            Assert.AreEqual(true, appctrl.Login(nome, password));
        }
        
        
        [Test]
        public void TestLogin2()
        {
            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine("Testar(ORM): Login2 (Thread:{0})", Thread.CurrentThread.ManagedThreadId);
            // Console.WriteLine("Ligação:");
            // Console.WriteLine(AppCtrl.Db.ConnectionString);
            
            // Idioma idiomaTeste = AppCtrl.LoadDefaultIdioma();
            const string nome = "sysadmintest";
            const string password = "sysadmintest";
            //     
            // var utilizador = new Utilizador(nome, password, 0, idiomaTeste);
            Console.WriteLine("TestLogin2(): login do utilizador... (Thread:{0})", Thread.CurrentThread.ManagedThreadId);
            //Assert.AreEqual(null, AppCtrl.Utilizadores.UtilizadorLoggedInToken);
            Assert.AreEqual(true, appctrl.Login(nome, password));
        }
        
        [Test]
        public void TestCreateUtilizador()
        {
            AppCtrl appctrl = new AppCtrl();
            Console.WriteLine("Testar(ORM): Utilizador (Thread:{0})", Thread.CurrentThread.ManagedThreadId);
            // Console.WriteLine("Ligação:");
            // Console.WriteLine(AppCtrl.Db.ConnectionString);
            
            // Idioma idiomaTeste = AppCtrl.LoadDefaultIdioma();
            Random rnd = new Random();
            string nome = $"Teste{rnd.Next(99999999)}";
            const string password = "123";
            //     
            // var utilizador = new Utilizador(nome, password, 0, idiomaTeste);

            Console.WriteLine($"Testar: criar utilizador {nome}... (Thread:{0})", Thread.CurrentThread.ManagedThreadId);
            appctrl.Utilizadores.CurrentObj = appctrl.Utilizadores.NewUtilizador(nome, password, 0);
            Assert.AreEqual(appctrl.Utilizadores.CurrentObj.IdUtilizador, appctrl.Utilizadores.GetByNome(nome).IdUtilizador);
            // Assert.AreEqual(true,AppCtrl.Utilizadores.SaveObj(AppCtrl.Utilizadores.CurrentObj);
            //  AppCtrl.Utilizadores.Login(nome, password, ref id));
        }
        
    }
}