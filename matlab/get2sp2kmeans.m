function [g A] = get2sp2kmeans(set, year, n)
fprintf('getkmeans(''%s'', ''%s'', %d);\n', set, year, n);
kmeansRunTimes=10;

f1=load(['../../Data/' set '/' set '-' year 'WF.txt']); f2=load(['../../Data/' set '/' set 'KFQ.txt']);
f1=normalize(f1')'; f2=normalize(f2')';

[l1 c1]=gmeans(f1,n);
[l2 c2]=gmeans(f2,n);

E=zeros(size(f1,2), size(f2,2));
for i=1:size(f1,1), E=E+f1(i,:)'*f2(i,:); end; E=E/size(f1,1);
c1=c1'; c2=c2';

An=pinv(c1')*E*pinv(c2);
An=An.*(An>0);

Anp=normalize(An')';
Anq=normalize(An)';
ll1=zeros(size(l1,1),n); ll2=zeros(size(l1,1),n);
for i=1:size(l1,1), ll1(i,l1(i))=1; end;
for i=1:size(l1,1), ll2(i,l2(i))=1; end;

for krun = 1 : kmeansRunTimes
    v = +inf; vv = v; g = []; gg = [];
    try
        gg = kmeans(ll1*Anp+ll2*Anq,n);
        vv = scoreresult(gg, n);
    catch
    end;
    if v>vv, g=gg; v=vv; end;
end;
disp(g);