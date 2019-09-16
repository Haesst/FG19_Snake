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

    /// <summary>
    /// Get the first ListNode object in the list.
    /// </summary>
    /// <returns>The first ListNode object.</returns>
    public ListNode GetFirst()
    {
        return head;
    }

    /// <summary>
    /// Get the last ListNode objecct in the list.
    /// It runs through every node in the list until
    /// it gets to one that has NextNode set to null.
    /// That is the last object in the list.
    /// </summary>
    /// <returns>The last ListNode object in the list.</returns>
    public ListNode GetLast()
    {
        ListNode currentNode = head;

        while (currentNode.NextNode != null)
        {
            currentNode = currentNode.NextNode;
        }
        
        return currentNode;
    }

    /// <summary>
    /// Insert a GameObject in the first position of
    /// the list. Create a new ListNode with the NextNode
    /// set to the current head. Then update head to point
    /// to this new node.
    /// </summary>
    /// <param name="value">A gameobject that should be inserted first.</param>
    public void InsertFirst(GameObject value)
    {
        ListNode newNode = new ListNode(value, head);
        head = newNode;
        Count++;
    }

    /// <summary>
    /// A method to insert after a given ListNode. Very similar
    /// to add first but instead of head we set the new ListNode
    /// to point toward the given ListNodes NextNode and then set
    /// the given ListNodes NextNode to the new node.
    /// </summary>
    /// <param name="value">Gameobject to be inserted.</param>
    /// <param name="node">The ListNode that the given GameObject should be after in the list.</param>
    public void InsertAfter(GameObject value, ListNode node)
    {
        ListNode newNode = new ListNode(value, node.NextNode);
        node.NextNode = newNode;
        Count++;
    }

    /// <summary>
    /// Remove the Node at given position.
    /// First check if the given position is either less than 0 or
    /// more than the current count, if it is throw an IndexOutOfRangeException.
    /// If it's not then we can check if the position we want if 0 because if it
    /// is we can return the head. If it's not then we just loop through the list
    /// until we're at the given position and then set the previous node's NextNode
    /// to point toward the node that we want to deletes NextNode.
    /// </summary>
    /// <param name="position">The position of the node we want to delete.</param>
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