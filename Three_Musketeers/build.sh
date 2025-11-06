#!/bin/bash

set -e

# Generate LLVM IR (.ll)
dotnet build --nologo -v q || exit 1
dotnet run --no-build -- Examples/code.3m -o code || exit 1

# Execute
./Examples/bin/code arg1 arg2 arg3

EXIT_CODE=$?
echo "===================================="

if [ $EXIT_CODE -ne 0 ]; then
    echo ""
    echo "Program exited with code: $EXIT_CODE"
fi