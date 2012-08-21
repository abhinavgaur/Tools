function label = nCut( d, sigma, k )

N = size( d, 1 );
W = exp(-d.^2/(2*sigma^2));

d = sum(W,1);

B = sparse(1:N,1:N,1./sqrt(d));
WHat = B*W*B; [ V D ] = eig(WHat);

% get the eigen vectors in descending order
[ disc dindex ] = sort(diag(D));
Yk = V(:,dindex(k:-1:1));

% find Xk
Yk = Yk./repmat(sqrt(sum(Yk.^2,2)),1,k);
label = kmeans( Yk, k );