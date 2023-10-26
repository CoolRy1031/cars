// See https://aka.ms/new-console-template for more information
namespace Cars
{
  class Program
  {
    static void Main (string[] args)
    {
      var cars = ProcessFile("fuel.csv");
      var manufacturers = ProcessManufacturers("manufacturers.csv");
      // Join between 2 tables
      // var query = cars.Join(manufacturers, 
      //                           c => new {c.Manufacturer, c.Year}, 
      //                           m => new { Manufacturer = m.Name, m.Year},
      //                           (c,m) => new
      //                           {
      //                             m.Headquarters,
      //                             c.Name,
      //                             c.Combined
      //                           })
      //                           .OrderByDescending(c => c.Combined)
      //                           .ThenBy(c => c.Name);
        // var query = manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer, (m,g) => 
        //   new {
        //         Manufacturer = m,
        //         Cars = g
        //       })
        //       .GroupBy(m => m.Manufacturer.Headquarters);

        var query = cars.GroupBy( c => c.Manufacturer).Select(g =>
                            {
                              var results = g.Aggregate( new CarStatisitics(), (acc, c) => acc.Accumulate(c),
                              acc => acc.Compute());
                              return new
                              {
                                Name = g.Key,
                                Avg = results.Average,
                                Min = results.Min,
                                Max = results.Max
                              };
                            }).OrderByDescending(r => r.Max);

        foreach (var result in query)
        {
          Console.WriteLine($"{result.Name}");
          Console.WriteLine($"\t Max: {result.Max}");
          Console.WriteLine($"\t Min: {result.Min}");
          Console.WriteLine($"\t Avg: {result.Avg}");
        }

        // foreach (var group in query)
        // {
        //   Console.WriteLine($"{group.Key}");
        //   foreach(var car in group.SelectMany(g =>g.Cars).OrderByDescending(c => c.Combined).Take(3))
        //   {
        //     Console.WriteLine($"\t{car.Name} : {car.Combined}");
        //   }
        // }

      // var query = cars.OrderByDescending(c => c.Combined).ThenBy(c => c.Name);
      var top = cars.Where(c => c.Manufacturer == "BMW" && c.Year == 2016).OrderByDescending(c => c.Combined).ThenBy(c=> c.Name).Select(c => c).Last();
      // var result = cars.Select( c => new {c.Manufacturer, c.Name, c.Combined});
      // var result = cars.SelectMany( c =>  c.Name).OrderBy(c => c);
      // var result = cars.Any(c => c.Manufacturer == "Ford");
      Console.WriteLine(top.Name);
      // Console.WriteLine(result);
      // foreach(var car in query.Take(10))
      // {
      //   Console.WriteLine($" {car.Headquarters} {car.Name} : {car.Combined}");
      // }
      // foreach (var name in result)
      // {
      //   foreach(var character in name)
      //   {
      //     Console.WriteLine(character);
      //   }
      // }
    }

    private static List<Car> ProcessFile(string path)
    {
      var query = File.ReadAllLines(path).Skip(1).Where( line => line.Length > 1).ToCar();
      return query.ToList();
      // return File.ReadAllLines(path).Skip(1).Where(line => line.Length > 1).Select(Car.ParseFromCsv).ToList();
    }
        public static List<Manufacturer> ProcessManufacturers(string path)
    {
      var query = File.ReadAllLines(path).Where(line => line.Length > 1).Select(line =>
      {
        var columns = line.Split(",");
        return new Manufacturer
        {
          Name = columns[0],
          Headquarters = columns[1],
          Year = int.Parse(columns[2])
        };
      });
      return query.ToList();
    }
  }
  public class CarStatisitics
  {
    public CarStatisitics()
    {
      Max = Int32.MinValue;
      Min = Int32.MaxValue;
    }
    public CarStatisitics Accumulate(Car car)
    {
      Count += 1;
      Total += car.Combined;
      Max = Math.Max(Max, car.Combined);
      Min = Math.Min(Min, car.Combined);
      return this;
    }

    public CarStatisitics Compute()
    {
      Average = Total / Count;
      return this;
    }

    public int Max {get; set;}
    public int Min {get; set;}
    public double Average {get; set;}
    public int Total {get; set;}
    public int Count {get; set;}
  }
  public static class CarExtensions
  {
    public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
    {
      foreach (var line in source)
      {
          var columns = line.Split(",");
          yield return new Car
        {
          Year = int.Parse(columns[0]),
          Manufacturer = columns[1],
          Name = columns[2],
          Displacement = double.Parse(columns[3]),
          Cylinders = int.Parse(columns[4]),
          City = int.Parse(columns[5]),
          Highway = int.Parse(columns[6]),
          Combined = int.Parse(columns[7])
        
        };
      }
    }
  }
}
