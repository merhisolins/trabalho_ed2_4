using System;

namespace MeuProjeto
{
    public class BPlusTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private class Node
        {
            public int KeyCount;      
            public bool IsLeaf;
            public TKey[] Keys;       
            public Node[] Children;   
            public TValue[] Values;   
            public Node Parent;
            public Node NextLeaf;

            public Node(int d, bool isLeaf)
            {
                IsLeaf = isLeaf;
                Keys = new TKey[2 * d + 1];        
                Children = new Node[2 * d + 2];    
                Values = isLeaf ? new TValue[2 * d + 1] : null;
            }
        }

        private readonly int _d;
        private Node _root;

        public int Ordem => _d;

        public BPlusTree(int d = 2)
        {
            if (d < 1)
                throw new ArgumentException("Ordem d deve ser >= 1.");

            _d = d;
            _root = null;
        }

        
        public void ForEachInOrder(Action<TKey, TValue> visit)
        {
            if (_root == null || visit == null)
                return;

            Node leaf = GetLeftmostLeaf();
            while (leaf != null)
            {
                for (int i = 0; i < leaf.KeyCount; i++)
                {
                    visit(leaf.Keys[i], leaf.Values[i]);
                }
                leaf = leaf.NextLeaf;
            }
        }

        private Node GetLeftmostLeaf()
        {
            if (_root == null)
                return null;

            Node node = _root;
            while (!node.IsLeaf)
            {
                node = node.Children[0];
            }
            return node;
        }

        

        private (bool found, Node leaf, int pos) SearchNode(TKey key)
        {
            if (_root == null)
                return (false, null, -1);

            Node node = _root;

            // desce até folha
            while (!node.IsLeaf)
            {
                int i = 0;
                while (i < node.KeyCount && key.CompareTo(node.Keys[i]) >= 0)
                    i++;

                node = node.Children[i];
            }

            int pos = 0;
            while (pos < node.KeyCount && node.Keys[pos].CompareTo(key) < 0)
                pos++;

            if (pos < node.KeyCount && node.Keys[pos].CompareTo(key) == 0)
                return (true, node, pos);

            return (false, node, pos);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var (found, leaf, pos) = SearchNode(key);
            if (found)
            {
                value = leaf.Values[pos];
                return true;
            }
            value = default;
            return false;
        }

       

        public void Insert(TKey key, TValue value)
        {
            var (found, leaf, pos) = SearchNode(key);

            if (found)
            {
                return;
            }

            if (leaf == null)
            {
                _root = new Node(_d, isLeaf: true);
                _root.Keys[0] = key;
                _root.Values[0] = value;
                _root.KeyCount = 1;
                return;
            }

            InsertInLeaf(leaf, key, value, pos);

            if (leaf.KeyCount > 2 * _d)
                SplitLeafCorrected(leaf);
        }

        private void InsertInLeaf(Node leaf, TKey key, TValue value, int pos)
        {
            int i = leaf.KeyCount;

            while (i > pos)
            {
                leaf.Keys[i] = leaf.Keys[i - 1];
                leaf.Values[i] = leaf.Values[i - 1];
                i--;
            }

            leaf.Keys[pos] = key;
            leaf.Values[pos] = value;
            leaf.KeyCount++;
        }

        private void SplitLeafCorrected(Node leaf)
        {
            Node q = new Node(_d, isLeaf: true);
            q.Parent = leaf.Parent;

            q.KeyCount = _d + 1; // d+1

            for (int j = 0; j < q.KeyCount; j++)
            {
                q.Keys[j] = leaf.Keys[_d + j];
                q.Values[j] = leaf.Values[_d + j];
            }

            leaf.KeyCount = _d;

            q.NextLeaf = leaf.NextLeaf;
            leaf.NextLeaf = q;

            TKey k = q.Keys[0];

            InsertIntoParent(leaf, k, q);
        }

