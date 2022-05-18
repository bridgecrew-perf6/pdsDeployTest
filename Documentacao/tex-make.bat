pdflatex projetoOSLER_PDS -enable-installer -verbose
bibtex projetoOSLER_PDS -enable-installer -verbose
makeindex projetoOSLER_PDS.glo -s projetoOSLER_PDS.ist -t projetoOSLER_PDS.glg -o projetoOSLER_PDS.gls
makeindex projetoOSLER_PDS.acn -s projetoOSLER_PDS.ist -t projetoOSLER_PDS.alg -o projetoOSLER_PDS.acr
pdflatex projetoOSLER_PDS -enable-installer -verbose
pdflatex projetoOSLER_PDS -enable-installer -verbose
