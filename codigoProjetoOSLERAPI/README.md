  
# milestone 2 vAlfa  
  
## codigoProjetoOSLERAPI  
  
espaço destinado para o código do API do projetoOSLER que também irá usar codigoProjetoOSLERLIB 
  
## key para JWT  
  
exemplo:  
string key = AppCtrl.JwtSecret();  
  
## URL para acesso feito pelo front-end  
  
/OSLER/Login  
/OSLER/Login/Utente  
/OSLER/Login/Acompanhante  
/OSLER/Logout  
/OSLER/Enfermagem  
/OSLER/Medico  
/OSLER/Local/{id} [get|post]
/OSLER/Local/Novo [post]
/OSLER/Local/Lista [get]
/OSLER/Idioma/{id} [get|post]
/OSLER/Idioma/Novo [post]
/OSLER/Idioma/Lista [get]
/OSLER/Nacionalidade/{id} [get|post]
/OSLER/Nacionalidade/Novo [post]
/OSLER/Nacionalidade/Lista [get]
/OSLER/TipoLeitura/{id} [get|post]
/OSLER/TipoLeitura/Novo [post]
/OSLER/TipoLeitura/Lista [get]
/OSLER/CorTriagem/Lista [get]
/**
OSLER/ItemFluxoManchester/{id} [get|post]
OSLER/ItemFluxoManchester/novo [post]
OSLER/ItemFluxoManchester/Lista [get]

OSLER/Questionario/{id} [get|post]
OSLER/Questionario/Novo [post]
OSLER/Questionario/Lista [get]

OSLER/Utilizador/{id} [get|post]
OSLER/Utilizador/Novo [post]
OSLER/Utilizador/ListaNiveisAcesso [get]

/OSLER/Episodio/Novo [post]
/OSLER/Episodio/{idEpisodio}/mudarEstado [post]
/OSLER/Episodio/Historico/{SnSUtente} [get]
/OSLER/Episodio/{idEpisodio}/Local [get|post]
/OSLER/Episodio/{idEpisodio}/Dados [post|get]
/OSLER/Episodio/{idEpisodio}/Questionario/Add [post]
/OSLER/Episodio/{idEpisodio}/Questionario/List [get]
/OSLER/Episodio/{idEpisodio}/Questionario/Responder [post]
/OSLER/Episodio/{idEpisodio}/Respostas [get]

---
/OSLER/Episodio/{idEpisodio}/Questionario/AlterarResposta [Patch]

/OSLER/Enfermeiro/DadosUtente [get]

OSLER/Utente/DadosPessoais [get]