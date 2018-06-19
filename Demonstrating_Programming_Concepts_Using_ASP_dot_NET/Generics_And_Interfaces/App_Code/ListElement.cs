/*
 * Author: Giedrius Kristinaitis
 */

/// <summary>
/// List element containing references to previous and next list elements as well as the data
/// stored in the element
/// </summary>
/// <typeparam name="T"></typeparam>
public class ListElement<T> {

    public T Data { get; set; } // data of the element
    public ListElement<T> PreviousElement { get; set; } // previous element in the list
    public ListElement<T> NextElement { get; set; } // next element in the list

    /// <summary>
    /// Class cunstructor
    /// </summary>
    /// <param name="data">data to store in this element</param>
    /// <param name="previousElement">previous element in the list</param>
    /// <param name="nextElement">next element in the list</param>
	public ListElement(T data, ListElement<T> previousElement, ListElement<T> nextElement){
        Data = data;
        PreviousElement = previousElement;
        NextElement = nextElement;
	}
}
