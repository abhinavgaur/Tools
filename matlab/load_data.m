f1=load(['../../Data/' set '/' set '-' year 'PZ.txt']); f2=load(['../../Data/' set '/' set 'KFQ.txt']);
f1=normalize(f1')'; f2=normalize(f2')';

beta=load(['../../Data/' set '/' set '-' year 'PD/final.beta']); beta=exp(beta);
beta2=load(['../../Data/' set '/' set 'IDK/final.beta']); beta2=exp(beta2);
beta=[beta zeros(size(beta,1), size(f1,2)-size(beta,2))];
beta2=[beta2 zeros(size(beta2,1), size(f2,2)-size(beta2,2))];

gamma=normalize(load(['../../Data/' set '/' set '-' year 'WF/final.gamma'])')';
gamma2=normalize(load(['../../Data/' set '/' set 'IDK/final.gamma'])')';

E=zeros(size(f1,2), size(f2,2));
for i=1:size(f1,1), E=E+f1(i,:)'*f2(i,:); end; E=E/size(f1,1);

A=pinv(beta')*E*pinv(beta2);
%A=lsqSolveA(beta,beta2,E);
Ap=normalize(A')';
%disp(Ap);
