using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace keep.grass
{
	public static class XElementEx
	{
		public static IEnumerable<XElement> LocalElements(this XElement Self, string LocalName)
		{
			return Self.Elements().Where(i => i.Name.LocalName == LocalName);
		}
		public static XElement LocalElement(this XElement Self, string LocalName)
		{
			return Self.LocalElements(LocalName).FirstOrDefault();
		}
	}
}

