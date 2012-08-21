function g = pnz_income(set, year, topics)
tool_bin = 'D:\Documents\学校\作业\毕业设计\Tools\bin\';
data_filename = [tool_bin '..\..\Data\' set '\' set '-' year 'PD.txt'];
output_dir = [tool_bin '..\..\Data\' set '\' set '-' year 'PD\'];
if (exist('n') ~= 1), n = 1; end; % 切片长度
if (exist('m') ~= 1), m = 1; end; % 窗口长度

% 读取资料
p=load(data_filename)';
p=p(1:size(p,1)-1,:);
pn=[p(2:size(p,1),:);zeros(1,size(p,2))];
pr=log(pn./p);
pr(sum(abs(pr),2)==0,:)=[];
pr(size(pr,1),:)=[];
ps=sign(pr); % 得到的ps就是12支股票的正/负/零收益了
% 然后把ps转化为词频，一种方法是每天对应三个词，下面就用这个方法
slices=floor((size(ps,1)+1-m)/n);
pz=zeros(slices*3, size(ps, 2));
for i=1:size(ps, 2)
	psi=ps(:,i);
	pzi=[psi>0,psi==0,psi<0];
    pzin=zeros(slices, 3);
    for j=0:slices-1
        pzin(j+1,:)=sum(pzi(j*n+1:j*n+m,:),1);
    end;
	pzin=reshape(pzin,3*size(pzin,1),1);
	pz(:,i)=pzin;
end;
% 保存，并运行LDA
pz=pz';
dlmwrite([output_dir '..\' set '-' year 'PZ.txt'],pz,'newline','pc','precision','%.0f');
system([tool_bin 'LDAInputConverter.exe lsa2lda2 ' output_dir '..\' set '-' year 'PZ.txt' ' pz_lda.txt > nul']);
system([tool_bin 'lda.exe est 0.1 ' num2str(topics) ' "' tool_bin 'settings.txt" pz_lda.txt random ' output_dir ' > nul']);
% beta=load('pz/final.beta'); gamma=load('pz/final.gamma'); beta=exp(beta)';
% g=kmeans(gamma,topics); % 输出的就是在这种情况下分类的结果，即每支股票对应的topic编号
% 另外一种方法是用张量