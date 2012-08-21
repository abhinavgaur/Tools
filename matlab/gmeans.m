function [L C] = gmeans(datafile, clusters)
if ~ischar(datafile),
    save('~gmeans.prep', 'datafile', '-ascii');
    datafile='~gmeans.prep';
end;
tool_bin = '..\..\tools\bin\';
system([tool_bin 'LDAInputConverter.exe lsa2gmeans "' datafile '" "~gmeans.txt" > nul']);
system([tool_bin 'gmeans.exe -F t -c ' num2str(clusters) ' "~gmeans.txt" > nul']);
data=importdata(['~gmeans.txt_doctoclus.' num2str(clusters)]);
n=data(1);
L=data(2:1+n);
m=data(2+n);
data=data(3+n:size(data,1));
C=reshape(data,size(data,1)/m,m);
!del ~* > nul