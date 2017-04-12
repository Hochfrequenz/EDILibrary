using System;
using System.Net;
using System.Windows;

using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Linq;
namespace EDILibrary
{
    public class TreeElement :IDisposable
    {
        public String Name;
        public int Occurence;
        public List<String> Edi;
        public Dictionary<String, TreeElement> Children;
        public TreeElement Parent;
        public bool Dirty;
        public bool Key;
        public string CONTRL_Status;
        public string APERAK_Status;
        public string CONTRL_Check_String;
        public string APERAK_Check_String;
        public static string ExtractName(string name)
        {
            if (name.Contains("_"))
                return name.Substring(0, name.IndexOf('_'));
            else
                return name;
        }
        public void AddChild(TreeElement child)
        {
            child.Parent = this;
            if (child.Name.StartsWith("SG") || child.Name == "UNH")
            {
                if (this.Children.ContainsKey(child.Name + "_" + child.Occurence.ToString()) == false)
                    this.Children.Add(child.Name + "_" + child.Occurence.ToString(), child);
            }
            else
            {
                if (this.Children.ContainsKey(child.Name) == false)
                    this.Children.Add(child.Name, child);
            }
        }
        public TreeElement AddEdi(string edi, TreeElement currentRoot)
        {

            Edi.Add(edi);
            return this.Parent;

        }
        public TreeElement(TreeElement old)
        {
            this.Name = old.Name;

            this.Children = new Dictionary<string, TreeElement>();
            this.Parent = old.Parent;
            this.Key = old.Key;
            this.CONTRL_Status = old.CONTRL_Status;
            this.APERAK_Status = old.APERAK_Status;
            this.CONTRL_Check_String = old.CONTRL_Check_String;
            this.APERAK_Check_String = old.APERAK_Check_String;
            foreach (TreeElement child in old.Children.Values)
            {
                TreeElement newChild = new TreeElement(child)
                {
                    Parent = this
                };
                if (newChild.Name.StartsWith("SG") || newChild.Name == "UNH")
                {
                    if (Occurence > 0)
                    {
                        if (this.Children.ContainsKey(newChild.Name + "_" + Occurence.ToString()) == false)
                            this.Children.Add(newChild.Name + "_" + Occurence.ToString(), newChild);
                    }
                    else
                    {
                        if (this.Children.ContainsKey(newChild.Name) == false)
                            this.Children.Add(newChild.Name, newChild);
                    }
                }
                else
                {
                    if (this.Children.ContainsKey(newChild.Name) == false)
                        this.Children.Add(newChild.Name, newChild);
                }

            }
            this.Occurence = old.Occurence + 1;
            Edi = new List<string>();
            this.Dirty = old.Dirty;
        }
        public TreeElement(string name, int lineCounter = -1)
        {
            if (name.Contains('['))
            {
                string[] nameSplit = name.Split("[".ToCharArray());
                Name = nameSplit[0];
                string nameIndex = nameSplit[1];
                nameIndex = nameIndex.Substring(0, nameIndex.Length - 1);
                string[] nameParts = nameIndex.Split(";".ToCharArray());
                CONTRL_Status = nameParts[0];
                APERAK_Status = nameParts[1];
                if (nameParts.Length > 2)
                    CONTRL_Check_String = nameParts[2];
                if (nameParts.Length > 3)
                    APERAK_Check_String = nameParts[3];
            }
            else
            {
                Name = name;
            }
            Children = new Dictionary<string, TreeElement>();
            Parent = null;
            Edi = new List<string>();
            Dirty = false;
            Occurence = 0;
            Key = false;
        }
        public void FindElements(string name, bool recursive, ref List<TreeElement> list, int recursionDepth = Int32.MaxValue)
        {
            if (Name == name)
            {
                list.Add(this);
                return;
            }
            if (Name.Contains("_") && Name.Substring(0, Name.IndexOf('_')) == ExtractName(name))
            {
                list.Add(this);
                return;
            }
            //if (Children.ContainsKey(ExtractName(name)))
            //{
            //    foreach (KeyValuePair<string, TreeElement> child in Children)
            //    {
            //        if (child.Key.Contains("_"))
            //        {
            //            if (child.Key.Substring(0, child.Key.IndexOf('_')) == ExtractName(name))
            //            {
            //                list.Add(child.Value);
            //            }
            //        }
            //        else
            //        {
            //            if (child.Key == name)
            //                list.Add(child.Value);
            //        }
            //    }
            //    //   return;
            //}
            // else
            // {
            if (recursive && recursionDepth > 0)
            {
                foreach (TreeElement child in Children.Values)
                {
                    child.FindElements(name, recursive, ref list, recursionDepth-1);
                }
            }
            return;
            // }
        }
        public TreeElement FindElement(string name)
        {
            return FindElement(name, true);
        }
        public TreeElement FindElement(string name, bool recursive)
        {
            if (Name == name)
                return this;
            if (Children.ContainsKey(ExtractName(name)))
            {
                return Children[ExtractName(name)];
            }
            else
            {
                if (recursive)
                {
                    foreach (TreeElement child in Children.Values)
                    {
                        TreeElement ret = child.FindElement(name);
                        if (ret != null)
                            return ret;
                    }
                }
                return null;
            }
        }

