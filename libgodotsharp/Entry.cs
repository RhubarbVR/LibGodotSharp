using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Generators
{

    public static class Entry
    {

        public static void Execute(GeneratorExecutionContext context, List<Register.Data> classes)
        {
            var registrations = "";
            var editorRegistrations = "";
            var unregistrations = "";

            for (var i = 0; i < classes.Count; i++)
            {
                var n = classes[i];
                switch (n.level)
                {
                    case Register.Level.Scene:
                        registrations += $"{n.@namespace}.{n.name}.Register();\n\t\t\t";
                        break;
                    case Register.Level.Editor:
                        editorRegistrations += $"{n.@namespace}.{n.name}.Register();\n\t\t\t";
                        break;
                }
                unregistrations = $"GDExtensionMain.extensionInterface.classdb_unregister_extension_class(GDExtensionMain.library, {n.@namespace}.{n.name}.__godot_name._internal_pointer);\n\t\t\t" + unregistrations;
                classes[i] = classes.Last();
                classes.RemoveAt(classes.Count - 1);
            }
			var assimblyName = context.Compilation.AssemblyName ?? "NoName";
            var source = $$"""
			using System;
			using System.Runtime.CompilerServices;
			using System.Runtime.InteropServices;
			using GDExtension;
			using static GDExtension.Native;
			public static class {{assimblyName}}ExtensionEntry {
				public static unsafe bool EntryPoint(GDExtensionInterface @interface, void* library, GDExtensionInitialization* init) {
					GDExtensionMain.extensionInterface = @interface;
					GDExtensionMain.library = library;
					*init = new GDExtensionInitialization() {
						minimum_initialization_level = GDExtensionInitializationLevel.GDEXTENSION_INITIALIZATION_CORE,
						initialize = SaftyRapper.GetFunctionPointerForDelegate(Initialize),
						deinitialize = SaftyRapper.GetFunctionPointerForDelegate(Deinitialize),
					};
					return true;
				}
				public static unsafe void Initialize(void* userdata, GDExtensionInitializationLevel level) {
					switch (level) {
					case GDExtensionInitializationLevel.GDEXTENSION_INITIALIZATION_CORE:
						GDExtension.Register.RegisterBuiltin();
						GDExtension.Register.RegisterUtility();
						GDExtension.Register.RegisterCore();
						break;
					case GDExtensionInitializationLevel.GDEXTENSION_INITIALIZATION_SERVERS:
						GDExtension.Register.RegisterServers();
						break;
					case GDExtensionInitializationLevel.GDEXTENSION_INITIALIZATION_SCENE:
						GDExtension.Register.RegisterScene();
						{{registrations}}break;
					case GDExtensionInitializationLevel.GDEXTENSION_INITIALIZATION_EDITOR:
						GDExtension.Register.RegisterEditor();
						{{editorRegistrations}}break;
					}
				}
				public static unsafe void Deinitialize(void* userdata, GDExtensionInitializationLevel level) {
					switch (level) {
					case GDExtensionInitializationLevel.GDEXTENSION_INITIALIZATION_CORE:
						break;
					case GDExtensionInitializationLevel.GDEXTENSION_INITIALIZATION_SERVERS:
						break;
					case GDExtensionInitializationLevel.GDEXTENSION_INITIALIZATION_SCENE:
						{{unregistrations}}break;
					case GDExtensionInitializationLevel.GDEXTENSION_INITIALIZATION_EDITOR:
						break;
					}
				}
			}
			
			""";
            context.AddSource("ExtensionEntry.gen.cs", source);
        }
    }
}
