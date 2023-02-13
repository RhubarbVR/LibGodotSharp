using System;
using System.Text;

namespace Generators
{
	static class Renamer
	{

		public static string ToSnake(string name)
		{
			var res = "";
			if (name.StartsWith("_"))
			{
				res += "_";
				name = name.Substring(1);
			}
			var upper = false;
			var last = name[0];
			for (var i = 1; i < name.Length; i++)
			{
				var current = name[i];
				if (IsUpper(current))
				{
					res += char.ToLower(last);
					if (upper == false && (i + 1 < name.Length && IsUpper(name[i + 1]) || i + 1 == name.Length))
					{
						res += "_";
					}
					upper = true;
				}
				else
				{
					if (upper)
					{
						res += "_";
					}
					res += char.ToLower(last);
					upper = false;
				}
				last = current;
			}
			return res + char.ToLower(last);
		}

		static bool IsUpper(char c) => char.IsUpper(c) || char.IsDigit(c);
	}
}
