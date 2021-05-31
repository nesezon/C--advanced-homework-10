using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace CustomSerialization {
  class Program {
    static void Main(string[] args) {
      var Posts = new List<Post> {
        new Post {
          Id = 1,
          Date = new DateTime(2021, 5, 31, 8, 30, 0),
          Title = "Intel похвасталась темпами экспансии мобильных процессоров Tiger Lake-H",
          Text = "В три раза быстрее обычного – чем вам не новость про разгон?"
        },
        new Post {
          Id = 2,
          Date = new DateTime(2021, 5, 31, 8, 46, 0),
          Title = "Intel сравнила Tiger Lake-H с восьмиядерным AMD Cezanne в Crysis Remastered",
          Text = "Обоим помогала мобильная NVIDIA GeForce RTX 3080."
        },
        new Post {
          Id = 3,
          Date = new DateTime(2021, 5, 31, 8, 55, 0),
          Title = "Образец ноутбука на базе Intel Alder Lake мелькнул на Computex 2021",
          Text = "Анонс намечен на второе полугодие."
        }
      };

      // Сериализация тэгами
      XmlSerializer XMLserializer = new XmlSerializer(typeof(List<Post>));
      using (FileStream stream = new FileStream("PostsTags.xml", FileMode.Create)) {
        XMLserializer.Serialize(stream, Posts);
      }

      // Сериализация атрибутами
      using (FileStream stream = new FileStream("PostsAttributes.xml", FileMode.Create)) {
        Post.withAttributes = true;
        XMLserializer.Serialize(stream, Posts);
      }
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

    public void ReadXml(XmlReader reader) { }

    public void WriteXml(XmlWriter writer) {
      if (withAttributes) {
        // через атрибуты
        writer.WriteAttributeString("Id", Id.ToString());
        writer.WriteAttributeString("Date", Date.ToString("G"));
        writer.WriteAttributeString("Title", Title);
        writer.WriteAttributeString("Text", Text);
      } else {
        // через тэги
        writer.WriteElementString("Id", Id.ToString());
        writer.WriteElementString("Date", Date.ToString("G"));
        writer.WriteElementString("Title", Title);
        writer.WriteElementString("Text", Text);
      }
    }
  }
}
