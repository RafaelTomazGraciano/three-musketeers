# 1. LLVM IR (.ll) → Bitcode (.bc)
llvm-as code.ll -o code.bc

# 2. Otimizar (opcional)
opt -O2 code.bc -o code-opt.bc

# 3. Bitcode (.bc) → Assembly (.s)
llc code-opt.bc -o code.s

# 4. Assembly (.s) → Executável
gcc code.s -o code -no-pie

# 5. Executar!
./code