using System.Xml.Serialization;
namespace SharpGenerator.Documentation;
#nullable enable
#pragma warning disable CS8618
[XmlRootAttribute("class", IsNullable = false)]
public record Class
{
	[XmlAttribute] public string name;
	[XmlAttribute] public string inherits;
	[XmlAttribute] public string version;
	public string brief_description;
	public string description;
	public Link[] tutorials;
	public Method[] methods;
	public Member[] members;
	public Signal[] signals;
	public Constant[] constants;
}

[XmlRootAttribute("class", IsNullable = false)]
public record BuiltinClass
{
	[XmlAttribute] public string name;
	[XmlAttribute] public string version;
	public string brief_description;
	public string description;
	public Link[] tutorials;
	public Constructor[] constructors;
	public Method[] methods;
	public Member[] members;
	public Constant[] constants;
	public Operator[] operators;
}

[XmlType("link")]
public record Link
{
	[XmlAttribute] public string title;
	[XmlText] public string link;
}

[XmlType("method")]
public record Method
{
	[XmlAttribute] public string name;
	[XmlAttribute] public string? qualifiers;
	public Return @return;
	[XmlElement("param")] public Parameter[]? parameters;
	public string description;
}

[XmlType("return")]
public record Return
{
	[XmlAttribute] public string type;
}

[XmlType("param")]
public record Parameter
{
	[XmlAttribute] public int index;
	[XmlAttribute] public string name;
	[XmlAttribute] public string type;
}

[XmlType("constant")]
public record Constant
{
	[XmlAttribute] public string name;
	[XmlAttribute] public string value;
	[XmlAttribute] public string? @enum;
	[XmlText] public string? comment;
}

[XmlType("member")]
public record Member
{
	[XmlAttribute] public string name;
	[XmlAttribute] public string type;
	[XmlAttribute] public string setter;
	[XmlAttribute] public string getter;
	[XmlAttribute] public string? @enum;
	[XmlAttribute] public string? @default;
	[XmlText] public string comment;
}

[XmlType("signal")]
public record Signal
{
	[XmlAttribute] public string name;
	[XmlArray("param")] public Parameter[] parameters;
	public string description;
}

[XmlType("constructor")]
public record Constructor
{
	[XmlAttribute] public string name;
	public Return @return;
	[XmlElement("param")] public Parameter[]? parameters;
	public string description;
}

[XmlType("operator")]
public record Operator
{
	[XmlAttribute] public string name;
	public Return @return;
	[XmlArray("param")] public Parameter[] parameters;
	public string description;
}
#pragma warning restore CS8618
