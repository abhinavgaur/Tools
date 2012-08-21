function g = getindkmeans(set, clusters)
fprintf('getindkmeans(''%s'', %d);\n', set, clusters);
kmeansRunTimes = 10;

disp('K-means over IND:');
data=load(['..\' set '\' set 'DFQ.txt']);
g=kmeans(data, clusters);
disp(g);

disp('LDA over IND:');
data=load(['..\' set '\' set 'IND\final.gamma']);
[mm g] = max(data'); g = g';
disp(g);
disp('LDA + K-means over IND:');
try
    for krun = 1 : kmeansRunTimes
        v = +inf; g = [];
        gg = kmeans(data, clusters);
        vv = scoreresult(gg, clusters);
        if v>vv, g=gg; v=vv; end;
    end;
    disp(g);
catch
    disp('Failed.');
end;

disp('K-means over IDK:');
data=load(['..\' set '\' set 'KFQ.txt']);
g=kmeans(data, clusters);
disp(g);

disp('LDA over IDK:');
data=load(['..\' set '\' set 'IDK\final.gamma']);
[mm g] = max(data'); g = g';
disp(g);
disp('LDA + K-means over IDK:');
try
    for krun = 1 : kmeansRunTimes
        v = +inf; g = [];
        gg = kmeans(data, clusters);
        vv = scoreresult(gg, clusters);
        if v>vv, g=gg; v=vv; end;
    end;
    disp(g);
catch
    disp('Failed.');
end;