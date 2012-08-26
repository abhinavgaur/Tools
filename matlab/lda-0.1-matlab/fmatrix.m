function m = fmatrix(file,offset)
% m = fmatrix(file,[offset])
% constructs a cell array of struct of (id, cnt) array.
% file   : file of feature data
% offset : offset of initial feature id (specify 1 if you use zero-origin id)
% m      : cell array of data, each cell d is a struct of {d.id, d.cnt},
%          each of which is an array of ids and corresponding counts.
%          e.g. d.id(1) = 5, d.cnt(1) = 2
%               d.id(2) = 1, d.cnt(2) = 10 ..
% $Id: fmatrix.m,v 1.6 2004/10/26 02:23:37 dmochiha Exp $
if nargin < 2
  offset = 0;
end

if ischar(file)
    % open file
    fid = fopen(file);
    if (fid == -1)
      error(sprintf('fmatrix: can''t open %s.',file));
    end
    m = {};
    j = 0;
    % read file
    while ~feof(fid)
      l = fgetl(fid);
      f = sscanf(l,'%d:%g',Inf);
      n = length(f) / 2;
      assert(isint(n));
      d.id  = zeros(1,n);
      d.cnt = zeros(1,n);
      for i = 1:n
        d.id(i)  = f(2*i-1) + offset;
        d.cnt(i) = f(2*i);
      end
      j = j + 1;
      m{j} = d;
    end
    % close file
    fclose(fid);
else
    m = {};
    nd = size(file,1);
    n = size(file,2);
    for di = 1:nd
        j = 1;
        for i = 1:n
            if file(di, i) > 0
                d.id(j) = i;
                d.cnt(j) = file(di, i);
                j = j + 1;
            end;
        end;
        m{di} = d;
    end;
end;