# 财经数据分析工具集
## Matlab 函数工具
朱恬骅 [zthpublic@gmail.com](mailto:zthpublic@gmail.com)

### G-means (Gaussian means)
gmeans.m

声明：
`function [L C] = gmeans(data, alpha)`

输入：

- `data` 数据，一行为一个文档。
- `alpha` 正态分布置信度，可不给，默认为0.5%。

输出：

- `L` 列向量，表示每个文档的所在类。
- `C` 一个矩阵，表示每行表示一个类的 K-means 中心。

### 3D K-means
kmeans3d.m

声明：
`function [L C] = kmeans3d(data, k)`

输入：

- `data` 数据，一行为一个文档。
- `k` K-means 的类数量。

输出：

- `L` 列向量，表示每个文档的所在类。
- `C` 一个矩阵，表示每行表示一个类的 K-means 中心。

### N-Cut
nCut.m   

源码来源于网上。

声明：  
`function label = nCut( d, sigma, k )`

输入：

- `d` 表示邻接矩阵。   
- `sigma` 表示 N-Cut 中的 `sigma`。  
- `k` 表示要分割成的数量。

输出：

- `label` 列向量，表示每个点所在的那一个割。

### Net Branches 分支数
Net_Branches.m

Refer:   
Ulrik Barandes: *A faster algorithm for betweennes centrality*    
Written by Hu Yong, E-mail: carrot.hy2010@gmail.com   


### 归一化
normalize.m  

将输入矩阵按*列*归一化。

### 余弦角 K-means (spkmeans)
spkmeans.m


声明：
`function [L C] = kmeans3d(data, k)`

输入：

- `data` 数据，一行为一个文档。
- `k` K-means 的类数量。

输出：

- `L` 列向量，表示每个文档的所在类。
- `C` 一个矩阵，表示每行表示一个类的 K-means 中心。


### Spectral Clustering
spectral_clustering.m

代码来源于网上，目前我也不知道怎么用。

### lda-0.1-matlab
[http://chasen.org/~daiti-m/dist/lda/](http://chasen.org/~daiti-m/dist/lda/)

LDA 的一个 Matlab 实现，备用。不带计算后验概率的功能。