function simiR=createSimiR(I)
D = squareform( pdist(I) );       %'# euclidean distance between columns of I
simiR = exp(-(1/10) * D);              %# similarity matrix between columns of I
