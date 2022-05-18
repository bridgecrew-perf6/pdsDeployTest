
CREATE SCHEMA IF NOT EXISTS osler;

CREATE TABLE IF NOT EXISTS osler.Utilizador (
  idUtilizador  bigint NOT NULL, 
  idIdioma      bigint NOT NULL, 
  nome          varchar(80) NOT NULL, 
  password      varchar(255) NOT NULL, 
  nivelAcesso   int2 DEFAULT 0 NOT NULL, 
  ativo         bool DEFAULT 'true' NOT NULL,
  criadoPor     bigint, 
  criadoEm      timestamp, 
  modificadoPor bigint, 
  modificadoEm  timestamp, 
  PRIMARY KEY (idUtilizador));
  
CREATE TABLE IF NOT EXISTS osler.Idioma (
  idIdioma      bigint NOT NULL, 
  descricao     varchar(100) NOT NULL, 
  criadoPor     bigint, 
  criadoEm      timestamp, 
  modificadoPor bigint, 
  modificadoEm  timestamp, 
  PRIMARY KEY (idIdioma));

CREATE TABLE IF NOT EXISTS osler.Episodio (
  idEpisodio      bigint NOT NULL, 
  idCorTriagem    bigint NOT NULL, 
  idNacionalidade bigint NOT NULL, 
  idIdioma        bigint NOT NULL, 
  idSNS           varchar(20), 
  estado          int2 DEFAULT 0 NOT NULL, 
  dataAberto      timestamp NOT NULL, 
  dataFechado     timestamp, 
  criadoPor       bigint, 
  criadoEm        timestamp, 
  modificadoPor   bigint, 
  modificadoEm    timestamp, 
  PRIMARY KEY (idEpisodio));
  
CREATE TABLE IF NOT EXISTS osler.Local (
  idLocal       bigint NOT NULL, 
  descricao     varchar(255) NOT NULL, 
  criadoPor     bigint, 
  criadoEm      timestamp, 
  modificadoPor bigint, 
  modificadoEm  timestamp, 
  PRIMARY KEY (idLocal));
  
CREATE TABLE IF NOT EXISTS osler.Nacionalidade (
  idNacionalidade bigint NOT NULL, 
  idIdioma        bigint NOT NULL, 
  descricao       varchar(100) NOT NULL, 
  criadoPor       bigint, 
  criadoEm        timestamp, 
  modificadoPor   bigint, 
  modificadoEm    timestamp, 
  PRIMARY KEY (idNacionalidade));
  
CREATE TABLE IF NOT EXISTS osler.CorTriagem (
  idCorTriagem  bigint NOT NULL, 
  descricao     varchar(50) NOT NULL, 
  codigoCorHex  varchar(50) NOT NULL, 
  criadoPor     bigint, 
  criadoEm      timestamp, 
  modificadoPor bigint, 
  modificadoEm  timestamp, 
  PRIMARY KEY (idCorTriagem));
  
CREATE TABLE IF NOT EXISTS osler.Questionario (
  idQuestionario bigint NOT NULL, 
  idIdioma       bigint NOT NULL, 
  descricao      varchar(255) NOT NULL, 
  criadoPor      bigint, 
  criadoEm       timestamp, 
  modificadoPor  bigint, 
  modificadoEm   timestamp, 
  PRIMARY KEY (idQuestionario));
  
CREATE TABLE IF NOT EXISTS osler.EpisodioQuestionario (
  idEpisodioQuestionario bigint NOT NULL, 
  idEpisodio             bigint NOT NULL, 
  idQuestionario         bigint NOT NULL, 
  sequenciaQuestionario  int2 DEFAULT 1 NOT NULL, 
  criadoPor              bigint, 
  criadoEm               timestamp, 
  modificadoPor          bigint, 
  modificadoEm           timestamp, 
  PRIMARY KEY (idEpisodioQuestionario));
  
