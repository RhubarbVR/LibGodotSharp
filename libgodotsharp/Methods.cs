using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators
{

	public class Methods
	{

		public struct Info
		{
			public string name;
			public (ITypeSymbol, string)[] arguments;
			public ITypeSymbol ret;
			public string property;
		}

		List<Info> methods;

		public Methods()
		{
			methods = new List<Info>();
		}

		public void AddMethod(Info info)
		{
			methods.Add(info);
		}

		void AddAttributeMethods(GeneratorExecutionContext context, INamedTypeSymbol c)
		{
			var ms = c.GetMembers().
				Where(x => x is IMethodSymbol).
				Select(x => (IMethodSymbol)x).
				Where(x => x.GetAttributes().Where(x => x.AttributeClass.ToString() == "GDExtension.MethodAttribute").Count() > 0)
				.ToArray();

			foreach (var method in ms)
			{
				var info = new Info()
				{
					name = method.Name,
					arguments = method.Parameters.Select(x => (x.Type, x.Name)).ToArray(),
					ret = method.ReturnsVoid ? null : method.ReturnType,
					property = null,
				};
				AddMethod(info);
			}
		}

		public void Generate(GeneratorExecutionContext context, INamedTypeSymbol c)
		{
			AddAttributeMethods(context, c);

			var code = $$"""
			using System.Runtime.CompilerServices;
			using System.Runtime.InteropServices;
			using GDExtension;
			using static GDExtension.Native;
			namespace {{c.ContainingNamespace}} {
			public unsafe partial class {{c.Name}} : {{c.BaseType.Name}} {
				static unsafe void RegisterMethods() {
			""";

			for (var i = 0; i < methods.Count; i++)
			{
				code += "\t\t{\n";
				var method = methods[i];
				if (method.arguments.Length > 0)
				{
					code += $"\t\t\tvar args = stackalloc GDExtensionPropertyInfo[{method.arguments.Length}];\n";
					code += $"\t\t\tvar args_meta = stackalloc GDExtensionClassMethodArgumentMetadata[{method.arguments.Length}];\n";
					for (var j = 0; j < method.arguments.Length; j++)
					{
						var arg = method.arguments[j];
						code += $"\t\t\targs[{j}] = {CreatePropertyInfo(arg.Item1, arg.Item2, 3)}";
						code += $"\t\t\targs_meta[{j}] = GDExtensionClassMethodArgumentMetadata.GDEXTENSION_METHOD_ARGUMENT_METADATA_NONE;\n";
					}
				}
				if (method.ret != null)
				{
					code += $"\t\t\tvar ret = {CreatePropertyInfo(method.ret, "return", 3)}";
				}

				code += $$"""
							var info = new GDExtensionClassMethodInfo() {
								name = new StringName("{{Renamer.ToSnake(method.name)}}")._internal_pointer,
								method_userdata = (void*)(new IntPtr({{i}})),
								call_func = SaftyRapper.GetFunctionPointerForDelegate(CallFunc),
								ptrcall_func = SaftyRapper.GetFunctionPointerForDelegate(CallFuncPtr),
								method_flags = (uint)GDExtensionClassMethodFlags.GDEXTENSION_METHOD_FLAGS_DEFAULT,
								has_return_value = System.Convert.ToByte({{(method.ret != null ? "true" : "false")}}),
								return_value_info = {{(method.ret != null ? "&ret" : "null")}},
								return_value_metadata = GDExtensionClassMethodArgumentMetadata.GDEXTENSION_METHOD_ARGUMENT_METADATA_NONE,
								argument_count = {{method.arguments.Length}},
								arguments_info = {{(method.arguments.Length > 0 ? "args" : "null")}},
								arguments_metadata = {{(method.arguments.Length > 0 ? "args_meta" : "null")}},
								default_argument_count = 0,
								default_arguments = null,
							};
							GDExtensionMain.extensionInterface.classdb_register_extension_class_method(GDExtensionMain.library, __godot_name._internal_pointer, info);
				""";
				code += "\t\t}\n";
			}

			code += $$"""
				}
				static void CallFuncPtr(void* method_userdata, void* p_instance, void** p_args, void* r_ret) {
					var instance = ({{c.Name}})p_instance;
					switch ((int)method_userdata) {
			""";
			for (var i = 0; i < methods.Count; i++)
			{
				var method = methods[i];
				var args = "";
				for (var j = 0; j < method.arguments.Length; j++)
				{
					var arg = method.arguments[j];
					if (arg.Item1.Name == "String")
					{
						args += $"StringMarshall.ToManaged(p_args[{j}])";
					}
					else if (TypeToVariantType(arg.Item1) == "Object")
					{
						args += $"({arg.Item1})GDExtension.Object.ConstructUnknown(p_args[{j}])"; //note: changed
					}
					else
					{
						args += $"*({arg.Item1}*)p_args[{j}]";
					}
					if (j < method.arguments.Length - 1)
					{
						args += ", ";
					}
				}
				code += $"\t\tcase {i}:\n\t\t\t";
				if (method.ret != null)
				{
					if (TypeToVariantType(method.ret) == "Object")
					{
						code += "throw new NotImplementedException();\n";
						continue;
					}
					else
					{
						code += $"*({method.ret}*)r_ret = ";
					}
				}
				if (method.property != null)
				{
					if (method.ret != null)
					{
						code += $"instance.{method.property};\n";
					}
					else
					{
						code += $"instance.{method.property} = {args};\n";
					}
				}
				else
				{
					code += $"instance.{method.name}({args});\n";
				}
				code += $"\t\t\tbreak;\n";
			}

			code += $$"""
					}
				}
				static void CallFunc(
					void* method_userdata,
					void* p_instance,
					void** p_args,
					long p_argument_count,
					void* r_return,
					GDExtensionCallError* r_error
				) {
					GDExtensionMain.extensionInterface.variant_new_nil(r_return); //no clue why this is needed
					var instance = ({{c.Name}})p_instance;
					switch ((int)method_userdata) {
			""";
			for (var i = 0; i < methods.Count; i++)
			{
				var method = methods[i];
				var args = "";
				for (var j = 0; j < method.arguments.Length; j++)
				{
					var arg = method.arguments[j];
					var t = TypeToVariantType(arg.Item1);
					args += $"({arg.Item1})Variant.Get{t}FromPointer(p_args[{j}])";
					if (j < method.arguments.Length - 1)
					{
						args += ", ";
					}
				}
				code += $"\t\tcase {i}: {{\n\t\t\t";
				if (method.ret != null)
				{
					code += "var res = ";
				}
				if (method.property != null)
				{
					if (method.ret != null)
					{
						code += $"instance.{method.property};\n";
					}
					else
					{
						code += $"instance.{method.property} = {args};\n";
					}
				}
				else
				{
					code += $"instance.{method.name}({args});\n";
				}
				if (method.ret != null)
				{
					var t = TypeToVariantType(method.ret);
					code += $"\t\t\tVariant.SaveIntoPointer(res, r_return);\n";
				}
				code += $"\t\t\tbreak;\n";
				code += "\t\t\t}\n";
			}

			code += $$"""
					}
				}
			} }
			""";

			context.AddSource($"{c.Name}.methods.gen.cs", code);
		}

		public static string TypeToVariantType(ITypeSymbol type)
		{
			return TypeToVariantType(type, LibGodotGenerators.GetSpecialBase(type));
		}

		public static string TypeToVariantType(ITypeSymbol type, SpecialBase sBase)
		{
			return sBase switch
			{
				SpecialBase.Node => "Object",
				SpecialBase.Resource => "Object",
				_ => type.Name switch
				{
					"Boolean" => "Bool",
					"Int64" => "Int",
					"Double" => "Float",
					"String" => "String",
					"Vector2" => "Vector2",
					"Vector2i" => "Vector2i",
					"Rect2" => "Rect2",
					"Rect2i" => "Rect2i",
					"Vector3" => "Vector3",
					"Vector3i" => "Vector3i",
					"Transform2D" => "Transform2D",
					"Vector4" => "Vector4",
					"Vector4i" => "Vector4i",
					"Plane" => "Plane",
					"Quaternion" => "Quaternion",
					"AABB" => "AABB",
					"Basis" => "Basis",
					"Transform3D" => "Transform3D",
					"Projection" => "Projection",
					"Color" => "Color",
					"StringName" => "StringName",
					"NodePath" => "NodePath",
					"RID" => "RID",
					"Callable" => "Callable",
					"Signal" => "Signal",
					"Dictionary" => "Dictionary",
					"Array" => "Array",
					"PackedByteArray" => "PackedByteArray",
					"PackedInt32Array" => "PackedInt32Array",
					"PackedInt64Array" => "PackedInt64Array",
					"PackedFloat32Array" => "PackedFloat32Array",
					"PackedFloat64Array" => "PackedFloat64Array",
					"PackedStringArray" => "PackedStringArray",
					"PackedVector2Array" => "PackedVector2Array",
					"PackedVector3Array" => "PackedVector3Array",
					"PackedColorArray" => "PackedColorArray",
					_ => throw new Exception($"Unknown type: {type.Name}"),
				},
			};
		}

		public static string TypeToHintString(ITypeSymbol type, SpecialBase sBase)
		{
			return sBase switch
			{
				SpecialBase.Node or SpecialBase.Resource => $"StringMarshall.ToNative(\"{type.Name}\")",
				SpecialBase.None => "StringMarshall.ToNative(\"\")",
				_ => throw new Exception(),
			};
		}

		public static string TypeToHint(ITypeSymbol type, SpecialBase sBase)
		{
			return sBase switch
			{
				SpecialBase.Node => "PropertyHint.NodeType",
				SpecialBase.Resource => "PropertyHint.ResourceType",
				SpecialBase.None => "PropertyHint.None",
				_ => throw new Exception(),
			};
		}

		public static string CreatePropertyInfo(ITypeSymbol type, string name, int tabs)
		{
			var sBase = LibGodotGenerators.GetSpecialBase(type);

			var t = new String('\t', tabs);

			return $$"""
			new GDExtensionPropertyInfo() {
			{{t}}	type = (GDExtensionVariantType)Variant.Type.{{TypeToVariantType(type, sBase)}},
			{{t}}	name = new StringName("{{Renamer.ToSnake(name)}}")._internal_pointer,
			{{t}}	class_name = __godot_name._internal_pointer,
			{{t}}	hint = (uint){{TypeToHint(type, sBase)}},
			{{t}}	hint_string = {{TypeToHintString(type, sBase)}},
			{{t}}	usage = (uint)PropertyUsageFlags.Default,
			{{t}}};
			""";
		}
	}
}
