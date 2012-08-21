﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LDAInputConverter
{
    class Program
    {
        struct ColValue {
            public int col;
            public double val;

            public ColValue(int column, double value)
            {
                col = column; val = value;
            }
        }

        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: LDAInputConverter [lda2lsa|lsa2lda|lsa2lda2|lsa2gmeans] <input> <output>");
                return;
            }
            string option = args[0], input = args[1], output = args[2];
            
            string[] lines = System.IO.File.ReadAllLines(input);
                    
            switch (option)
            {
                case "lda2lsa":
                    int colNum = 0;
                    foreach (string line in lines)
                    {
                        string[] cols = line.Split(' ');
                        foreach (string col in cols)
                        {
                            if (col.IndexOf(':') <= 0) continue;
                            string[] val = col.Split(':');
                            int cval = int.Parse(val[0]);
                            if (cval > colNum) colNum = cval;
                        }
                    }

                    double[,] mat = new double[lines.Length, colNum];

                    for(int row = 0; row < lines.Length; ++row)
                    {
                        string[] cols = lines[row].Split(' ');
                        foreach (string col in cols)
                        {
                            if (col.IndexOf(':') <= 0) continue;
                            string[] val = col.Split(':');
                            int cval = int.Parse(val[0]); double eval = double.Parse(val[1]);
                            mat[row, cval] = eval;
                        }
                    }

                    using(System.IO.StreamWriter sw = new System.IO.StreamWriter(output)) {
                        for (int i = 0; i < lines.Length; ++i)
                        {
                            sw.Write(mat[i, 0]);
                            for (int j = 1; j < colNum; ++j)
                            {
                                sw.Write("," + mat[i, j]);
                            }
                            sw.WriteLine();
                        }
                        sw.Flush();
                    }

                    break;

                case "lsa2gmeans":
                    int rowCount1 = lines.Length, colCount1 = lines[0].Split(',',' ','\t').Length;
                    
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(output))
                    {
                        sw.WriteLine("{0} {1}", rowCount1, colCount1);
                        for (int i = 0; i < rowCount1; ++i)
                        {
                            string[] cols = lines[i].Split(',', ' ', '\t');
                            for (int j = 0; j < colCount1; ++j)
                            {
                                double eval = 0;
                                double.TryParse(cols[j], System.Globalization.NumberStyles.Any, null, out eval);
                                sw.Write(eval.ToString("0.0000") + " ");
                            }
                            sw.WriteLine();
                        }
                    }
                    
                    break;

                case "lsa2lda":
                case "lsa2lda2":
                    int rowCount = lines.Length, colCount = lines[0].Split(',',' ', '\t').Length;
                    List<ColValue> ls = new List<ColValue>();
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(output))
                    {
                        for (int i = 0; i < rowCount; ++i)
                        {
                            ls.Clear();
                            string[] cols = lines[i].Split(',', ' ', '\t');
                            if (cols.Length != colCount) continue;
                            for (int j = 0; j < colCount; ++j)
                            {
                                if (cols[j] == "") continue;
                                try
                                {
                                    double eval = double.Parse(cols[j], System.Globalization.NumberStyles.Any);
                                    if (eval != 0)
                                        ls.Add(new ColValue(j, eval));
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex);
                                    Console.WriteLine(cols[j]);
                                }
                            }

                            if (option == "lsa2lda2") sw.Write(ls.Count + " ");
                            foreach (ColValue cv in ls)
                                sw.Write(cv.col + ":" + cv.val + " ");

                            sw.WriteLine();
                        }
                        sw.Flush();
                    }
                    break;
            }
        }
    }
}
