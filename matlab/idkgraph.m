f=fopen(['..\..\data\' set '\' set 'IDK.txt'], 'rb', 'n', 'utf-8');
p=[];while (~feof(f)), s=fgets(f); parts=regexp(s, '[\t, ]', 'split');
p=[p; {parts}]; end;
fclose(f);
M=zeros(size(p,1),size(p,1));
v={}; b={};
for i=1:size(p,1), b=union(b, p{i}); end;
for i=1:size(b,2), v{i}=i; end;
W=zeros(size(b,2), size(b,2));
bdict = containers.Map(b, v);
q=p;
for i=1:size(p,1), for j=2:size(p{i},1), q{i}{j}=bdict(p{i}{j}); end; end;
for i=1:size(p,1), for j=2:size(q{i},1) q{i}{j}=char(q{i}{j});end;end; 
for i=1:size(p,1), 
    for j=1:i, 
        %M(i,j)=size(intersect(p{i},p{j}),2);
        M(i, j) = 0; M(j, i) = 0;
        for k=1:size(p{i},2),
            for l=k:size(p{j},2),
                if strcmp(p{i}{k},p{j}{l}), M(i,j)=M(i,j)+1/k; M(j,i)=M(j,i)+1/l; end;
            end;
        end;
    end;
    % disp(i);
end;

for i=1:size(p,1),
    for j=1:size(p{i},2),
        for k=1:size(p{i},2),
            W(bdict(p{i}{j}),bdict(p{i}{k}))=W(bdict(p{i}{j}),bdict(p{i}{k}))+1;
        end;
    end;
end;

%N=M'; diag(diag(1./N))*N;
%M=diag(diag(1./M))*M;
%N=M+N-diag(diag(N)) % 因为是无向图
N=M;