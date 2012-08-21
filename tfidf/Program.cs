using System;
using System.Collections.Generic;
using System.Text;

namespace tfidf
{    
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
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: tfidf <dict> <input> <output> [tf]");
                return;
            }
            bool tfonly = false;
            if (args.Length == 4)
            {
                tfonly = args[3] == "tf";
            }

            string[] dict = System.IO.File.ReadAllLines(args[0], Encoding.GetEncoding("GBK")),
                docs = System.IO.File.ReadAllLines(args[1], Encoding.GetEncoding("GBK"));
            for (int i = 0; i < dict.Length; ++i)
                if (dict[i].IndexOf('\t') > 0) dict[i] = dict[i].Substring(0, dict[i].IndexOf('\t'));
            
            TFIDFDictionary tfidf = new TFIDFDictionary(dict);
            tfidf.CalcIDF(docs);

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
