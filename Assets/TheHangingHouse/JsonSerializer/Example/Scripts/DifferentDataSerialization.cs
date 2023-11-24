using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheHangingHouse.JsonSerializer;
using System;

public class DifferentDataSerialization : MonoBehaviourID
{
    [JsonSerializeField(DataName = "People")]
    public Person[] persons = new Person[1]
    {
        new Person{name = "mohammad", age = 19}
    };

    [JsonSerializeField(DataName = "Books")]
    public Book[] books = new Book[1]
    {
        new Book{title = "Math", pagesCount = 200}
    };

    [JsonSerializeField(DataName = "People")]
    public int peopleCount = 10;

    [Serializable]
    public class Person
    {
        public string name;
        public float age;
    }

    [Serializable]
    public class Book
    {
        public string title;
        public int pagesCount;
    }
}
