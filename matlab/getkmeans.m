function g = getkmeans(set, year, clusters)
fprintf('getkmeans(''%s'', ''%s'', %d);\n', set, year, clusters);
kmeansRunTimes = 10;
disp('K-means over WF:');
data=load(['..\' set '\' set '-' year 'WF.txt']);
for krun = 1 : kmeansRunTimes
    v = +inf; g = [];
    gg = gmeans(data, clusters);
    vv = scoreresult(gg, clusters);
    if v>vv, g=gg; v=vv; end;
end;
disp(g);
disp('LDA over WF:');
data=load(['..\' set '\' set '-' year 'WF\final.gamma']);
[mm g] = max(data'); g = g';
disp(g);
disp('LDA + K-means over WF:');
try
    for krun = 1 : kmeansRunTimes
        v = +inf; g = [];
        gg = gmeans(data, clusters);
        vv = scoreresult(gg, clusters);
        if v>vv, g=gg; v=vv; end;
    end;
    disp(g);
catch
    disp('Failed.');
end;

disp('K-means over PD:');
data=load(['..\' set '\' set '-' year 'PZ.txt']);
for krun = 1 : kmeansRunTimes
    v = +inf; g = [];
    gg = gmeans(data, clusters);
    vv = scoreresult(gg, clusters);
    if v>vv, g=gg; v=vv; end;
end;
disp(g);
disp('LDA over PD:');
data=load(['..\' set '\' set '-' year 'PD\final.gamma']);
[mm g] = max(data'); g = g';
disp(g);
disp('LDA + K-means over PD:');
try
    for krun = 1 : kmeansRunTimes
        v = +inf; g = [];
        gg = gmeans(data, clusters);
        vv = scoreresult(gg, clusters);
        if v>vv, g=gg; v=vv; end;
    end;
    disp(g);
catch
    disp('Failed.');
end;