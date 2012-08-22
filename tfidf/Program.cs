using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace tfidf
{
    public class EncodingType
    {
        /// <summary>
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// </summary>
        /// <param name="FILE_NAME">文件路径</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// <summary>
        /// 通过给定的文件流，判断文件的编码类型
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;

        }

        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }

    }

    public class TFIDFDictionary
    {
        public Dictionary<string, double> tf = new Dictionary<string, double>();
        public Dictionary<string, double> idf = new Dictionary<string, double>();

        public void ClearTF()
        {
            foreach (string s in idf.Keys)
                tf[s] = 0;
        }

        public void ClearIDF()
        {
            foreach (string s in tf.Keys)
                idf[s] = 0;
        }

        public TFIDFDictionary(string[] dict)
        {
            foreach (string s in dict)
            {
                if (tf.ContainsKey(s)) continue;
                tf.Add(s, 0);
                idf.Add(s, 0);
            }
        }

        public TFIDFDictionary(Dictionary<string, int> dict)
        {
            foreach (string s in dict.Keys)
            {
                tf.Add(s, 0);
                idf.Add(s, 0);
            }
        }

        public Dictionary<string, double> CalcTFIDF(string doc)
        {
            int wc = 0;
            this.ClearTF();
            string[] words = doc.Split(new char[] { ' ', '\t' });
            foreach (string word in words)
            {
                if (!tf.ContainsKey(word)) continue;
                wc++; tf[word]++;
            }

            foreach(string word in idf.Keys)
                tf[word] = tf[word] / wc * idf[word];

            return tf;
        }

        public Dictionary<string, double> CalcTF(string doc)
        {
            int wc = 0;
            this.ClearTF();
            string[] words = doc.Split(new char[] { ' ', '\t' });
            foreach (string word in words)
            {
                if (!tf.ContainsKey(word)) continue;
                wc++; tf[word]++;
            }
            return tf;
        }

        public Dictionary<string, double> CalcIDF(string[] docs)
        {
            ClearIDF();
            double logDocCount = Math.Log(docs.Length);

            Dictionary<string, Boolean> dictc = new Dictionary<string, Boolean>();
            foreach(string word in tf.Keys)
                dictc.Add(word, false);

            foreach (string doc in docs)
            {
                string[] words = doc.Split(new char[] { ' ', '\t' });
                
                foreach (string word in tf.Keys)
                    dictc[word] = false;

                foreach (string word in words)
                {
                    if (!tf.ContainsKey(word) || dictc[word]) continue;
                    idf[word]++; dictc[word] = true;
                }
            }

            foreach (string word in tf.Keys)
            {
                idf[word] = logDocCount - Math.Log(idf[word]);
            }

            return idf;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: tfidf <dict> <input> <output> [tf]\r\n       tfidf <input> <output dict> [tf]");
                return;
            }
            string[] dict, docs;
            if (args.Length == 2) {
                docs = System.IO.File.ReadAllLines(args[0], EncodingType.GetType(args[0]));
                Dictionary<string, int> mydict = new Dictionary<string, int>();

                foreach (string doc in docs)
                {
                    string[] words = doc.Split(new char[] { ' ', '\t' });

                    foreach(string word in words) {
                        if (!mydict.ContainsKey(word))
                            mydict.Add(word, 1);
                    }
                }

                TFIDFDictionary idf = new TFIDFDictionary(mydict);
                idf.CalcIDF(docs);

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(args[1]))
                {
                    foreach (string s in idf.idf.Keys)
                    {
                        sw.WriteLine("{0}\t{1}", s, idf.idf[s]);
                    }
                }

                return;
            }

            if (args.Length == 3 && args[2] == "tf") {
                string doc = System.IO.File.ReadAllText(args[0], EncodingType.GetType(args[0]));
                string[] words = doc.Split(new char[] { ' ', '\t' });

                Dictionary<string, int> mydict = new Dictionary<string, int>();                
                foreach (string wordOrig in words)
                {
                    string word = wordOrig.Trim();
                    if (!mydict.ContainsKey(word))
                        mydict.Add(word, 1);
                    else mydict[word]++;
                }

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(args[1]))
                {
                    foreach (string s in mydict.Keys)
                    {
                        sw.WriteLine("{0}\t{1}", s, mydict[s]);
                    }
                }

                return;
            }

            bool tfonly = false;
            if (args.Length == 4)
            {
                tfonly = args[3] == "tf";
            }

            dict = System.IO.File.ReadAllLines(args[0], EncodingType.GetType(args[0]));
            docs = System.IO.File.ReadAllLines(args[1], EncodingType.GetType(args[1]));
            for (int i = 0; i < dict.Length; ++i)
            {
                if (dict[i].IndexOf('\t') > 0) dict[i] = dict[i].Substring(0, dict[i].IndexOf('\t'));
                if (dict[i].IndexOf(' ') > 0) dict[i] = dict[i].Substring(0, dict[i].IndexOf(' '));
            }
            
            TFIDFDictionary tfidf = new TFIDFDictionary(dict);
            if (!tfonly) tfidf.CalcIDF(docs);

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(args[2]))
            {
                foreach (string doc in docs)
                {
                    Dictionary<string, double> ti = null;
                    if (!tfonly)
                        ti = tfidf.CalcTFIDF(doc);
                    else 
                        ti = tfidf.CalcTF(doc);

                    double[] hi = new double[ti.Values.Count];
                    ti.Values.CopyTo(hi, 0);

                    sw.Write(hi[0].ToString("#0.0000"));
                    for (int i = 1; i < hi.Length; ++i)
                        sw.Write(",{0:N4}", hi[i]);

                    sw.WriteLine();
                }
            }
        }
    }
}
