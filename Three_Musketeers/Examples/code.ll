; ModuleID = 'Three_Musketeers'
target triple = "x86_64-pc-linux-gnu"

@.str.0 = private unnamed_addr constant [21 x i8] c"Printf esta nota %d\0A\00"

declare i32 @printf(i8*, ...)

define i32 @main() {
entry:
  %x = alloca i32
  store i32 100, i32* %x
  %1 = getelementptr [21 x i8], [21 x i8]* @.str.0, i32 0, i32 0
  %2 = load i32, i32* %x
  %3 = call i32 (i8*, ...) @printf(i8* %1, i32 %2)
  ret i32 0
}