        private void InsertIntoParent(Node left, TKey keyToInsert, Node right)
        {
            if (left.Parent == null)
            {
                Node newRoot = new Node(_d, isLeaf: false);
                newRoot.Keys[0] = keyToInsert;
                newRoot.Children[0] = left;
                newRoot.Children[1] = right;
                newRoot.KeyCount = 1;

                left.Parent = newRoot;
                right.Parent = newRoot;

                _root = newRoot;
                return;
            }

            Node parent = left.Parent;

            int iPos = 0;
            while (iPos <= parent.KeyCount && parent.Children[iPos] != left)
                iPos++;

            for (int j = parent.KeyCount; j > iPos; j--)
            {
                parent.Keys[j] = parent.Keys[j - 1];
                parent.Children[j + 1] = parent.Children[j];
            }

            parent.Keys[iPos] = keyToInsert;
            parent.Children[iPos + 1] = right;
            parent.KeyCount++;
            right.Parent = parent;

            if (parent.KeyCount > 2 * _d)
                SplitInternal(parent);
        }

        private void SplitInternal(Node node)
        {
            Node q = new Node(_d, isLeaf: false);
            q.Parent = node.Parent;

            int middle = _d;
            TKey keyUp = node.Keys[middle];

            int qKeyCount = node.KeyCount - middle - 1;
            q.KeyCount = qKeyCount;

            int j = 0;
            for (int i = middle + 1; i <= node.KeyCount - 1; i++)
            {
                q.Keys[j] = node.Keys[i];
                q.Children[j] = node.Children[i];
                if (q.Children[j] != null)
                    q.Children[j].Parent = q;
                j++;
            }

            q.Children[j] = node.Children[node.KeyCount];
            if (q.Children[j] != null)
                q.Children[j].Parent = q;

            node.KeyCount = middle;

            InsertIntoParent(node, keyUp, q);
        }

        public void Remove(TKey key)
        {
            var (found, leaf, pos) = SearchNode(key);
            if (!found || leaf == null)
                return;

            RemoveFromLeaf(leaf, pos);

            if (leaf == _root)
            {
                if (leaf.KeyCount == 0)
                    _root = null;
                return;
            }

            if (leaf.KeyCount >= _d)
                return;

            FixLeafUnderflow(leaf);
        }

        private void RemoveFromLeaf(Node leaf, int pos)
        {
            for (int i = pos; i < leaf.KeyCount - 1; i++)
            {
                leaf.Keys[i] = leaf.Keys[i + 1];
                leaf.Values[i] = leaf.Values[i + 1];
            }
            leaf.KeyCount--;
        }

        private void FixLeafUnderflow(Node leaf)
        {
            Node parent = leaf.Parent;

            int i = 0;
            while (i <= parent.KeyCount && parent.Children[i] != leaf)
                i++;

            Node leftSibling = (i > 0) ? parent.Children[i - 1] : null;
            Node rightSibling = (i < parent.KeyCount) ? parent.Children[i + 1] : null;

            if (leftSibling != null && leftSibling.KeyCount > _d)
            {
                RedistributeLeaves(parent, leftSibling, leaf, i - 1);
                return;
            }

            if (rightSibling != null && rightSibling.KeyCount > _d)
            {
                RedistributeLeaves(parent, leaf, rightSibling, i);
                return;
            }

            if (leftSibling != null)
                ConcatenateLeaves(parent, leftSibling, leaf, i - 1);
            else if (rightSibling != null)
                ConcatenateLeaves(parent, leaf, rightSibling, i);
        }

        private void RedistributeLeaves(Node parent, Node leftLeaf, Node rightLeaf, int separatorIndex)
        {
            if (leftLeaf.KeyCount > rightLeaf.KeyCount)
            {
                TKey moveKey = leftLeaf.Keys[leftLeaf.KeyCount - 1];
                TValue moveValue = leftLeaf.Values[leftLeaf.KeyCount - 1];

                InsertInLeaf(rightLeaf, moveKey, moveValue, 0);
                RemoveFromLeaf(leftLeaf, leftLeaf.KeyCount - 1);

                parent.Keys[separatorIndex] = rightLeaf.Keys[0];
            }
            else
            {
                TKey moveKey = rightLeaf.Keys[0];
                TValue moveValue = rightLeaf.Values[0];

                InsertInLeaf(leftLeaf, moveKey, moveValue, leftLeaf.KeyCount);
                RemoveFromLeaf(rightLeaf, 0);

                parent.Keys[separatorIndex] = rightLeaf.Keys[0];
            }
        }

