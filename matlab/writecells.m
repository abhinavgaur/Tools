function writecells(filename, cell)
fid=fopen(filename,'w');
for i=1:size(cell,1)
    for j=1:size(cell,2)
        fprintf(fid,'%s ',cell{i,j});
    end
    fprintf(fid,'\r\n');
end
fclose(fid);