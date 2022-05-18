  
# milestone 2 vAlfa  
  
## material de apoio BD  
  
### adicionar campos na tabela osler.episodio  
  
* adicionar descrição (12-abr-2022)  
  
alter table osler.episodio  
add column descricao varchar(100);  
  
* adicionar codepisodiotxt, dtnascimento, pin4, estadotxt (13-abr-2022)  
  
alter table osler.episodio  
add column codepisodiotxt varchar(30) not null,  
add column dtnascimento date not null,  
add column pin4 smallint not null,  
add column estadotxt varchar(200);  
  
### criar tabela: ItemFluxoManchester  
  
CREATE TABLE IF NOT EXISTS osler.itemfluxomanchester  
(iditemfluxomanchester bigint NOT NULL,  
idcortriagem bigint NOT NULL,  
descricao varchar(200) NOT NULL,  
criadopor bigint,  
criadoem timestamp without time zone,  
modificadopor bigint,  
modificadoem timestamp without time zone,  
CONSTRAINT itemfluxomanchester_pkey PRIMARY KEY (iditemfluxomanchester),  
CONSTRAINT fkitemfluxomanchester695798 FOREIGN KEY (idcortriagem)  
REFERENCES osler.cortriagem (idcortriagem) MATCH SIMPLE  
ON UPDATE NO ACTION  
ON DELETE NO ACTION);  
  
### alterar tabela: loginregisto  
  
alter table osler.loginregisto  
drop column token;  
  
alter table osler.loginregisto  
add column tokenpayload varchar(250) not null;  
  
create index tokenpayload_index on osler.loginregisto (tokenpayload);  

### SQL para limpar dados: Testes .PlayLib  
  
delete from osler.loginregisto where idlogin>=0;
delete from osler.episodio where idepisodio>=0;
delete from osler.itemfluxomanchester where iditemfluxomanchester>=0;
delete from osler.cortriagem where idcortriagem>=0;
delete from osler.nacionalidade where idnacionalidade>=0;
delete from osler.utilizador where idutilizador>=0;
delete from osler.idioma where ididioma>=0;
delete from osler.local where idlocal>=0;
delete from osler.tipoleitura where idtipoleitura>=0;
delete from osler.pergunta where idpergunta>=0;
delete from osler.questionario where idquestionario>=0;


  