using System.Text.RegularExpressions;
using System;

namespace SharpGenerator;
public static class Fixer {

	public static string Type(string name, Api api) {
		if (name.StartsWith("enum::")) {
			name = name[6..];
			if (name.Contains('.'))
			{
                var className = name[..name.IndexOf('.')];
                var enumName = name[(name.IndexOf('.') + 1)..];
                bool isBuiletinClass = api.builtinClasses.Where(x => x.name == className).Cast<Api.BuiltinClass?>().FirstOrDefault() is not null;
                if (className != "Variant" && !isBuiletinClass)
				{
                    var amount = api.classes.Where(x => x.name == className).First().enums.Where(x => x.name == enumName).Count();
                    if (amount == 0)
                    {
                        Program.Warn($"ENUM {name} not found");
                        return "long";
                    }
                }
			}
        }
		name = name.Replace("const ", "");
		if (name.Contains("typedarray::")) {
			return "Array";
		}
		name = name.Replace("::", ".");
		if (name.Contains("VariantType.")) {
			return "Variant";
		}
		if (name.StartsWith("bitfield.")) { name = name.Replace("bitfield.", ""); }
		if (name.StartsWith("uint64_t")) { name = name.Replace("uint64_t", "UInt64"); }
		if (name.StartsWith("uint16_t")) { name = name.Replace("uint16_t", "UInt16"); }
		if (name.StartsWith("uint8_t")) { name = name.Replace("uint8_t", "byte"); }
		if (name.StartsWith("int32_t")) { name = name.Replace("int32_t", "int"); }
		if (name.StartsWith("real_t")) { name = name.Replace("real_t", "float"); }
		if (name.StartsWith("float")) { name = name.Replace("float", "double"); }
		if (name.StartsWith("int")) { name = name.Replace("int", "long"); }
		if (name.StartsWith("String")) { name = name.Replace("int", "string"); }
		if (name.StartsWith("VariantType")) { name = name.Replace("VariantType", "Variant.Type"); }

		return name;
	}

	public static string MethodName(string name) {
		var res = "";
		var parts = name.Split(".");
		for (var i = 0; i < parts.Length - 1; i++) {
			res += parts[i] + ".";
		}
		var last = Fixer.SnakeToPascal(parts[^1]);
		return res + last switch {
			"GetType" => "GetTypeGD",
			_ => last,
		};
	}

	public static string Name(string name) {
		return name switch {
			"object" => "@object",
			"base" => "@base",
			"interface" => "@interface",
			"class" => "@class",
			"default" => "@default",
			"char" => "@char",
			"string" => "@string",
			"event" => "@event",
			"lock" => "@lock",
			"operator" => "@operator",
			"enum" => "@enum",
			"in" => "@in",
			"out" => "@out",
			"checked" => "@checked",
			"override" => "@override",
			"new" => "@new",
			"params" => "@params",
			"internal" => "@internal",
			"bool" => "@bool",
			_ => name,
		};
	}

	public static string VariantOperator(string type) {
		return type switch {
			"==" => "Equal",
			"!=" => "NotEqual",
			"<" => "Less",
			"<=" => "LessEqual",
			">" => "Greater",
			">=" => "GreaterEqual",
			/* mathematic */
			"+" => "Add",
			"-" => "Subtract",
			"*" => "Multiply",
			"/" => "Divide",
			"unary-" => "Negate",
			"unary+" => "Positive",
			"%" => "Module",
			"**" => "Power",
			/* bitwise */
			"<<" => "ShiftLeft",
			">>" => "ShiftRight",
			"&" => "BitAnd",
			"|" => "BitOr",
			"^" => "BitXor",
			"!" => "BitNegate",
			/* logic */
			"and" => "And",
			"or" => "Or",
			"xor" => "Xor",
			"not" => "Not",
			/* containment */
			"in" => "In",
			_ => type,
		};
	}

	public static string VariantName(string name) {
		return name switch {
			"int" => "Int",
			"float" => "Float",
			"bool" => "Bool",
			_ => name,
		};
	}

	public static string Value(string value) {
		if (value.Contains('(')) {
			value = "new " + value;
		};
		value = value.Replace("inf", "double.PositiveInfinity");
		return value;
	}

	public static string SnakeToPascal(string name) {
		var res = "";
		foreach (var w in name.Split('_')) {
			if (w.Length == 0) {
				res += "_";
			} else {
				res += w[0].ToString().ToUpper() + w[1..].ToLower();
			}
		}
		for (var i = 0; i < res.Length - 1; i++) {
			if (char.IsDigit(res[i]) && res[i + 1] == 'd') {
				res = string.Concat(res.AsSpan(0, i + 1), "D", res.AsSpan()[(i + 2)..]);
			}
		}
		return res;
	}