        public void Dispose()
        {
            foreach (TreeElement child in Children.Values)
            {
                child.Dispose();
            }
        }

        public TreeElement this[string name]
        {
            get { return Children[ExtractName(name)]; }
            set { Children[ExtractName(name)] = value; }
        }
    }
    public class TreeHelper
    {
        public static Dictionary<String, TreeElement> treeCopyMap = new Dictionary<String, TreeElement>();
        public static TreeElement treeRoot;
        public static void RefreshDirtyFlags(TreeElement root)
        {
            var children = from elem in root.Children.Values where elem.Dirty && elem.Children.Count() == 0 select elem;
            if (children.Count() == 0)
                root.Dirty = false;
            foreach (TreeElement child in root.Children.Values)
            {
                RefreshDirtyFlags(child);
            }
            treeRoot = null;
            treeCopyMap = new Dictionary<string, TreeElement>();
        }
        public static TreeElement FindEdiElement(ref TreeElement root, string segment)
        {


            TreeElement oldRoot = null;
            string segName = segment.LowMemSplit("+").First();
            if (segName.StartsWith("UNS"))
            {
            }
            if (root.Children.ContainsKey(segName))
            {
                if (root.Children[segName].Dirty)
                {
                    if (root.Name != "/")
                    {
                        //TEST 1302
                        //foreach (TreeElement child in root.Children.Values)
                        //    child.Dirty = true;
                    }
                    TreeElement ele = null;
                    if (treeCopyMap.ContainsKey(root.Name) && treeCopyMap[root.Name].Occurence != root.Occurence)
                    {
                        TreeElement linkRoot = treeCopyMap[root.Name];
                        ele = FindEdiElement(ref linkRoot, segment);
                        oldRoot = root;
                        root = linkRoot;

                        System.Diagnostics.Debug.Assert(root != null, segment, oldRoot.ToString());
                        return ele;
                    }
                    else
                    {
                        oldRoot = root;
                        if (root.Name != "/")
                        {
                            root = root.Parent;
                            System.Diagnostics.Debug.Assert(root != null, segment, oldRoot.ToString());
                            ele = FindEdiElement(ref root, segment);
                            return ele;
                        }
                        else if (segName.StartsWith("UNH"))
                        {
                            if (treeRoot == null)
                                treeRoot = root.Children[segName];
                            TreeElement copy = new TreeElement(treeRoot);
                            TreeElement child = treeRoot;
                            treeRoot = copy;
                            //bei UNH muss ich dann schon die Anzahl der UNH-Kinder zählen und die Occurence manuell setzen
                            int UNHCounter = 0;
                            foreach (TreeElement tempChild in root.Children.Values)
                            {
                                if (tempChild.Name.StartsWith("UNH"))
                                    UNHCounter++;
                            }
                            foreach (TreeElement c in child.Children.Values)
                                c.Dirty = false;
                            foreach (TreeElement c in copy.Children.Values)
                                c.Dirty = false;
                            treeCopyMap["UNH"] = copy;
                            copy.Dirty = false;
                            copy.Occurence = UNHCounter;
                            root.AddChild(copy);
                            root = child;
                            return child;
                        }

                    }

                }
                if (root.Children[segName].Key)
                {

                    TreeElement child = root.Children[segName];
                    if (child.Parent.Name != "/" && child.Parent.Name != "UNH")
                    {
                        TreeElement copy = new TreeElement(root);
                        treeCopyMap[TreeElement.ExtractName(root.Name)] = copy;
                        root.Parent.AddChild(copy);
                        root.Dirty = true;
                        copy.Dirty = false;
                    }


                    root.Children[segName].Dirty = true;
                }
                return root.Children[segName];
            }
            else
            {
                foreach (TreeElement child in root.Children.Values)
                {
                    if (child.Dirty)
                        continue;
                    TreeElement ele = child.FindElement(segName, false);
                    // Wenn das Element existiert müssen wir tiefer navigieren und legen dafür eine Kopie an
                    if (ele != null)
                    {
                        TreeElement copy = new TreeElement(child);
                        treeCopyMap[TreeElement.ExtractName(child.Name)] = copy;
                        child.Parent.AddChild(copy);
                        child.Dirty = true;
                        copy.Dirty = false;
                        oldRoot = root;
                        root = child;
                        System.Diagnostics.Debug.Assert(root != null, segment, oldRoot.ToString());
                        if (child[segName].Key)
                            child[segName].Dirty = true;
                        return child[segName];
                    }
                }
                if (root.Parent != null)
                {
                    //TEST 1302
                    //if (root.Parent.Name != "/")
                    //{
                    //    foreach (TreeElement child in root.Children.Values)
                    //        child.Dirty = true;
                    //}
                    root = root.Parent;
                    TreeElement ele = FindEdiElement(ref root, segment);

                    return ele;
                }
                else
                {
                    //oldRoot = root;
                    //root = root.Parent;
                    //System.Diagnostics.Debug.Assert(root != null, segment, oldRoot.ToString());
                    return null;
                }

            }
        }
        static SHA1 hash = System.Security.Cryptography.SHA1.Create();
        static UnicodeEncoding UE = new UnicodeEncoding();
        public static string GetHash(string TextToHash)
        {
            //Prüfen ob Daten übergeben wurden.
            if ((TextToHash == null) || (TextToHash.Length == 0))
            {
                return string.Empty;
            }

            //MD5 Hash aus dem String berechnen. Dazu muss der string in ein Byte[]
            //zerlegt werden. Danach muss das Resultat wieder zurück in ein string.

            // Convert the string into an array of bytes.
            byte[] textToHash = UE.GetBytes(TextToHash);
            byte[] result = hash.ComputeHash(textToHash);

            return System.BitConverter.ToString(result);
        }
        public static bool CleanTree(TreeElement ele)
        {
            if (ele.Edi.Count == 0 && ele.Children.Count == 0)
                return true;
            if (ele.Children.Count > 0)
            {
                bool delete = true;
                List<string> deleteList = new List<string>();
                foreach (KeyValuePair<string, TreeElement> child in ele.Children)
                {
                    if (CleanTree(child.Value) == false)
                        delete = false;
                    else
                    {
                        deleteList.Add(child.Key);


                    }
                }
                foreach (string del in deleteList)
                    ele.Children.Remove(del);
                if (delete)
                {
                    ele.Children.Clear();
                    return true;
                }
            }
            return false;
        }

    }
}
