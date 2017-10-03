using CacheEXTREME2.WMetaGlobal;
using CacheEXTREME2.WDirectGlobal;
using InterSystems.Globals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CacheEXTREME2
{
    public class CacheTreeNode : TreeNode
    {
        public string globalName;
        public String[] Subscripts;
        public ArrayList SubscriptsTyped;
        public CacheTreeNode(TrueNodeReference nr)
            : base()
        {
            globalName = nr.GetName();
        }
        public CacheTreeNode(string title, TrueNodeReference nodeRef)
            : base(title)
        {
            globalName = nodeRef.GetName();
            Subscripts = CacheTreeNode.GetSubscripts(nodeRef);
            SubscriptsTyped = nodeRef.GetSubscripts();
        }
        public static String[] GetSubscripts(TrueNodeReference nodeRef)
        {
            int count = nodeRef.GetSubscripts().Count;
            String[] subs = new String[count];
            for (int i = 0; i < count; i++)
            {
                subs[i] = nodeRef.GetSubscripts()[i].ToString();   
            }
            return subs;
        }
    }

    static class CacheHelper
    {
        public static CacheTreeNode CacheGlobalToCacheTree(TrueNodeReference global)
        {
            CacheTreeNode root = new CacheTreeNode(global.GetName(), global);
            buildTree(global, root);
            return root;
        }
        private static void buildTree(TrueNodeReference glNode, CacheTreeNode treeNode)
        {
            string subscript;
            glNode.AppendSubscript("");
            do
            {
                glNode.GoNextSubscript();
                subscript = glNode[glNode.SubsCount - 1].ToString();
                CacheTreeNode newNode = new CacheTreeNode(subscript, glNode);
                treeNode.Nodes.Add(newNode);
                if (glNode.HasSubnodes())
                {
                    buildTree(glNode, newNode);
                    glNode.GoParentNodeSubscripts();
                }
            } while (glNode.NextSubscript() != "");
        }


        public static void CloneNodeOnTheSameLevel(string Clonename, TrueNodeReference glNode, CacheTreeNode tree)
        {
            /*int c = glNode.GetSubscriptCount() - 1;
            String[] treepath = CacheTreeNode.GetSubscripts(glNode);
            CacheTreeNode cnd;
            int temp = tree.Nodes.IndexOfKey(treepath[0]);
            for(int i = 1; i < treepath.Length; i++)
            {
                //cnd = temp;
                //temp = (CacheTreeNode)tree.Nodes[treepath[i]];
            }
            glNode.SetSubscriptCount(c);
            glNode.AppendSubscript(Clonename);

            CloneNodes(glNode, tree);
            throw new NotImplementedException("CLONE NODE ON THE SAME LEVEL IS NOT IMPLEMENTED=(");*/
        }
        private static void CloneNodes(NodeReference glNode, CacheTreeNode tree)
        {
            string subscript;
            do
            {
                subscript = glNode.NextSubscript();
                glNode.SetSubscriptCount(glNode.GetSubscriptCount() - 1);
                glNode.AppendSubscript(subscript);
                if (glNode.HasSubnodes())
                {
                    glNode.SetSubscriptCount(glNode.GetSubscriptCount() - 1);
                }
            } while (glNode.NextSubscript() != "");
        }
    }
}