CREATE TABLE IF NOT EXISTS osler.Pergunta (
  idPergunta        bigint NOT NULL, 
  idQuestionario    bigint NOT NULL, 
  sequenciaPergunta int2 DEFAULT 1 NOT NULL, 
  textoPergunta     varchar(255) NOT NULL, 
  criadoPor         bigint, 
  criadoEm          timestamp, 
  modificadoPor     bigint, 
  modificadoEm      timestamp, 
  PRIMARY KEY (idPergunta));
  
CREATE TABLE IF NOT EXISTS osler.LoginRegisto (
  idLogin         bigint NOT NULL, 
  idUtilizador    bigint NOT NULL, 
  idEpisodio      bigint NOT NULL, 
  dataHora        timestamp NOT NULL, 
  token           varchar(50) NOT NULL, 
  validadeToken   timestamp NOT NULL, 
  ativo           bool DEFAULT 'true' NOT NULL, 
  infoDispositivo varchar(50), 
  criadoPor       bigint, 
  criadoEm        timestamp, 
  modificadoPor   bigint, 
  modificadoEm    timestamp, 
  PRIMARY KEY (idLogin));
  
CREATE TABLE IF NOT EXISTS osler.EpisodioQuestResposta (
  idEpisodioQuestionario bigint NOT NULL, 
  sequenciaResposta      int2 DEFAULT 1 NOT NULL, 
  idQuestionario         bigint NOT NULL, 
  idPergunta             bigint NOT NULL, 
  resposta               varchar(255) NOT NULL, 
  ativo                  bool DEFAULT 'true' NOT NULL, 
  criadoPor              bigint, 
  criadoEm               timestamp, 
  modificadoPor          bigint, 
  modificadoEm           timestamp, 
  PRIMARY KEY (idEpisodioQuestionario, 
  sequenciaResposta));
  
CREATE TABLE IF NOT EXISTS osler.EpisodioHistLocal (
  idHistoricoLocal bigint NOT NULL, 
  idLocal          bigint NOT NULL, 
  idEpisodio       bigint NOT NULL, 
  dataHora         timestamp NOT NULL, 
  criadoPor        bigint, 
  criadoEm         timestamp, 
  modificadoPor    bigint, 
  modificadoEm     timestamp, 
  PRIMARY KEY (idHistoricoLocal));
  
CREATE TABLE IF NOT EXISTS osler.EpisodioNotificacao (
  idNotificacao   bigint NOT NULL, 
  idEpisodio      bigint NOT NULL, 
  dataHora        timestamp NOT NULL, 
  mensagem        varchar(255) NOT NULL, 
  dataHoraLeitura timestamp, 
  criadoPor       bigint, 
  criadoEm        timestamp, 
  modificadoPor   bigint, 
  modificadoEm    timestamp, 
  PRIMARY KEY (idNotificacao));
  
CREATE TABLE IF NOT EXISTS osler.EpisodioRegistoDados (
  idRegistoDados bigint NOT NULL, 
  idEpisodio     bigint NOT NULL, 
  idTipoLeitura  bigint NOT NULL, 
  dataHora       timestamp NOT NULL, 
  valor          varchar(50) NOT NULL, 
  criadoPor      bigint, 
  criadoEm       timestamp, 
  modificadoPor  bigint, 
  modificadoEm   timestamp, 
  PRIMARY KEY (idRegistoDados));
  
CREATE TABLE IF NOT EXISTS osler.TipoLeitura (
  idTipoLeitura bigint NOT NULL, 
  descricao     varchar(255) NOT NULL, 
  medida        varchar(50), 
  criadoPor     bigint, 
  criadoEm      timestamp, 
  modificadoPor bigint, 
  modificadoEm  timestamp, 
  PRIMARY KEY (idTipoLeitura));
  
CREATE INDEX IF NOT EXISTS LoginRegisto_token ON osler.LoginRegisto (token);
  
