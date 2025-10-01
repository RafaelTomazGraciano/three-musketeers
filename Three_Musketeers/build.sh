# 1. Generate LLVM IR (.ll)
dotnet build
dotnet run

# 2. LLVM IR (.ll) → Bitcode (.bc)
llvm-as Examples/code.ll -o Examples/code.bc

# 3. Optimize
opt -O2 Examples/code.bc -o Examples/code-opt.bc

# 4. Bitcode (.bc) → Assembly (.s)
llc Examples/code-opt.bc -o Examples/code.s

# 5. Assembly (.s) → Executable
gcc Examples/code.s -o Examples/code -no-pie

# 6. Execute
./Examples/code