using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ClassDiff
{
    public class Diff
    {
        public static void ScanTemp(string temp, string server)
        {
            Dictionary<string, string> CustDic = new Dictionary<string, string>();
            Dictionary<string, string> SerDic = new Dictionary<string, string>();
            List<string> CustListKey = new List<string>();
            List<string> SerLIstKey = new List<string>();
            List<string> ChangLIstKey = new List<string>();
            bool isSame = false;

            SerDic = Scaner(server);
            foreach (var x in SerDic.Keys)
            {
                SerLIstKey.Add(x);
            }
            string diffs = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\diffs";
            if (!Directory.Exists(diffs))
            { Directory.CreateDirectory(diffs); }
            List<string> middle = new List<string>();
            foreach (var item in Directory.EnumerateDirectories(temp))
            {
                bool DelOrNot = false;
                string txtname = Path.Combine(diffs, item.Substring(item.LastIndexOf("\\") + 1)) + " _" + DateTime.Now.ToString("yyyy.MM.dd") + ".txt";
                CustDic = Scaner(item);
                foreach (var y in CustDic.Keys)
                { CustListKey.Add(y); }
                ChangLIstKey = CompareLine(SerLIstKey, CustListKey, ref isSame);
                using (StreamWriter writer = new StreamWriter(txtname))
                {
                    foreach (var sa in ChangLIstKey)
                    {
                        if (sa[0] == '+' || sa[0] == '-')
                        {
                            DelOrNot = true;
                            writer.WriteLine(sa);
                        }
                        else
                        {
                            if (sa.Contains("."))
                            {
                                middle = CompareText(CustDic[sa], SerDic[sa]);
                                if (middle.Count == 0)
                                    continue;
                                writer.WriteLine(sa);
                                DelOrNot = true;
                                foreach (var T in middle)
                                {
                                    writer.WriteLine(string.Format("     {0}", T));
                                }
                            }
                        }
                        middle.Clear();
                    }
                }
                if (!DelOrNot)
                {
                    File.Delete(txtname);
                }
                CustListKey.Clear();
                ChangLIstKey.Clear();
            }
        }

        private static Dictionary<string, string> Scaner(string path)
        {
            Dictionary<string, string> a = new Dictionary<string, string>();
            scan(path, ref a);
            foreach (var item in a.Keys)
            {
                a.Remove(item); break;
            }
            return a;
        }

        private static void scan(string menu, ref Dictionary<string, string> path)// #
        {
            path.Add(menu.Substring(menu.LastIndexOf("\\") + 1), menu);
            foreach (var item in Directory.EnumerateFiles(menu))
            {
                path.Add(item.Substring(item.LastIndexOf("\\") + 1), item);
            }
            foreach (var item in Directory.EnumerateDirectories(menu))
            {
                scan(item, ref path);
            }
        }

        private static List<string> CompareLine(List<string> server, List<string> user, ref bool isThere)
        {

            List<string> store = new List<string>();
            int rev = 0; int m; int r = 0;
            int j = 0; int g = 0;
            int z; bool isFind = false;
            int l = server.Count + user.Count;
            for (int i = 0; i < l; i++)
            {
                store.Add("");
            }
            for (int i = 0; i < server.Count; i++)
            {
                for (j = rev; j < user.Count; j++)
                {
                    isFind = false;
                    if (server[i] == user[j])
                    {
                        isFind = true;
                        if (g > rev)
                        {
                            z = j - rev;
                            rev = j + 1; m = g + z; r = m;
                            store[m] = server[i];
                            while (z-- > 0)
                            {
                                m--; j--;
                                store[m] = string.Format("+ {0}", user[j]);
                            } g = r + 1; break;
                        }
                        store[j] = server[i];
                        z = j - rev;
                        rev = j + 1;
                        while (z-- > 0)
                        {
                            j--;
                            store[j] = string.Format("+ {0}", user[j]);
                        } g = rev; break;
                    }
                }
                if (!isFind)
                {
                    store[g] = string.Format("- {0}", server[i]);
                    g++;
                }

                if (rev == user.Count && i < server.Count)
                {
                    while (i++ < server.Count - 1)
                        store[g++] = string.Format("- {0}", server[i]);
                }

            }

            while (rev < user.Count)
            {
                store[g] = string.Format("+ {0}", user[rev]);
                rev++; g++;
            }
            for (int i = l - 1; i > 0; i--)
            {
                if (String.IsNullOrEmpty(store[i]))
                {
                    store.Remove(store[i]);
                    continue;
                }
            }

            if (store.Count == server.Count && server.Count == user.Count)
                isThere = true;
            else
                isThere = false;

            return store;
        }

        private static List<string> CompareText(string custPath, string serPath)
        {
            List<string> userList = new List<string>();
            List<string> serverList = new List<string>();
            List<string> changedList = new List<string>();
            string temp = "";

            using (StreamReader readFromUser = new StreamReader(custPath))
            {
                while (!readFromUser.EndOfStream)
                {
                    temp = readFromUser.ReadLine();
                    if (temp == string.Empty)
                    {
                        continue;
                    }
                    userList.Add(temp);
                }
            }
            using (StreamReader readFromServer = new StreamReader(serPath))     //from text to list<string>
            {
                while (!readFromServer.EndOfStream)
                {
                    temp = readFromServer.ReadLine();
                    if (temp == string.Empty)
                    {
                        continue;
                    }
                    serverList.Add(temp);
                }
            }

            bool IsThere = false;
            changedList = CompareLine(serverList, userList, ref IsThere);
            if (IsThere)
            {
                changedList.Clear();
            }

            return changedList;
        } 
    
    }
}
