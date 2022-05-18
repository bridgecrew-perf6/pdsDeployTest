/*
*	<description>DataConfig, objeto da camada "Tools" responsável por carregar os dados de configuração do ficheiro XML</description>
* 	<author>João Carlos Pinto</author>
*   <date>04-04-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.IO;
using System.Xml.Serialization;

namespace WebAppOSLERLib.Tools
{
    /// <summary>
    /// estrutura com as informações gravadas em ficheiro XML
    /// </summary>
    [Serializable]
    public struct EstruturaDadosXml{
        [XmlElement]
        public string NpgsqlConnection;
        [XmlElement]
        public string JWTSecret;
        [XmlElement]
        public string CORSList;
        public EstruturaDadosXml(string npgsqlConnection, string jwtsecret, string corslist)
        {
            NpgsqlConnection = npgsqlConnection;
            JWTSecret = jwtsecret;
            CORSList = corslist;
        }
    }

    /// <summary>
    /// classe responsável por fornecer as informações de configuração do programa
    /// </summary>
    public sealed class DataConfig
    {
        private static bool _inicializado = false;
        private static object _block = new object();
        private static EstruturaDadosXml _config;
        private void LoadDados(string fileName)
        {
            if (!_inicializado)
            {
                lock (_block)
                {
                    if (!_inicializado)
                    {
                        if (File.Exists(@fileName)) {
                            try {
                                Stream stream = File.Open(fileName, FileMode.Open);
                                XmlSerializer xs = new XmlSerializer(_config.GetType(), new []{typeof(EstruturaDadosXml)});
                                _config = (EstruturaDadosXml)xs.Deserialize(stream)!;
                                stream.Dispose();
                                _inicializado = true;
                            } catch (IOException e) {
                                throw new MyException($"ERRO: ler do ficheiro ({fileName})", e);
                            }
                        } else 
                            throw new MyException($"ERRO: ficheiro ({fileName}) não existe!");
                    }
                }
            }
        }
        /// <summary>
        /// construtor da instância
        /// </summary>
        /// <exception cref="MyException"></exception>
        public DataConfig()
        {
            string ficheiroConfig = Directory.GetCurrentDirectory()+Path.DirectorySeparatorChar+"cfg.xml";
            try
            {
                LoadDados(ficheiroConfig);
            }
            catch (Exception e)
            {
                throw new MyException($"LoadDataConfig(), erreur catastrophique! ({ficheiroConfig})", e);
            }
        }
        /// <summary>
        /// string de ligação
        /// </summary>
        public string DbConnectString => _config.NpgsqlConnection;
        /// <summary>
        /// JWT secret 
        /// </summary>
        public string JwtSecret => _config.JWTSecret;
        public string CORSList => _config.CORSList;
        /// <summary>
        /// fornece a instância única do processo de execução
        /// </summary>
        public static DataConfig Instancia => new DataConfig();
    }
}