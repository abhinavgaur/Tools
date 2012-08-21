@echo off
LDAInputConverter lsa2lda2 ..\..\Data\%1\%1-%2WF.txt ..\..\Data\%1\%1-%2LDA.txt
lda est 0.1 %3 settings.txt ..\..\Data\%1\%1-%2LDA.txt random ..\..\Data\%1\%1-%2WF\
lda inf settings.txt ..\..\Data\%1\%1-%2WF\final ..\..\Data\%1\%1-%2LDA.txt price