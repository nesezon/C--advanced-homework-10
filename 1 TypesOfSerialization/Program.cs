using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

namespace TypesOfSerialization {
  public class Program {
    static void Main(string[] args) {
      // SoapFormatter понимает только объекты, которые могут быть созданы с помощью .NET 1.1
      // поэтому вместо List приходится использовать ArrayList
      ArrayList staff = new ArrayList();
      staff.Add(new Employee("Катя", 1000, Employee.role.accountant));
      staff.Add(new Employee("Света", 1500, Employee.role.secretary));
      staff.Add(new Employee("Иннокентий", 2000, Employee.role.manager));
      staff.Add(new Employee("Вася", 2000, Employee.role.director));
      foreach (var item in staff) {
        Console.WriteLine(item);
      }

      // XML
      // второй параметр в XmlSerializer указываю поскольку использую ArrayList
      XmlSerializer XMLserializer = new XmlSerializer(typeof(ArrayList), new Type[] { typeof(Employee) });
      using (FileStream stream = new FileStream("Employees.xml", FileMode.Create)) {
        XMLserializer.Serialize(stream, staff);
      }

      // SOAP
      SoapFormatter soapFormat = new SoapFormatter();
      using (FileStream stream = new FileStream("EmployeesSOAP.xml", FileMode.Create)) {
        soapFormat.Serialize(stream, staff);
      }

      // JSON
      // в данной реализации перечисление Role осталось просто цифрами
      // (Json реализованный через стороннюю библиотеку Newtonsoft.Json ведет себя так же)
      // и здесь в DataContractJsonSerializer добавил второй параметр поскольку использую ArrayList
      DataContractJsonSerializer JSONserializer = new DataContractJsonSerializer(typeof(ArrayList), new Type[] { typeof(Employee) });
      using (FileStream stream = new FileStream("Employees.json", FileMode.Create)) {
        JSONserializer.WriteObject(stream, staff);
      }

      // Binary
      BinaryFormatter binFormat = new BinaryFormatter();
      using (FileStream stream = new FileStream("Employees.dat", FileMode.Create)) {
        binFormat.Serialize(stream, staff);
      }

      // Binary десериализую обратно чтобы убедиться что весь граф объектов восстановился
      ArrayList newStaff;
      using (Stream fStream = File.OpenRead("Employees.dat")) {
        newStaff = (ArrayList)binFormat.Deserialize(fStream);
      }
      Console.WriteLine("После бинарной пересериализации все сохраняется:");
      foreach (var item in newStaff) {
        Console.WriteLine(item);
      }

      Console.ReadKey();
    }

    [Serializable]
    public class Employee {
      public enum role {
        accountant,
        secretary,
        manager,
        director
      }
      public string Name { get; set; }
      public decimal Salary { get; set; }
      public role Role { get; set; }
      private static int _count;
      public int Count {   // read only свойство в XML и SOAP методах не сериализируется,
        get { return _count; }
        set { }            // поэтому добавил пустой сеттер
      }
      public Employee() { }
      public Employee(string nm, decimal sl, role rl) {
        Name = nm;
        Salary = sl;
        Role = rl;
        _count++;
      }
      public override string ToString() {
        return string.Format(format: "{0, -20} {1, -15} {2,-15} Сount={3}", Name, Role, Salary, Count);
      }
    }
  }
}
