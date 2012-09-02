using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace MySqlConnector
{
	public class MySqlDB
	{
		static MySqlConnection conn = null;

        public static void Connect(string database = "", string userid = "root", string password = "")
        {
            string cs = @"server=localhost;userid=" + userid + ";password=" + password + ";Charset=utf8;database=" + database;
            conn = new MySqlConnection(cs);
            conn.Open();
        }

		public static void ExecuteSql(string sql) {
		 	MySqlCommand cmd = new MySqlCommand();
			cmd.Connection = conn;
			cmd.CommandText = sql;
			cmd.Prepare();
			cmd.ExecuteNonQuery();
		}

        public static List<string[]> ExecuteQuery(string sql)
        {
            List<string[]> rows = new List<string[]>();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            cmd.Prepare();
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                string[] row = new string[dataReader.FieldCount];
                for (int i = 0; i < dataReader.FieldCount; ++i)
                    row[i] = dataReader.GetString(i);
                rows.Add(row);
            }

            //close Data Reader
            dataReader.Close();

            return rows;
        }

		public static void Main (string[] args)
		{
            string[] list = System.IO.Directory.GetFiles(args[0]);
            string cs = @"server=localhost;userid=root;password=F8e2B5V4a;Charset=utf8;database=stockdata";
            conn = new MySqlConnection(cs);
            conn.Open();
            Console.WriteLine("MySQL version : {0}", conn.ServerVersion);
            ExecuteSql("set names utf8;");
             
            foreach (string filename in list)
            {
                string table_name = filename.Substring(filename.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1);
                table_name = table_name.Substring(0, table_name.IndexOf('.'));
             
                //try {
                ExecuteSql("DROP TABLE IF EXISTS `" + table_name + "`");
                ExecuteSql("CREATE TABLE IF NOT EXISTS `" + table_name + "`("
                           + "`id` INT PRIMARY KEY AUTO_INCREMENT, "
                           + "`date` VARCHAR(12), `title` VARCHAR(255), `content` TEXT"
                           + ") ENGINE=MyISAM default charset utf8;");

                using (System.IO.StreamReader sr = new System.IO.StreamReader(filename))
                {
                    int count = 0;
                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (line == null) break;
                        if (++count % 50 == 0) Console.WriteLine("Importing from line {0}...", count);
                        string[] cols = line.Split('\t');
                        if (cols.Length < 3) continue;
                        if (string.IsNullOrEmpty(cols[2])) continue;

                        for (int i = 0; i < cols.Length; ++i)
                            cols[i] = cols[i].Replace("\'", "\'\'");

                        //try
                        {
                            ExecuteSql("INSERT INTO `" + table_name + "`(`date`, `title`, `content`) VALUES('" + cols[0] + "','" + cols[1] + "','" + cols[2] + "')");
                        }
                        //catch (MySqlException ex)
                        {
                        //    Console.WriteLine("Record {1}, Exception: {0}", ex, count);
                        }
                    }
                }
                /*
                } catch (MySqlException ex) {
                    Console.WriteLine ("Error: {0}", ex.ToString ());
                } finally {          
                    if (conn != null) {
                        conn.Close ();
                    }
                }*/
            }
	    }
	}
}