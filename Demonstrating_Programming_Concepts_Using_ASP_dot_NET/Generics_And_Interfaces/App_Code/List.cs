/*
 * Author: Giedrius Kristinaitis
 */

using System;
using System.Collections;

/// <summary>
/// Both-way generic type linked list. List elements must implement IComparable interface in order
/// to sort the list
/// </summary>
/// <typeparam name="T"></typeparam>
public class List<T> : IEnumerable {

    private ListElement<T> FirstElement; // first element in the list
    private ListElement<T> LastElement; // second element in the list

    /// <summary>
    /// Adds a new element to the end of the list
    /// </summary>
    /// <param name="data">element to add</param>
    public void Add(T data) {
        if (FirstElement == null) {
            FirstElement = new ListElement<T>(data, null, null);
        } else {
            if (LastElement == null) {
                LastElement = new ListElement<T>(data, FirstElement, null);
                FirstElement.NextElement = LastElement;
            } else {
                ListElement<T> newElement = new ListElement<T>(data, LastElement, null);
                LastElement.NextElement = newElement;
                LastElement = newElement;
            }
        }
    }

    /// <summary>
    /// Checks if the list is empty (has no elements)
    /// </summary>
    /// <returns>true if there are no elements in the list</returns>
    public bool IsEmpty() {
        return FirstElement == null;
    }

    /// <summary>
    /// Gets an enumerator to loop through the list
    /// </summary>
    /// <returns>enumerator that can be used to loop through the list</returns>
    public IEnumerator GetEnumerator() {
        for (ListElement<T> element = FirstElement; element != null; element = element.NextElement) {
            yield return element.Data;
        }
    }

    /// <summary>
    /// Sorts the list elements in ascending order using bubble sort algorithm
    /// List elements must implement IComparable interface for this method to work
    /// </summary>
    public void Sort() {
        bool swapped = false;

        do {
            ListElement<T> currentElement = FirstElement;
            swapped = false;

            if (currentElement != null) {
                do {
                    IComparable currentData = currentElement.Data as IComparable;
                    
                    if (currentElement.NextElement != null && currentData.CompareTo(currentElement.NextElement.Data) > 0) {
                        SwapElements(currentElement, currentElement.NextElement);
                        swapped = true;
                    }
                } while ((currentElement = currentElement.NextElement) != null && currentElement != FirstElement);
            }
        } while (swapped);
    }

    /// <summary>
    /// Swaps two elements in the list
    /// </summary>
    /// <param name="firstElement">first element to swap</param>
    /// <param name="secondElement">second element to swap</param>
    private void SwapElements(ListElement <T>firstElement, ListElement<T> secondElement) {
        T temp = firstElement.Data;
        firstElement.Data = secondElement.Data;
        secondElement.Data = temp;
    }
}
