using System;
using System.Xml;

namespace keep.grass
{
	public class Grass
	{
		static private string AtomUrlFormat = "https://github.com/{0}.atom";
		static public string AtomUrl(string Id)
		{
			return String.Format (AtomUrlFormat, Id);
		}
		static public DateTime GetLastPublicActivity(string Id)
		{
			using(var doc = new XmlDocument())
			{
				doc.Load(AtomUrl(Id));
				var eumerator = doc.ChildNodes.GetEnumerator();
				while(eumerator.MoveNext())
				{


					throw new NotImplementedException();
				}
			}
			throw new MissingFieldException();
		}
	}
}

