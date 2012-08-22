# 财经数据分析工具集说明文档
# Financial Data Analysis Tools Set

朱恬骅 Zhu Tianhua  
zthpublic@gmail.com  
[http://github.com/zhuth/Tools/](http://github.com/zhuth/Tools/)

## G-means 工具
### 说明
来源：[http://www.cs.utexas.edu/users/dml/Software/gmeans.html](http://www.cs.utexas.edu/users/dml/Software/gmeans.html)  
修改其中代码，使之可以在Windows下通过VC2010编译运行。  
可执行文件：gmeans.exe  
授权协议：GPL

### 用法
详细情况请参看：[http://www.cs.utexas.edu/users/dml/datamining/README_gmeans](http://www.cs.utexas.edu/users/dml/datamining/README_gmeans)。  

### 调用样例
> `gmeans -F t -c 3 gmeans.input`

输入文件 gmeans.input 第一行是矩阵的大小n m，后面n行，每行m列以空格隔开，是整个矩阵的数值。其中每一个文档（特征向量）用一行表示。用gmeans分3类。

## LDA 工具
### 说明
来源：[http://www.cs.princeton.edu/~blei/lda-c/](http://www.cs.princeton.edu/~blei/lda-c/)  
可执行文件：lda.exe  
授权协议：GPL

### 用法
详细情况请参看：[http://www.cs.princeton.edu/~blei/lda-c/readme.txt](http://www.cs.princeton.edu/~blei/lda-c/readme.txt)

### 调用样例
> `lda est 0.01 3 settings.txt lda.input random output_dir\`

从文件 lda.input 读入矩阵，并归为3个话题。输出文件在 `output_dir\`。一般而言，使用 `final.gamma` 和 `final.beta` 。其中，`final.beta` 中保存了每个 topic 下词汇的概率分布的对数；`final.gamma` 中保存了每个输入文档中各 topic 的 Dirichlet prior 值，可以将其看作文档中每个 topic 的比重。

## 矩阵文件格式转换工具
### 说明
可执行文件：LDAInputConverter.exe
授权协议：LGPL

### 用法

> `LDAInputConverter [lda2lsa|lsa2lda|lsa2lda2|lsa2gmeans] <input> <output>`

#### 参数含义

- lda2lsa: 将 LDA 输入格式的文件转换为 LSA 的输入格式。具体而言，就是展开 LDA 所接受的“本行非零元个数 [列号：数值]”展开成能包含所有这些数值的最小矩阵。
- lsa2lda: 将 LSA 的输入格式（完整的矩阵）转换为每一行是“[列号：数值]”的格式，行首不包含本行非零元个数。
- lsa2lda2: 将 LSA 的输入格式（完整的矩阵）转换为每一行是“本行非零元个数 [列号：数值]”的格式。
- lsa2gmeans: 将 LSA 的输入格式（完整的矩阵）转换为 gmeans 工具所接受的带大小说明的输入格式。（请在调用 gmeans 时使用 `-F t` 参数）

`<input>`是输入文件，`<output>`是输出文件。输入文件若为完整的矩阵，元素间可以使用半角空格` `、半角逗号`,`或制表符`\t`分割。*注意：不允许连续的分隔符。*

### 调用样例

> `LDAInputConverter lsa2lda2 freq.txt lda.input  
> lda est 0.01 3 settings.txt lda.input random output_dir\`

## 股票数据选择工具
### 说明
可执行文件：StockDataChooser.exe  
依赖数据文件：

- tfidf.exe  
- industry.txt  保存每只股票的经营范围文本信息，文件格式为：
> [股票代码]（6 位数字）`\t`[经营范围描述]。

本程序有图形界面和命令行两种运行方式。如果不带有参数，则会出现图形界面；如果带有参数，则自动按参数运行并退出。

### 用法

> `StockChooser <data dir> <field id>[_<field id>_<field id>...] <begin date> <end date> <output> <industry output>`

#### 参数含义

- `<data dir>` 股票数据文件夹，每支股票以其代码命名（6位数字），一行代表一个交易日的数据，数据格式如下：
> 年月日,开盘价,最高,最低,收盘价,成交笔数,成交量  
如：
19911223,1.56,1.56,1.55,1.56,127000,3530600.000

- `<field_id>_[<field_id>...]` 每一个`<field_id>`是介于0到7之间的数字，其中0~6分别代表上述7个字段，7表示当日向对于前一天的涨跌幅百分比（精确到1%，涨、跌分别用一列表示）。

- `<begin date>` 起始日期。

- `<end date>` 截止日期。

- `<output>` 股价信息的输出文件名。

- `<industry output>` 这些股票对应的经营范围描述信息的输出文件名。

### 调用样例

> `StockDataChooser ..\..\data\S12\data _ 2011-1-1 2012-1-1 sel.txt idk.txt`

其图形界面的使用方法从略。

## 股票数据随机选择
### 说明
是一个C#源码文件，位置：`StockDataChooserCompact\StockChooser.cs`。请在源代码中设置股票数据文件夹位置、输出文件夹位置和随机选择的数量。  
授权协议：LGPL

## 词频统计和TFIDF统计工具
### 说明
可执行文件：tfidf.exe
授权协议：LGPL

### 用法
> `tfidf <dict> <input> <output> [tf]  
tfidf <input> <output dict> [tf]`

#### 参数含义

- `<dict>` 词典文件。可以包含词频等额外信息，但这些信息都不会被用到。

- `<input>` 输入文件。对于给定了词典文件的情况，每一行输入文本被当作一个单独的文档处理；对于输出词典的情况，整个输入文件被视作一个文档处理。

- `<output>` 输出文件。输出按词典顺序，词典中各个词出现的频率或TFIDF信息。

- `tf` 确定输出的是词频还是TFIDF（对于处理文档）或IDF（对于构造词典）。

- `<output dict>` 输出的词典文件名。

## 话题输出
### 说明
可执行文件：topics.exe  
授权协议：LGPL

### 用法
> `topics <vocab> <beta>`

#### 参数含义

- `<vocab>` 词典文件。

- `<beta>` LDA 输出的beta文件的路径，如`output_dir\final.beta`。

## Matlab 工具集

`matlab` 文件夹下存有所用到的Matlab程序/函数文件。其中还包含了一个G-means工具的Matlab调用。
