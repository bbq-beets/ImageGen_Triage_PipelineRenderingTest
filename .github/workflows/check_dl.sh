#!/usr/bin/env bash
set -euo pipefail

cat > check_dl.c << 'EOF'
#include <dlfcn.h>
#include <stdio.h>

int main() {
    const char* lib = "libglfw.so.3";
    void* handle = dlopen(lib, RTLD_NOW);
    if (!handle) {
        printf("dlopen FAILED: %s\n", dlerror());
        return 1;
    }
    printf("dlopen SUCCESS\n");
    return 0;
}
EOF

gcc check_dl.c -ldl -o check_dl

echo "=== Test 1: no LD_LIBRARY_PATH ==="
./check_dl || true

echo "=== Test 2: add NuGet GLFW path ==="
export LD_LIBRARY_PATH="$HOME/.nuget/packages/ultz.native.glfw/3.4.0/runtimes/linux-x64/native"
./check_dl || true
