﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Xml;
using System.IO;
using QuickGraph.Serialization;
using Xunit;

namespace QuickGraph.Tests.Serialization
{
    public class XmlSerializationTest
    {
        [Fact]
        public void DeserializeFromXml()
        {
            var doc = new XPathDocument(Path.Combine("graphml", "repro12273.xml"));
            var ug = SerializationExtensions.DeserializeFromXml(doc,
                "graph", "node", "edge",
                nav => new UndirectedGraph<string, TaggedEdge<string, double>>(),
                nav => nav.GetAttribute("id", ""),
                nav => new TaggedEdge<string, double>(
                    nav.GetAttribute("source", ""),
                    nav.GetAttribute("target", ""),
                    int.Parse(nav.GetAttribute("weight", ""))
                    )
                );

            var ug2 = SerializationExtensions.DeserializeFromXml(
                XmlReader.Create(Path.Combine("graphml", "repro12273.xml")),
                "graph", "node", "edge", "",
                reader => new UndirectedGraph<string, TaggedEdge<string, double>>(),
                reader => reader.GetAttribute("id"),
                reader => new TaggedEdge<string, double>(
                    reader.GetAttribute("source"),
                    reader.GetAttribute("target"),
                    int.Parse(reader.GetAttribute("weight"))
                    )
                );

            Assert.Equal(ug.VertexCount, ug2.VertexCount);
            Assert.Equal(ug.EdgeCount, ug2.EdgeCount);
        }
    }
}
