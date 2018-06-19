/// <summary>
/// One-way linked list class to store car information
/// </summary>
public class CarList {

    public int Count { get; private set; } // number of elements that currently are in the list
    private ListElement FirstElement; // reference to the first list element
    private ListElement LastElement; // reference to the last list element
    private ListElement CurrentElement; // reference to the current list element

    /// <summary>
    /// Adds a car to the end of the list
    /// </summary>
    /// <param name="data">car information to add to the list</param>
    public void AddCar(CarData data) {
        ListElement newElement = new ListElement(data, null);

        if (FirstElement == null) {
            FirstElement = newElement;
            LastElement = newElement;
        } else {
            LastElement.NextElement = newElement;
            LastElement = newElement;
        }

        Count++;
    }

    /// <summary>
    /// Gets data from the element pointed to by CurrentElement field
    /// </summary>
    /// <returns>CarData object from the current element</returns>
    public CarData GetCurrentElement() {
        if (CurrentElement != null) {
            return CurrentElement.Data;
        }

        return null;
    }
    
    /// <summary>
    /// Moves to the next element in the list by making CurrentElement field point to
    /// the next element, moves to the first element if current element is null
    /// </summary>
    /// <returns>true if successfully moved to the next element or the first one, false
    /// if the end of the list is reached</returns>
    public bool MoveToNextElement() {
        if (CurrentElement != null) {
            if (CurrentElement.NextElement != null) {
                CurrentElement = CurrentElement.NextElement;
                return true;
            } else {
                CurrentElement = null;
                return false;
            }
        } else {
            CurrentElement = FirstElement;
            return true;
        }
    }

    /// <summary>
    /// Moves to the first element of the list by making CurrentElement field point to
    /// FirstElement field
    /// </summary>
    /// <returns>true is successfully moved to the first element</returns>
    public bool MoveToFirstElement() {
        if (FirstElement != null) {
            CurrentElement = FirstElement;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Sorts the list using bubble sort algorithm in ascending order
    /// Operators overloaded by the ListElement class are used when sorting
    /// </summary>
    public void SortList() {
        bool swapped = false;

        do {
            swapped = false;

            if (MoveToFirstElement()) {
                do {
                    if (CurrentElement.NextElement != null && CurrentElement > CurrentElement.NextElement) {
                        SwapElements(CurrentElement, CurrentElement.NextElement);
                        swapped = true;
                    }
                } while (MoveToNextElement());
            }
        } while (swapped);
    }

    /// <summary>
    /// Swaps the data of two list elements
    /// </summary>
    /// <param name="firstElement">first element to swap</param>
    /// <param name="secondElement">second element to swap</param>
    private void SwapElements(ListElement firstElement, ListElement secondElement) {
        CarData temp = firstElement.Data;
        firstElement.Data = secondElement.Data;
        secondElement.Data = temp;
    }
}