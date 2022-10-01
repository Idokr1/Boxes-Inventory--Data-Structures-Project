using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class BST<T> where T : IComparable<T>
    {
        private Node root { get; set; }

        /// <summary>
        /// adding an object to the tree
        /// </summary>
        /// <param name="value">the DataX/DataY object we want to add</param>
        public void Add(T value)
        {
            if (root == null)
            {
                root = new Node(value);
                return;
            }

            Node tmp = root;
            while (true)
            {
                if (value.CompareTo(tmp.data) < 0)
                {
                    if (tmp.left == null) //adding value to the left and return
                    {
                        tmp.left = new Node(value);
                        tmp.left.up = tmp;
                        break;
                    }
                    tmp = tmp.left;
                }
                else
                {
                    if (tmp.right == null) //adding value to the right and return
                    {
                        tmp.right = new Node(value);
                        tmp.right.up = tmp;
                        break;
                    }
                    tmp = tmp.right;
                }
            }
        }

        /// <summary>
        /// deleting an item from the tree by the conventions of deleting
        /// </summary>
        /// <param name="data">the dataX/DataY object we want to delete</param>        
        public bool Delete(T data) 
        {
            Node tmp = root;
            if (tmp == null)
                return false;
            while (tmp.data.CompareTo(data) != 0) //finding the node that we want to remove
            {
                if (tmp.data.CompareTo(data) > 0)                
                    tmp = tmp.left;                
                else                
                    tmp = tmp.right;                
                if (tmp == null)
                    return false;
            }
            if (tmp.right == null && tmp.left == null) //leaf
            {
                if (tmp.up == null) //removing root
                {
                    root = null;
                    return true;
                }
                if (tmp.up.data.CompareTo(tmp.data) > 0)                
                    tmp.up.left = null;                
                else                
                    tmp.up.right = null;                
                tmp.up = null;
            }
            if (tmp.right == null && tmp.left != null) //one side to handle
            {
                if (tmp.up == null) //removing root
                {
                    root = tmp.left;
                    root.up = null;
                    return true;
                }
                if (tmp.up.data.CompareTo(tmp.data) > 0)
                {
                    tmp.up.left = tmp.left;
                    tmp.left.up = tmp.up;
                }
                else
                {
                    tmp.up.right = tmp.left;
                    tmp.left.up = tmp.up;
                }
            }
            if (tmp.right != null && tmp.left == null) //one side to handle
            {
                if (tmp.up == null) //removing root
                {
                    root = tmp.right;
                    root.up = null;
                    return true;
                }
                if (tmp.up.data.CompareTo(tmp.data) > 0)
                {
                    tmp.up.left = tmp.right;
                    tmp.right.up = tmp.up;
                }
                else
                {
                    tmp.up.right = tmp.right;
                    tmp.right.up = tmp.up;
                }
            }
            if (tmp.right != null && tmp.left != null) //Two sides to handle
            {
                Node ToBeReplaced = tmp;
                Node replacer;
                tmp = tmp.right;
                while (tmp.left != null)
                {
                    tmp = tmp.left;
                }
                replacer = tmp;
                if (replacer.right == null)
                {
                    ToBeReplaced.data = replacer.data;
                    replacer.up.left = null;
                }
                else
                {
                    ToBeReplaced.data = replacer.data;
                    replacer.up.left = replacer.right;
                    replacer.right.up = replacer.up;
                    replacer.right = null;
                    replacer.up = null;
                }
            }
            return true;
        }

        /// <summary>
        /// searching an equal or bigger item and allways returning something, the specific item we wanted or bigger (null if there is nothing big enough)
        /// </summary>
        /// <param name="itemToSearch">the item we want to search</param>
        /// <param name="foundItem">the item we found, if we found</param>
        public bool SearchEqualOrBigger(T itemToSearch, out T foundItem)
        {
            foundItem = default(T);
            if (root == null)
                return false;
            Node tmp = root;
            return SearchEqualOrBigger(itemToSearch, out foundItem, tmp, default(T));
        }
        private bool SearchEqualOrBigger(T itemToSearch, out T foundItem, Node tmp, T closestNow)
        {            
            foundItem = closestNow;
            if (tmp == null)
                return true;            
            else
            {
                if (tmp.data.CompareTo(itemToSearch) > 0)
                {
                    closestNow = tmp.data;                    
                    return SearchEqualOrBigger(itemToSearch, out foundItem, tmp.left, closestNow);
                }

                else if (tmp.data.CompareTo(itemToSearch) < 0)
                {
                    return SearchEqualOrBigger(itemToSearch, out foundItem, tmp.right, closestNow);
                }
                else
                {
                    foundItem = tmp.data;
                    return true;
                }
            }
        }

        /// <summary>
        /// checking if the depth of the tree is one
        /// </summary>
        public bool IsDepthIsOne()
        {
            return (root.left == null && root.right == null); 
        }

        /// <summary>
        /// searching if an item in the tree exists
        /// </summary>
        /// <param name="itemToSearch">the item we want to search</param>
        /// <param name="foundItem">the item we found, if we found</param>
        /// <returns></returns>
        public bool Search(T itemToSearch, out T foundItem)
        {
            foundItem = default(T);
            if (root == null)
                return false;

            Node tmp = root;
            while (true)
            {
                if (itemToSearch.CompareTo(tmp.data) < 0)
                {
                    tmp = tmp.left;
                    if (tmp == null)
                    {
                        return false;
                    }
                    if (itemToSearch.CompareTo(tmp.data) == 0)
                    {
                        foundItem = tmp.data;
                        return true;
                    }
                }
                else
                {
                    if (itemToSearch.CompareTo(tmp.data) == 0)
                    {
                        foundItem = tmp.data;
                        return true;
                    }
                    tmp = tmp.right;
                    if (tmp == null)
                    {
                        return false;
                    }
                }
            }
        }
        public class Node
        {
            public T data;
            public Node up;
            public Node left;
            public Node right;

            public Node(T data)
            {
                this.data = data;
                left = right = null;
            }
        }
    }
}
