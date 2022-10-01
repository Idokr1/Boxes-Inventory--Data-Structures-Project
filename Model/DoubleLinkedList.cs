using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DoubleLinkedList<T>
    {
        public Node Start = null;
        public Node End = null;
        private int count;
        public int Count { get { return count; } }
        
        public DoubleLinkedList() { }    
        public DoubleLinkedList(DoubleLinkedList<T> list)
        {
            Start = list.Start;
            End = list.End;
            count = list.Count;
        }

        /// <summary>
        /// adding a value to the beginning of the list
        /// </summary>
        /// <param name="value">the value we want to add</param>       
        public void AddFirst(T value) //O(1)
        {
            Node newNode = new Node(value);
            if (Start != null)
                Start.prev = newNode;
            newNode.next = Start;
            Start = newNode;
            if (Start.next == null)
            {
                End = newNode;
            }
            count++;
        }

        /// <summary>
        /// removing the first value in the list
        /// </summary>
        /// <param name="removedValue">the value we want to remove</param>   
        public bool RemoveFirst(out T removedValue) //O(1)
        {
            if (Start == null)
            {
                removedValue = default(T);
                return false;
            }
            count--;
            removedValue = Start.Data;
            Start = Start.next;
            if (Start == null)
            {
                End = null;
                return true;
            }
            Start.prev = null;
            return true;
        }

        /// <summary>
        /// adding a value to the end of the list
        /// </summary>
        /// <param name="value">the value we want to add</param>    
        public void AddLast(T value) //O(1)
        {
            if (Start == null)
            {
                AddFirst(value);
                return;
            }
            Node newNode = new Node(value);
            End.next = newNode;
            End.next.prev = End;
            End = End.next;
            count++;
        }

        /// <summary>
        /// removing the last value in the list
        /// </summary>
        /// <param name="removedValue">the value we want to remove</param>   
        public bool RemoveLast(out T removedValue) //O(1)
        {
            if (Start == null)
            {
                removedValue = default(T);
                return false;
            }
            removedValue = End.Data;
            End = End.prev;
            count--;
            if (End == null)
            {
                Start = null;
                return true;
            }
            End.next = null;
            return true;
        }

        /// <summary>
        /// removing an object by his node
        /// </summary>
        /// <param name="node">the object's node</param>
        /// <param name="boxWasDeleted">determines whether an old box was already deleted while the user tarried when he was asked if he wish to buy the box</param>
        public void RemoveByNode(Node node, out bool boxWasDeleted) //O(1)
        {
            boxWasDeleted = true;
            if (End == null)
            {
                boxWasDeleted = false;
                return;
            }
            if (node.Data.Equals(End.Data))
            {
                RemoveLast(out _);
                return;
            }
            if (node.Data.Equals(Start.Data))
            {
                RemoveFirst(out _);
                return;
            }
            node.next.prev = node.prev;
            node.prev.next = node.next;
            count--;
        }

        /// <summary>
        /// repositioning a node
        /// </summary>
        /// <param name="node">the object's node</param>
        public void RePositeToEnd(Node node) //O(1)
        {
            if (node.Data.Equals(End.Data))
            {
                return;
            }
            if (node.Data.Equals(Start.Data))
            {
                Start = node.next;
                node.next.prev = null;
                AddLast(node.Data);
            }
            else
            {
                node.next.prev = node.prev;
                node.prev.next = node.next;
                AddLast(node.Data);
            }
        }

        /// <summary>
        /// checking if the list is empty
        /// </summary>
        public bool IsEmpty()
        {
            return (Start == null);
        }

        public class Node
        {
            public T Data { get; set; }
            public Node next;
            public Node prev;

            public Node(T data)
            {
                this.Data = data;
                next = null;
                prev = null;
            }
        }
    }
}
