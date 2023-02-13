using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators
{

	public static class Notification
	{

		public static bool Generate(GeneratorExecutionContext context, INamedTypeSymbol c)
		{
			var methods = c.GetMembers().Where(x => x is IMethodSymbol).Select(x => (IMethodSymbol)x);

			var code = $$"""
			using System.Runtime.CompilerServices;
			using System.Runtime.InteropServices;
			using GDExtension;
			namespace {{c.ContainingNamespace}};
			public unsafe partial class {{c.Name}} : {{c.BaseType.Name}} {
			""";

			var notificationName = "";
			foreach (var method in methods)
			{
				var notificationAttribute = method.GetAttributes().SingleOrDefault(x => x.AttributeClass.ToString() == "GDExtension.NotificationAttribute");
				if (notificationAttribute != null)
				{
					notificationName = method.Name;
				}
			}
			if (notificationName == "")
			{
				notificationName = "_Notification";
				var has = false;
				code += $$"""
					void {{notificationName}}(int what) {
						switch (what) {
				""";
				foreach (var method in methods)
				{
					var att = method.GetAttributes().SingleOrDefault(x => x.AttributeClass.ToString() == "GDExtension.NotifyAttribute");


					if (att != null)
					{
						has = true;
						var args = att.NamedArguments.SingleOrDefault(x => x.Key == "arguments").Value.Value ?? "";

						code += $$"""
								case {{att.ConstructorArguments[0].Value}}:
									{{method.Name}}({{args}});
									break;
						""";
					}
				}
				if (has == false)
				{
					return false;
				}
				code += $$"""
						}
					}
				""";
			}
			code += $$"""
				public static unsafe new void __Notification(void* instance, int what) {
					var inst = ({{c.Name}})instance;
					{{c.BaseType.Name}}.__Notification(instance, what);
					inst.{{notificationName}}(what);
				}
			}
			""";

			context.AddSource($"{c.Name}.notification.gen.cs", code);
			return true;
		}
	}
}
