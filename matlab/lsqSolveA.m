function A = lsqSolveA(P,Q,E)
A=zeros(size(P,1),size(Q,1));
%for i=1:size(E,2),
    %AQ(:,i)=lsqlin(P',E(:,i),[],[],[],[],zeros(size(P,1),1));
    %AQ(:,i)=lsqnonneg(P',E(:,i))';
%end;
AQ=pinv(P')*E;
for i=1:size(P,1),
    %A(i,:)=lsqlin(Q',AQ(i,:),[],[],[],[],zeros(size(Q,1),1));
    A(i,:)=lsqnonneg(Q',AQ(i,:)')';
end;
A=A./sum(sum(A));