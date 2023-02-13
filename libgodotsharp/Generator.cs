using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators
{
	[Generator]
	public class Generators : ISourceGenerator
	{

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				var rec = (SyntaxReciever)context.SyntaxReceiver!;
				var classes = new List<Register.Data>();
				foreach (var cSyntax in rec.names)
				{
					var c = (INamedTypeSymbol)context.Compilation.GetSemanticModel(cSyntax.SyntaxTree).GetDeclaredSymbol(cSyntax);
					var methods = new Methods();
					var sBase = GetSpecialBase(c);
					var notification = Notification.Generate(context, c);
					Export.Generate(context, c, methods);
					Signal.Generate(context, c);
					methods.Generate(context, c);
					var data = Register.Generate(context, c, notification, sBase);
					classes.Add(data);
				}
				Entry.Execute(context, classes);
			}
			catch (System.Exception e)
			{
				context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
					"godot",
					"godotError",
					e.Message,
					"location",
					DiagnosticSeverity.Error,
					true
				), null));
			}
		}

		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new SyntaxReciever());
		}

		public static SpecialBase GetSpecialBase(ITypeSymbol type)
		{
			return type.Name switch
			{
				"Node" => SpecialBase.Node,
				"Resource" => SpecialBase.Resource,
				"RefCounted" => SpecialBase.RefCounted,
				_ => type.BaseType switch
				{
					null => SpecialBase.None,
					_ => GetSpecialBase(type.BaseType),
				},
			};
		}
	}

	public enum SpecialBase
	{
		None,
		Node,
		Resource,
		RefCounted,
	}

	class SyntaxReciever : ISyntaxReceiver
	{

		public HashSet<TypeDeclarationSyntax> names;

		public SyntaxReciever()
		{
			names = new();
		}

		public void OnVisitSyntaxNode(SyntaxNode node)
		{
			if (node is ClassDeclarationSyntax c)
			{
				var att = GetAttribute(c, "Register");
				if (att != null)
				{
					names.Add(c);
				}
			}
		}

		AttributeSyntax GetAttribute(ClassDeclarationSyntax c, string name)
		{
			return c.AttributeLists.SelectMany(x => x.Attributes).SingleOrDefault(x => x.Name.ToString() == name);
		}
	}
}
