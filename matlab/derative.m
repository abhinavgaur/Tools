function D = derative(n)
D = zeros(n-3, n-1);
dd = [1 -2 1 zeros(1, n-4)];
for i=1:n-3,
    D(i,:) = dd;
    dd = [0 dd(1:n-2)];
end;