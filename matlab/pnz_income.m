function g = pnz_income(set, year, topics)
tool_bin = 'D:\Documents\ѧУ\��ҵ\��ҵ���\Tools\bin\';
data_filename = [tool_bin '..\..\Data\' set '\' set '-' year 'PD.txt'];
output_dir = [tool_bin '..\..\Data\' set '\' set '-' year 'PD\'];
if (exist('n') ~= 1), n = 1; end; % ��Ƭ����
if (exist('m') ~= 1), m = 1; end; % ���ڳ���

% ��ȡ����
p=load(data_filename)';
p=p(1:size(p,1)-1,:);
pn=[p(2:size(p,1),:);zeros(1,size(p,2))];
pr=log(pn./p);
pr(sum(abs(pr),2)==0,:)=[];
pr(size(pr,1),:)=[];
ps=sign(pr); % �õ���ps����12֧��Ʊ����/��/��������
% Ȼ���psת��Ϊ��Ƶ��һ�ַ�����ÿ���Ӧ�����ʣ���������������
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
% ���棬������LDA
pz=pz';
dlmwrite([output_dir '..\' set '-' year 'PZ.txt'],pz,'newline','pc','precision','%.0f');
system([tool_bin 'LDAInputConverter.exe lsa2lda2 ' output_dir '..\' set '-' year 'PZ.txt' ' pz_lda.txt > nul']);
system([tool_bin 'lda.exe est 0.1 ' num2str(topics) ' "' tool_bin 'settings.txt" pz_lda.txt random ' output_dir ' > nul']);
% beta=load('pz/final.beta'); gamma=load('pz/final.gamma'); beta=exp(beta)';
% g=kmeans(gamma,topics); % ����ľ�������������·���Ľ������ÿ֧��Ʊ��Ӧ��topic���
% ����һ�ַ�����������