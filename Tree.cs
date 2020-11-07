using System.Collections.Generic;
using System.Drawing;

namespace AlgorithmsVisual
{
    class TreeNode
    {
        public int Data;
        public Color Color;
        public int Level;
        public TreeNode Left;
        public TreeNode Right;
        public TreeNode(int data)
        {
            Data = data;
        }
        public Color Add(TreeNode node)
        {
            node.Level++;
            node.Color = ColorToReturn(node.Level);
            if (node.Data < Data)
            {
                if (Left == null)
                {
                    Left = node;
                    return ColorToReturn(node.Level);
                }
                else
                    return Left.Add(node);
            }
            else
            {
                if(Right == null)
                {
                    Right = node;
                    return ColorToReturn(node.Level);
                }
                else
                    return Right.Add(node);
            }
        }
        public IEnumerable<TreeNode> VisitTree()
        {
            if (Left != null)
                foreach (var e in Left.VisitTree())
                    yield return e;
            yield return this;
            if (Right != null)
                foreach (var e in Right.VisitTree())
                    yield return e;
        }
        private Color ColorToReturn(int level)
        {
            switch(level)
            {
                case 0:
                    return Color.Red;
                case 1:
                    return Color.Orange;
                case 2:
                    return Color.Yellow;
                case 3:
                    return Color.Green;
                case 4:
                    return Color.LightBlue;
                case 5:
                    return Color.Blue;
                default:
                    return Color.Violet;
            }
        }
    }
    class Tree
    {
        public TreeNode Root;
        public Color Add(TreeNode node)
        {
            if (Root == null)
            {
                node.Level = 0;
                node.Color = Color.Red;
                Root = node;
                return Color.Red;
            }
            else
                return Root.Add(node);
        }
        public IEnumerable<TreeNode> VisitTree()
        {
            return Root.VisitTree();
        }
    }
}
