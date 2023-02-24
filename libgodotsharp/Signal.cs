using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators
{

	public static class Signal
	{

		public static void Generate(GeneratorExecutionContext context, INamedTypeSymbol c)
		{
			var events = c.GetMembers().
				Where(x => x is INamedTypeSymbol).
				Select(x => (INamedTypeSymbol)x).
				Where(x => x.GetAttributes().Where(x => x.AttributeClass.ToString() == "GDExtension.GDExtensionMain.libraryAttribute").Count() > 0)
				.ToArray();

			var code = $$"""
			using System.Runtime.CompilerServices;
			using System.Runtime.InteropServices;
			using GDExtension;
			using static GDExtension.Native;
			namespace {{c.ContainingNamespace}} {
			public unsafe partial class {{c.Name}} : {{c.BaseType.Name}} {
				static unsafe void RegisterSignals() {
			""";
			for (var i = 0; i < events.Length; i++)
			{
				var ev = events[i];
				var m = ev.DelegateInvokeMethod;

				var infosName = "null";
				if (m.Parameters.Length > 0)
				{
					infosName = $"infos{ev.Name}";
					code += $"\t\tvar {infosName} = stackalloc GDExtensionPropertyInfo[{m.Parameters.Length}];\n";
					for (var j = 0; j < m.Parameters.Length; j++)
					{
						var p = m.Parameters[j];
						code += $"\t\tinfos{ev.Name}[{j}] = {Methods.CreatePropertyInfo(p.Type, p.Name, 2)}\n";
					}
				}
				code += $"\t\tGDExtensionMain.extensionInterface.classdb_register_extension_class_signal(GDExtensionInterface.gdLibrary, __godot_name._internal_pointer, new StringName(\"{Renamer.ToSnake(ev.Name)}\")._internal_pointer, {infosName}, {m.Parameters.Length});\n";
			}
			code += $$"""
				}
			""";

			for (var i = 0; i < events.Length; i++)
			{
				var ev = events[i];
				var m = ev.DelegateInvokeMethod;
				code += $"\tpublic void EmitSignal{ev.Name}(";
				for (var j = 0; j < m.Parameters.Length; j++)
				{
					var p = m.Parameters[j];
					code += $"{p.Type.Name} {p.Name}{(j < m.Parameters.Length - 1 ? ", " : "")}";
				}
				code += $") => EmitSignal(\"{Renamer.ToSnake(ev.Name)}\"";
				for (var j = 0; j < m.Parameters.Length; j++)
				{
					var p = m.Parameters[j];
					code += $", {p.Name}";
				}
				code += ");\n";
			}
			code += $$"""
			}
			}
			""";
			context.AddSource($"{c.Name}.Signal.gen.cs", code);
		}
	}
}
