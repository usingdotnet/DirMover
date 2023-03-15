namespace UsingDotNET.DirMover.Models;

public class Person(string name, int age)
{
    public string Name { get; set; } = name;

    public int Age { get; set; } = age;

    public Person() : this("Jack", 16)
    {
    }
}