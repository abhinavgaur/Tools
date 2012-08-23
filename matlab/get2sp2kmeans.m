function [g A] = get2sp2kmeans(set, year, n, lambda)
if (nargin < 4), lambda=1; end;
fprintf('getkmeans(''%s'', ''%s'', %d);\n', set, year, n);
kmeansRunTimes=10;

f1=load(['../../Data/' set '/' set '-' year 'WF.txt']); f2=load(['../../Data/' set '/' set 'KFQ.txt']);
f1=normalize(f1')'; f2=normalize(f2')';

[l1 c1]=spkmeans(f1,n);
[l2 c2]=spkmeans(f2,n);

%[l1 c1]=spectral_clustering(f1,n);

E=zeros(size(f1,2), size(f2,2));
for i=1:size(f1,1), E=E+f1(i,:)'*f2(i,:); end; E=E/size(f1,1);

%An=pinv(c1')*E*pinv(c2);
An=lsqSolveA(c1,c2,E);
An=An.*(An>0);
%disp(An);

Anp=normalize(An')';
Anq=normalize(An)';
ll1=zeros(size(l1,1),n); ll2=zeros(size(l1,1),n);
for i=1:size(l1,1), ll1(i,:)=(f1(i,:)*c1'); end;
for i=1:size(l2,1), ll2(i,:)=(f2(i,:)*c2'); end;
ll1=normalize(ll1')'; ll2=normalize(ll2')';

for krun = 1 : kmeansRunTimes
    v = +inf; vv = v; g = []; gg = [];
    try
        %gg = spectral_clustering(ll1*lambda+ll2*Anq,n);
        %gg = spectral_clustering(max(ll1, ll2*Anq),n);
        gg = kmeans(max(ll1, ll2*Anq),n);
        vv = scoreresult(gg, n);
    catch
    end;
    if v>vv, g=gg; v=vv; end;
end;
disp(g);