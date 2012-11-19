function g = normalize(A, n)
if (nargin < 2), n=1; end;
if (n==1),
    g = A*diag(1./sum(A));
else
    g = A*diag(1./sqrt(sum(A.*A)));
end;