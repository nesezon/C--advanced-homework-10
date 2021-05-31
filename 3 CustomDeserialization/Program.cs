using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace CustomDeserialization {
  class Program {
    static void Main(string[] args) {

      // ищу файлы в папке 2-го упражнения
      DirectoryInfo directoryInfo = new DirectoryInfo("../../../2 CustomSerialization");
      FileInfo[] fileAttrNames = directoryInfo.GetFiles("PostsAttributes.xml", SearchOption.AllDirectories);
      FileInfo[] fileTagNames = directoryInfo.GetFiles("PostsTags.xml", SearchOption.AllDirectories);
      if (fileAttrNames.Length == 0 || fileTagNames.Length == 0) {
        Console.WriteLine($"Отсутствуют исходные файлы");
        Console.WriteLine("Чтобы они появились, запустите исполняемый файл второго упражнения");
        // Задержка.
        Console.ReadKey();
        return;
      }

      var Posts = new List<Post>();
      XmlSerializer XMLserializer = new XmlSerializer(typeof(List<Post>));

      Console.WriteLine("Десериализация по тэгам:\r\n");
      using (FileStream stream = new FileStream(fileTagNames[0].FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
        Posts = (List<Post>)XMLserializer.Deserialize(stream);
      }
      foreach (var post in Posts) {
        Console.WriteLine(post);
      }

      Console.WriteLine(new string('-', 10));
      Console.WriteLine();

      Console.WriteLine("Десериализация по атрибутам:\r\n");
      Posts = new List<Post>();
      Post.withAttributes = true;
      using (FileStream stream = new FileStream(fileAttrNames[0].FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
        Posts = (List<Post>)XMLserializer.Deserialize(stream);
      }
      foreach (var post in Posts) {
        Console.WriteLine(post);
      }

      // Задержка.
      Console.ReadKey();
    }
  }

  public class Post : IXmlSerializable {
    public Post() { }
    public static bool withAttributes { get; set; } = false;
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public string Text { get; set; }
    public XmlSchema GetSchema() { return null; }
    public void ReadXml(XmlReader reader) {
      if (withAttributes) {
        // через атрибуты
        Id = int.Parse(reader.GetAttribute("Id"));
        Title = reader.GetAttribute("Title");
        Date = DateTime.ParseExact(reader.GetAttribute("Date"), "G", null);
        Text = reader.GetAttribute("Text");
        reader.ReadStartElement();
      } else {
        // через тэги
        reader.ReadStartElement();
        Id = reader.ReadElementContentAsInt();
        Date = DateTime.ParseExact(reader.ReadElementContentAsString(), "G", null);
        Title = reader.ReadElementContentAsString();
        Text = reader.ReadElementContentAsString();
        reader.ReadEndElement();
      }
    }
    public void WriteXml(XmlWriter writer) { }
    public override string ToString() {
      return Id + ": " + Date.ToString("G") + " " + Title + "\r\n\t" + Text;
    }
  }
}
