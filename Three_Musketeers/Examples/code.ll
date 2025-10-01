; ModuleID = 'Three_Musketeers'
target triple = "x86_64-pc-linux-gnu"

@.str.0 = private unnamed_addr constant [15 x i8] c"Hello, World!\0A\00"
@.str.1 = private unnamed_addr constant [24 x i8] c"\09Int: %d, Double: %.4f\0A\00"

declare i32 @printf(i8*, ...)

define i32 @main() {
entry:
  %x = alloca i32
  store i32 42, i32* %x
  %y = alloca double
  store double 3.14, double* %y
  %1 = getelementptr [15 x i8], [15 x i8]* @.str.0, i32 0, i32 0
  %2 = call i32 (i8*, ...) @printf(i8* %1)
  %3 = getelementptr [24 x i8], [24 x i8]* @.str.1, i32 0, i32 0
  %4 = load i32, i32* %x
  %5 = load double, double* %y
  %6 = call i32 (i8*, ...) @printf(i8* %3, i32 %4, double %5)
  ret i32 0
}
