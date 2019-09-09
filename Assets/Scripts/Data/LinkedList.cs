using System;
using UnityEngine;

public class LinkedList
{
    public class ListNode
    {
        public GameObject Value { get; set; }
        public ListNode NextNode { get; set; }

        public ListNode(GameObject value, ListNode nextNode)
        {
            Value = value;
            NextNode = nextNode;
        }
    }

    private ListNode head;

    public int Count { get; private set; }

    public ListNode GetFirst()
    {
        return head;
    }

    public ListNode GetLast()
    {
        ListNode currentNode = head;

        while (currentNode.NextNode != null)
        {
            currentNode = currentNode.NextNode;
        }
        
        return currentNode;
    }

    public void InsertFirst(GameObject value)
    {
        ListNode newNode = new ListNode(value, head);
        head = newNode;
        Count++;
    }

    public void InsertAfter(GameObject value, ListNode node)
    {
        ListNode newNode = new ListNode(value, node.NextNode);
        node.NextNode = newNode;
        Count++;
    }

    public void RemoveAt(int position)
    {
        if(position < 0 || position > Count)
        {
            throw new IndexOutOfRangeException();
        }
        else
        {
            if(position == 0)
            {
                head = head.NextNode;
            }
            else
            {
                ListNode previousNode = null;
                ListNode currentNode = head;
                for(int i = 0; i < position; i++)
                {
                    previousNode = currentNode;
                    currentNode = currentNode.NextNode;
                }

                previousNode.NextNode = currentNode.NextNode;
            }

            Count--;
        }
    }
}