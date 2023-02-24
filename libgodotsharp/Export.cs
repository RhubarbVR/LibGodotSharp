using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators
{

	public static class Export
	{

		record struct Data(string name, string setter, string getter, ITypeSymbol type);

		public static void Generate(GeneratorExecutionContext context, INamedTypeSymbol c, Methods methods)
		{
			var members = c.GetMembers().
				Where(x => x is IPropertySymbol || x is IFieldSymbol).
				Where(x => x.GetAttributes().Where(x => x.AttributeClass.ToString() == "GDExtension.ExportAttribute").Count() > 0).
				Select(x => x switch {
					IPropertySymbol prop => new Data(prop.Name, prop.SetMethod.Name, prop.GetMethod.Name, prop.Type),
					IFieldSymbol field => new Data(field.Name, "set_" + field.Name, "get_" + field.Name, field.Type),
					_ => throw new System.NotSupportedException(),
				}).
				ToArray();

			var code = $$"""
			using System.Runtime.CompilerServices;
			using System.Runtime.InteropServices;
			using GDExtension;
			using static GDExtension.Native;
			namespace {{c.ContainingNamespace}} {
			public unsafe partial class {{c.Name}} : {{c.BaseType.Name}} {
				static unsafe void RegisterExports() {
			""";

			for (var i = 0; i < members.Length; i++)
			{
				var member = members[i];

				code += $"\t\tvar __{member.name}Info = " + Methods.CreatePropertyInfo(member.type, member.name, 2);

				methods.AddMethod(new Methods.Info()
				{
					name = member.setter,
					arguments = new (ITypeSymbol, string)[] { (member.type, "value") },
					ret = null,
					property = member.name,
				});
				methods.AddMethod(new Methods.Info()
				{
					name = member.getter,
					arguments = new (ITypeSymbol, string)[] { },
					ret = member.type,
					property = member.name,
				});
				code += $$"""
						GDExtensionMain.extensionInterface.classdb_register_extension_class_property(
							GDExtensionMain.library,
							__godot_name._internal_pointer,
							&__{{member.name}}Info,
							new StringName("{{Renamer.ToSnake(member.setter)}}")._internal_pointer,
							new StringName("{{Renamer.ToSnake(member.getter)}}")._internal_pointer
						);
				""";
			}
			code += """
				}
			}}
			""";
			context.AddSource($"{c.Name}.export.gen.cs", code);
		}
	}
}
