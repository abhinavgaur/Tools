function [L C] = gmeans(data, alpha)
%1
if (nargin < 2), alpha = 0; end;
n = size(data, 1);
nC = 1;
C(1,:) = mean(data);
while 1,
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

if (alpha == 0), alpha = 0.005; end;
switch alpha
    case 0.1
        alpha = 0.631;
    case 0.05
        alpha = 0.752;
    case 0.025
        alpha = 0.873;
    case 0.01
        alpha = 1.035;
    case 0.005
        alpha = 1.159;
end;

if (size(data,1) == 1)
    t = 1; CC = [data; data];
    return;
end;

% choose two new centers, step 2
[~, lambda, s] = svds(data, 1);
m = s' * sqrt(2 * lambda / pi);
%disp(size(m)); disp(size(c));
CC(1, :) = c + m; CC(2, :) = c - m;

%3
[lc CC] = kmeans(data, 2, 'start', CC, 'EmptyAction', 'drop');
v = CC(1, :) - CC(2, :);

%4
nd = size(data, 1);
%%disp('data'); disp(size(data)); disp('v'); disp(size(v));
X = data*v';
X = X / std(X); X = X - mean(X);
%5
Z = normcdf(X, 0, 1);

% compute A^2(Z)
AA = 0;
for j=1:nd,
    AA = AA + (2*j-1)*(log(Z(j)) + log(1 - Z(nd+1-j)));
end;
AA = - AA / nd + nd;
AA = AA * (1+4/nd-25/(nd*nd)); %A-star^2(Z)
t = (AA <= alpha);
