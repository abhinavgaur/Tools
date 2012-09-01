function [result, A] = clustercomp(c1, c2)
if (length(c1) ~=length(c2)), result = 1; A = []; return; end;
l1 = max(c1(:)); l2 = max(c2(:));
A = zeros(l1, l2);
for i=1:l1,
    for j=1:l2,
        A(i,j) = sum(c1==i & c2==j);
    end;
end;
result = sum(sum(A)-max(A)) + sum(sum(A')-max(A'));
result = 1 - result / length(c1);