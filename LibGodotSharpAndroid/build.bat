cd ./../../godot-lib/
rem scons platform=android library_type=shared_library debug_symbols=yes optimize=debug target=template_debug arch=x86_64
rem scons platform=android library_type=shared_library debug_symbols=yes optimize=debug target=template_debug arch=x86
scons platform=android library_type=shared_library debug_symbols=yes optimize=debug target=template_debug arch=armv7
scons platform=android library_type=shared_library debug_symbols=yes optimize=debug target=template_debug arch=arm64v8
cd ./platform/android/java
./gradlew assemble
