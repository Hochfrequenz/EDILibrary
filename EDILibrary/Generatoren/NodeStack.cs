// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using EDILibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDILibrary.Generatoren
{
    public class NodeStack : Stack<INodeScope>
    {
        public NodeStack(IEnumerable<INodeScope> collection) : base(collection)
        {
        }
        public NodeStack() : base()
        {

        }
        public string Path
        {
            get
            {
                return SkippedPath(0);
            }
        }
        public string EscapedPath
        {
            get
            {
                return SkippedPath(0, true);
            }
        }
        public string SkippedPath(int iSkip, bool escape = false, NodeStack overrideStack = null)
        {
            List<string> paths = new List<string>();
            var stack = this;
            if (overrideStack != null)
                stack = overrideStack;
            foreach (var node in stack)
            {
                if (escape)
                    paths.Add("['" + node.Node + "']" + "[" + node.Counter + "]");
                else
                    paths.Add(node.Node + "[" + node.Counter + "]");
            }
            paths.Reverse();
            return String.Join(".", paths.Skip(iSkip));
        }
        public static string EscapePath(string path)
        {
            var extensions = path.Split('.');
            List<string> escaped = new List<string>();
            foreach (var ext in extensions)
            {
                if (ext.Contains('['))
                {
                    var parts = ext.Split('[');
                    escaped.Add("['" + parts[0] + "']" + "[" + parts[1]);
                }
                else
                {
                    escaped.Add("['" + ext + "']");
                }
            }
            return String.Join(".", escaped);
        }
        public static string CombinePath(string path, string extension, bool escape = false)
        {
            var extensionSafe = extension;
            if (escape)
            {
                if (extension != null && extension.Contains("."))
                {
                    var extensions = extension.Split('.');
                    List<string> escaped = new List<string>();
                    foreach (var ext in extensions)
                    {
                        if (ext.Contains('['))
                        {
                            var parts = ext.Split('[');
                            escaped.Add("['" + parts[0] + "']" + "[" + parts[1]);
                        }
                        else
                        {
                            escaped.Add("['" + ext + "']");
                        }
                        extensionSafe = String.Join(".", escaped);
                    }
                }
                else if(extension != null)
                {
                    if (extension.Contains('['))
                    {
                        var parts = extension.Split('[');
                        extensionSafe = "['" + parts[0] + "']" + "[" + parts[1];
                    }
                    else
                    {
                        extensionSafe = "['" + extension + "']";
                    }

                }
            }
            if (String.IsNullOrWhiteSpace(path) == false)
            {
                if (String.IsNullOrWhiteSpace(extensionSafe) == false)
                    return path + "." + extensionSafe;
                else
                    return path;
            }
            else
                return extensionSafe;

        }

        public object Clone()
        {
            return new NodeStack(this.Reverse());
        }
    }
}