	public static int SharedPrefixLength(string[] names) {
		for (var l = 0; true; l++) {
			if (l >= names[0].Length) { return 0; }
			var c = names[0][l];
			foreach (var name in names) {
				if (l >= name.Length) {
					return 0;
				}
				if (name[l] != c) {
					return l;
				}
			}
		}
	}

	static string XMLConstant(string value) {
		value = SnakeToPascal(value);
		return value switch {
			"@gdscript.nan" => "NaN",
			"@gdscript.tau" => "Tau",
			_ => value,
		};
	}

	static readonly (string, MatchEvaluator)[] xmlReplacements = new (string, MatchEvaluator)[] {
		(@"<", x => "&lt;"),
		(@">", x => "&gt;"),
		(@"&", x => "&amp;"),
		(@"\[b\](?<a>.+?)\[/b\]", x => $"<b>{x.Groups["a"].Captures[0].Value}</b>"),
		(@"\[i\](?<a>.+?)\[/i\]", x => $"<i>{x.Groups["a"].Captures[0].Value}</i>"),
		(@"\[constant (?<a>\S+?)\]", x => $"<see cref=\"{XMLConstant(x.Groups["a"].Captures[0].Value)}\"/>"),
		(@"\[code\](?<a>.+?)\[/code\]", x => $"<c>{x.Groups["a"].Captures[0].Value}</c>"),
		(@"\[param (?<a>\S+?)\]",x => $"<paramref name=\"{x.Groups["a"].Captures[0].Value}\"/>"),
		(@"\[method (?<a>\S+?)\]", x => $"<see cref=\"{MethodName(x.Groups["a"].Captures[0].Value)}\"/>"),
		(@"\[member (?<a>\S+?)\]", x => $"<see cref=\"{x.Groups["a"].Captures[0].Value}\"/>"),
		(@"\[enum (?<a>\S+?)\]",x => $"<see cref=\"{x.Groups["a"].Captures[0].Value}\"/>"),
		(@"\[signal (?<a>\S+?)\]", x => $"<see cref=\"EmitSignal{SnakeToPascal( x.Groups["a"].Captures[0].Value)}\"/>"), //currently just two functions
		(@"\[theme_item (?<a>\S+?)\]", x => $"<see cref=\"{x.Groups["a"].Captures[0].Value}\"/>"), //no clue
		//(@"cref=""Url=\$docsUrl/(?<a>.+?)/>", x => $"href=\"https://docs.godotengine.org/en/stable/{x.Groups["a"].Captures[0].Value}\"/>"),
		(@"\[url=(?<a>.+?)\](?<b>.+?)\[/url]", x => $"<see href=\"{x.Groups["a"].Captures[0].Value}\">{x.Groups["b"].Captures[0].Value}</see>"),
		(@"\[(?<a>\S+?)\]", x => $"<see cref=\"{x.Groups["a"].Captures[0].Value}\"/>"), //can be multiple things
	};

	public static string XMLComment(string comment, int indent = 1) {

		var tabs = new string('\t', count: indent);
		var result = tabs + "/// <summary>\n";
		var lines = comment.Trim().Split("\n");

		for (var i = 0; i < lines.Length; i++) {
			var line = lines[i].Trim();
			if (line.Contains("[codeblock]")) {
				var offset = lines[i].Count(x => x == '\t');
				result += tabs + "/// <code>\n";
				i += 1;
				line = lines[i][offset..];
				while (line.Contains("[/codeblock]") == false) {
					i += 1;
					result += tabs + "/// " + line + "\n";
					while (lines[i].Length <= offset) { i += 1; }
					line = lines[i][offset..];
				}
				result += tabs + "/// </code>\n";
			} else if (line.Contains("[codeblocks]")) {
				while (line.Contains("[/codeblocks]") == false) {
					i += 1;
					line = lines[i].Trim();
					if (line.Contains("[csharp]")) {
						var offset = lines[i].Count(x => x == '\t');
						result += tabs + "/// <code>\n";
						i += 1;
						line = lines[i][offset..];
						while (line.Contains("[/csharp]") == false) {
							i += 1;
							result += tabs + "/// " + line + "\n";
							while (lines[i].Length <= offset) { i += 1; }
							line = lines[i][offset..];
						}
						result += tabs + "/// </code>\n";
					}
				}
			} else {
				foreach (var (pattern, replacement) in xmlReplacements) {
					line = Regex.Replace(line, pattern, replacement);
				}
				result += tabs + "/// " + line + "<br/>" + "\n";
			}
		}
		result += tabs + "/// </summary>";
		return result.ReplaceLineEndings();
	}
}
