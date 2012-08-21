function [g A] = get2spkmeans(set, year, n)
fprintf('getkmeans(''%s'', ''%s'', %d);\n', set, year, n);
kmeansRunTimes=10;

f1=load(['../' set '/' set '-' year 'WF.txt']); f2=load(['../' set '/' set 'KFQ.txt']);
f1=normalize(f1')'; f2=normalize(f2')';

beta=load(['../' set '/' set '-' year 'WF/final.beta']); beta=exp(beta);
beta2=load(['../' set '/' set 'IDK/final.beta']); beta2=exp(beta2);
beta=[beta zeros(size(beta,1), size(f1,2)-size(beta,2))];
beta2=[beta2 zeros(size(beta2,1), size(f2,2)-size(beta2,2))];

gamma=normalize(load(['../' set '/' set '-' year 'WF/final.gamma'])')';
gamma2=normalize(load(['../' set '/' set 'IDK/final.gamma'])')';

E=zeros(size(f1,2), size(f2,2));
for i=1:size(f1,1), E=E+f1(i,:)'*f2(i,:); end; E=E/size(f1,1);

A=pinv(beta')*E*pinv(beta2);
Ap=normalize(A');

for krun = 1 : kmeansRunTimes
    v = +inf; vv = v; g = []; gg = [];
    try
        gg = kmeans(gamma*Ap+gamma2,n);
        vv = scoreresult(gg, n);
    catch
    end;
    if v>vv, g=gg; v=vv; end;
end;
disp(g);