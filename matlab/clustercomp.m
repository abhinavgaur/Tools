function [result, check] = clustercomp(c1, c2, n)
result = zeros(size(c1));
for i=1:n,
    for j=1:n,
        result = result + ((c1==i)&(c2==j));
    end;
end;
check = all(result == ones(size(result)));