        private void ConcatenateLeaves(Node parent, Node leftLeaf, Node rightLeaf, int separatorIndex)
        {
            for (int j = 0; j < rightLeaf.KeyCount; j++)
            {
                leftLeaf.Keys[leftLeaf.KeyCount + j] = rightLeaf.Keys[j];
                leftLeaf.Values[leftLeaf.KeyCount + j] = rightLeaf.Values[j];
            }
            leftLeaf.KeyCount += rightLeaf.KeyCount;

            leftLeaf.NextLeaf = rightLeaf.NextLeaf;

            RemoveInternalEntry(parent, separatorIndex);
        }

        private void RemoveInternalEntry(Node node, int keyIndex)
        {
            for (int i = keyIndex; i < node.KeyCount - 1; i++)
            {
                node.Keys[i] = node.Keys[i + 1];
                node.Children[i + 1] = node.Children[i + 2];
            }
            node.KeyCount--;

            if (node == _root)
            {
                if (node.KeyCount == 0)
                {
                    Node child = node.Children[0];
                    if (child != null)
                    {
                        child.Parent = null;
                        _root = child;
                    }
                    else
                    {
                        _root = null;
                    }
                }
                return;
            }

            if (node.KeyCount < _d)
                FixInternalUnderflow(node);
        }

        private void FixInternalUnderflow(Node node)
        {
            Node parent = node.Parent;

            int i = 0;
            while (i <= parent.KeyCount && parent.Children[i] != node)
                i++;

            Node leftSibling = (i > 0) ? parent.Children[i - 1] : null;
            Node rightSibling = (i < parent.KeyCount) ? parent.Children[i + 1] : null;

            if (leftSibling != null && leftSibling.KeyCount > _d)
            {
                for (int j = node.KeyCount; j > 0; j--)
                {
                    node.Keys[j] = node.Keys[j - 1];
                    node.Children[j + 1] = node.Children[j];
                }
                node.Children[1] = node.Children[0];

                node.Keys[0] = parent.Keys[i - 1];
                node.Children[0] = leftSibling.Children[leftSibling.KeyCount];
                if (node.Children[0] != null)
                    node.Children[0].Parent = node;
                node.KeyCount++;

                parent.Keys[i - 1] = leftSibling.Keys[leftSibling.KeyCount - 1];

                leftSibling.Children[leftSibling.KeyCount] = null;
                leftSibling.KeyCount--;

                return;
            }

            if (rightSibling != null && rightSibling.KeyCount > _d)
            {
                node.Keys[node.KeyCount] = parent.Keys[i];
                node.Children[node.KeyCount + 1] = rightSibling.Children[0];
                if (node.Children[node.KeyCount + 1] != null)
                    node.Children[node.KeyCount + 1].Parent = node;
                node.KeyCount++;

                parent.Keys[i] = rightSibling.Keys[0];

                for (int j = 0; j < rightSibling.KeyCount - 1; j++)
                {
                    rightSibling.Keys[j] = rightSibling.Keys[j + 1];
                    rightSibling.Children[j] = rightSibling.Children[j + 1];
                }
                rightSibling.Children[rightSibling.KeyCount - 1] = rightSibling.Children[rightSibling.KeyCount];
                rightSibling.Children[rightSibling.KeyCount] = null;
                rightSibling.KeyCount--;

                return;
            }

            if (leftSibling != null)
                MergeInternal(parent, leftSibling, node, i - 1);
            else if (rightSibling != null)
                MergeInternal(parent, node, rightSibling, i);
        }

        private void MergeInternal(Node parent, Node leftNode, Node rightNode, int separatorIndex)
        {
            int k = leftNode.KeyCount;
            leftNode.Keys[k] = parent.Keys[separatorIndex];
            k++;

            for (int j = 0; j < rightNode.KeyCount; j++)
            {
                leftNode.Keys[k] = rightNode.Keys[j];
                leftNode.Children[k] = rightNode.Children[j];
                if (leftNode.Children[k] != null)
                    leftNode.Children[k].Parent = leftNode;
                k++;
            }

            leftNode.Children[k] = rightNode.Children[rightNode.KeyCount];
            if (leftNode.Children[k] != null)
                leftNode.Children[k].Parent = leftNode;

            leftNode.KeyCount = k;

            RemoveInternalEntry(parent, separatorIndex);
        }
    }
}
