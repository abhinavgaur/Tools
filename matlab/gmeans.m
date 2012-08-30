function [L C] = gmeans(data, maxk, alpha)
%1
if (nargin < 2), maxk = size(data, 1) / 8; end;
if (nargin < 3), alpha = 0; end;
n = size(data, 1);
nC = 1;
C(1,:) = mean(data);
while nC <= maxk,
    %2
    incr_C = 0;
    [L C] = kmeans(data, nC, 'start', C, 'EmptyAction', 'singleton');
    for i = 1:nC,
        %4
        subdata = data(L==i,:);
        [t new_C] = look_gaussian(subdata, alpha, C(i,:));
        if (t ~= 1)
            %5
            C(i, :) = new_C(1, :);
            C = [C; new_C(2, :)];
            incr_C = incr_C + 1;
        end;
    end;
    if (incr_C == 0), break; end;
    %disp(incr_C);
    nC = size(C, 1);
end;

function [t CC]=look_gaussian(data, alpha, c)
if (alpha == 0), alpha = 0.05; end;

if (size(data,1) == 1)
    t = 1; CC = [data; data];
    return;
end;

% choose two new centers, step 2
[~, lambda, s] = svds(data, 1);
m = s' * sqrt(2 * lambda / pi);
CC(1, :) = c + m; CC(2, :) = c - m;

%3
[lc CC] = kmeans(data, 2, 'start', CC, 'EmptyAction', 'drop');
v = CC(1, :) - CC(2, :);

%4
nd = size(data, 1);
X = data*v' / norm(v);
X = (X - mean(X)) / std(X);
%5
t = AnDartest(X, alpha);

function [test] = AnDartest(x,alpha)
n = length(x);
if n < 3,
    test = 1;
    return,
else
    x = x(:);
    x = sort(x);
    fx = normcdf(x,mean(x),std(x));
    i = 1:n;

    S = sum((((2*i)-1)/n)*(log(fx)+log(1-fx(n+1-i))));
    AD2 = -n-S;

    AD2a = AD2*(1 + 0.75/n + 2.25/n^2); %correction factor for small sample sizes: case normal

    if (AD2a >= 0.00 && AD2a < 0.200);
        P = 1 - exp(-13.436 + 101.14*AD2a - 223.73*AD2a^2);
    elseif (AD2a >= 0.200 && AD2a < 0.340);
        P = 1 - exp(-8.318 + 42.796*AD2a - 59.938*AD2a^2);
    elseif (AD2a >= 0.340 && AD2a < 0.600);
        P = exp(0.9177 - 4.279*AD2a - 1.38*AD2a^2);
    else (AD2a >= 0.600 && AD2a <= 13);
        P = exp(1.2937 - 5.709*AD2a + 0.0186*AD2a^2);
    end
end

if P >= alpha;
   test=1;
else
   test=0;
end

return,



