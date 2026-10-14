class Driver
{
    public string Name { get; set; }
}

class Car
{
    public string Model { get; set; }

    // Ассоциация: у машины может быть водитель, но он передается извне
    public Driver CurrentDriver { get; set; }
}

// Использование:
class Program
{
    static void Main()
    {
        Driver ivan = new Driver { Name = "Иван" };
        Car bmw = new Car { Model = "BMW" };

        // Машина "знает" о водителе, но создавались они абсолютно отдельно
        bmw.CurrentDriver = ivan;
    }
}