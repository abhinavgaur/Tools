@echo off
del ..\..\data\*IDK.txt /s
StockDataChooser ..\..\data\S12\data _ 2013-1-1 2013-1-2 ~sel.txt ..\..\data\S12\S12IDK.txt
StockDataChooser ..\..\data\S50\data _ 2013-1-1 2013-1-2 ~sel.txt ..\..\data\S50\S50IDK.txt
StockDataChooser ..\..\data\R50\data _ 2013-1-1 2013-1-2 ~sel.txt ..\..\data\R50\R50IDK.txt
StockDataChooser ..\..\data\S100\data _ 2013-1-1 2013-1-2 ~sel.txt ..\..\data\S100\S100IDK.txt
StockDataChooser ..\..\data\S500\data _ 2013-1-1 2013-1-2 ~sel.txt ..\..\data\S500\S500IDK.txt
del ..\..\data\~sel.txt /s
pause