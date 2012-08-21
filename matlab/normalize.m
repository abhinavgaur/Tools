function g = normalize(A)
g = A*diag(1./sum(A));