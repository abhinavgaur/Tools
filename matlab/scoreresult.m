function v = scoreresult( g, clusters )
if (size(g,1) ==0), v = +Inf; return; end;
d = size(g, 1) / clusters;
v = 0;
for i = 1 : clusters
    v = v + abs(sum((g == i)) - d);
end;
end

