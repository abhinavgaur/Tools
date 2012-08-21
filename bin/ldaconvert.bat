@echo off
tfidf ..\..\Data\%1\%1IDK.txt ~keydict.txt tf
tfidf ~keydict.txt ..\..\Data\%1\%1IDK.txt ~fq.txt tf
LDAInputConverter lsa2lda2 ~fq.txt ~lda.txt
lda est 0.01 %2 settings.txt ~lda.txt random ..\..\Data\%1\%1IDK\
copy ~fq.txt ..\..\Data\%1\%1KFQ.txt
tfidf ..\..\Data\%1\%1IND.txt ~dict.txt tf
tfidf ~dict.txt ..\..\Data\%1\%1IND.txt ~fq2.txt tf
LDAInputConverter lsa2lda2 ~fq2.txt ~lda2.txt
lda est 0.01 %2 settings.txt ~lda2.txt random ..\..\Data\%1\%1IND\
copy ~fq2.txt ..\..\Data\%1\%1DFQ.txt
del ~*.txt
REM LDAInputConverter lsa2lda2 ..\..\Data\%1\%1-%2WF.txt ..\..\Data\%1\%1-%2LDA.txt
REM lda est 0.1 %3 settings.txt ..\..\Data\%1\%1-%2LDA.txt random ..\..\Data\%1\%1-%2WF\
REM lda inf settings.txt ..\..\Data\%1\%1-%2WF\final ..\..\Data\%1\%1-%2LDA.txt price
REM lda inf settings.txt ..\..\Data\steel-bank-pharm\industry\final ..\..\Data\steel-bank-pharm\industry_lda.txt industry
REM topics ..\..\Data\steel-bank-pharm\dict.txt ..\..\Data\steel-bank-pharm\industry\final.beta