ALTER TABLE IF EXISTS osler.Utilizador ADD CONSTRAINT FKUtilizador909733 FOREIGN KEY (idIdioma) REFERENCES osler.Idioma (idIdioma);
ALTER TABLE IF EXISTS osler.Episodio ADD CONSTRAINT FKEpisodio654607 FOREIGN KEY (idIdioma) REFERENCES osler.Idioma (idIdioma);
ALTER TABLE IF EXISTS osler.Nacionalidade ADD CONSTRAINT FKNacionalid695798 FOREIGN KEY (idIdioma) REFERENCES osler.Idioma (idIdioma);
ALTER TABLE IF EXISTS osler.Episodio ADD CONSTRAINT FKEpisodio631889 FOREIGN KEY (idNacionalidade) REFERENCES osler.Nacionalidade (idNacionalidade);
ALTER TABLE IF EXISTS osler.Episodio ADD CONSTRAINT FKEpisodio252970 FOREIGN KEY (idCorTriagem) REFERENCES osler.CorTriagem (idCorTriagem);
ALTER TABLE IF EXISTS osler.Questionario ADD CONSTRAINT FKQuestionar241870 FOREIGN KEY (idIdioma) REFERENCES osler.Idioma (idIdioma);
ALTER TABLE IF EXISTS osler.EpisodioQuestionario ADD CONSTRAINT FKEpisodioQu189786 FOREIGN KEY (idEpisodio) REFERENCES osler.Episodio (idEpisodio);
ALTER TABLE IF EXISTS osler.EpisodioQuestionario ADD CONSTRAINT FKEpisodioQu475963 FOREIGN KEY (idQuestionario) REFERENCES osler.Questionario (idQuestionario);
ALTER TABLE IF EXISTS osler.Pergunta ADD CONSTRAINT FKPergunta469295 FOREIGN KEY (idQuestionario) REFERENCES osler.Questionario (idQuestionario);
ALTER TABLE IF EXISTS osler.LoginRegisto ADD CONSTRAINT FKLoginRegis224005 FOREIGN KEY (idUtilizador) REFERENCES osler.Utilizador (idUtilizador);
ALTER TABLE IF EXISTS osler.LoginRegisto ADD CONSTRAINT FKLoginRegis701700 FOREIGN KEY (idEpisodio) REFERENCES osler.Episodio (idEpisodio);
ALTER TABLE IF EXISTS osler.EpisodioQuestResposta ADD CONSTRAINT FKEpisodioQu895511 FOREIGN KEY (idEpisodioQuestionario) REFERENCES osler.EpisodioQuestionario (idEpisodioQuestionario);
ALTER TABLE IF EXISTS osler.EpisodioQuestResposta ADD CONSTRAINT FKEpisodioQu555319 FOREIGN KEY (idQuestionario) REFERENCES osler.Questionario (idQuestionario);
ALTER TABLE IF EXISTS osler.EpisodioQuestResposta ADD CONSTRAINT FKEpisodioQu874849 FOREIGN KEY (idPergunta) REFERENCES osler.Pergunta (idPergunta);
ALTER TABLE IF EXISTS osler.EpisodioHistLocal ADD CONSTRAINT FKEpisodioHi910423 FOREIGN KEY (idLocal) REFERENCES osler.Local (idLocal);
ALTER TABLE IF EXISTS osler.EpisodioHistLocal ADD CONSTRAINT FKEpisodioHi471782 FOREIGN KEY (idEpisodio) REFERENCES osler.Episodio (idEpisodio);
ALTER TABLE IF EXISTS osler.EpisodioNotificacao ADD CONSTRAINT FKEpisodioNo635650 FOREIGN KEY (idEpisodio) REFERENCES osler.Episodio (idEpisodio);
ALTER TABLE IF EXISTS osler.EpisodioRegistoDados ADD CONSTRAINT FKEpisodioRe635260 FOREIGN KEY (idEpisodio) REFERENCES osler.Episodio (idEpisodio);
ALTER TABLE IF EXISTS osler.EpisodioRegistoDados ADD CONSTRAINT FKEpisodioRe52333 FOREIGN KEY (idTipoLeitura) REFERENCES osler.TipoLeitura (idTipoLeitura);

