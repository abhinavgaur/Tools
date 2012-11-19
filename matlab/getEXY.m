function E = getEXY(X, Y)
n = size(X, 1);
X(:, sum(X) == 0) = []; Y(:, sum(Y) == 0) = [];
X = normalize(X')'; Y = normalize(Y')';
E = zeros(size(X, 2), size(Y, 2));
for i = 1 : n
    E = E + X(i, :)' * Y(i, :);
end;
E = E / n;

