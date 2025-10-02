; ModuleID = 'Three_Musketeers'
target triple = "x86_64-pc-linux-gnu"

@.str.0 = private unnamed_addr constant [1 x i8] c"\00", align 1
@.str.1 = private unnamed_addr constant [29 x i8] c"Enter the name of the book: \00"
@.str.2 = private unnamed_addr constant [30 x i8] c"Enter the quantity of books: \00"
@.str.3 = private unnamed_addr constant [3 x i8] c"%d\00", align 1
@.str.4 = private unnamed_addr constant [30 x i8] c"Enter the value of the book: \00"
@.str.5 = private unnamed_addr constant [4 x i8] c"%lf\00", align 1
@.str.6 = private unnamed_addr constant [22 x i8] c"   --- Book Data ---\0A\00"
@.str.7 = private unnamed_addr constant [39 x i8] c"Name: %s | Quantity: %d | Value: %.2f\0A\00"

declare i32 @printf(i8*, ...)
declare i32 @scanf(i8*, ...)
declare i8* @strcpy(i8*, i8*)
declare i8* @fgets(i8*, i32, %struct._IO_FILE*)
@stdin = external global %struct._IO_FILE*
%struct._IO_FILE = type { i32, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, %struct._IO_marker*, %struct._IO_FILE*, i32, i32, i64, i16, i8, [1 x i8], i8*, i64, i8*, i8*, i8*, i8*, i64, i32, [20 x i8] }
%struct._IO_marker = type { %struct._IO_marker*, %struct._IO_FILE*, i32 }
define i32 @main() {
entry:
  %1 = alloca [256 x i8], align 1
  %2 = getelementptr inbounds [256 x i8], [256 x i8]* %1, i32 0, i32 0
  %3 = bitcast i8* getelementptr inbounds ([256 x i8], [256 x i8]* @.str.0, i32 0, i32 0) to i8*
  %4 = call i8* @strcpy(i8* %2, i8* %3)
  %5 = alloca i32, align 4
  store i32 0, i32* %5, align 4
  %6 = alloca double, align 4
  store double 0.0, double* %6, align 4
  %7 = getelementptr [29 x i8], [29 x i8]* @.str.1, i32 0, i32 0
  %8 = call i32 (i8*, ...) @printf(i8* %7)
  %9 = getelementptr inbounds [256 x i8], [256 x i8]* %1, i32 0, i32 0
  %10 = load %struct._IO_FILE*, %struct._IO_FILE** @stdin
  %11 = call i8* @fgets(i8* %9, i32 256, %struct._IO_FILE* %10)
  %12 = getelementptr [30 x i8], [30 x i8]* @.str.2, i32 0, i32 0
  %13 = call i32 (i8*, ...) @printf(i8* %12)
  %14 = getelementptr inbounds [3 x i8], [3 x i8]* @.str.3, i32 0, i32 0
  %15 = call i32 (i8*, ...) @scanf(i8* %14, i32* %5)
  %16 = getelementptr [30 x i8], [30 x i8]* @.str.4, i32 0, i32 0
  %17 = call i32 (i8*, ...) @printf(i8* %16)
  %18 = getelementptr inbounds [4 x i8], [4 x i8]* @.str.5, i32 0, i32 0
  %19 = call i32 (i8*, ...) @scanf(i8* %18, double* %6)
  %20 = getelementptr [22 x i8], [22 x i8]* @.str.6, i32 0, i32 0
  %21 = call i32 (i8*, ...) @printf(i8* %20)
  %22 = getelementptr [39 x i8], [39 x i8]* @.str.7, i32 0, i32 0
  %23 = getelementptr inbounds [256 x i8], [256 x i8]* %1, i32 0, i32 0
  %24 = load i32, i32* %5, align 4
  %25 = load double, double* %6, align 4
  %26 = call i32 (i8*, ...) @printf(i8* %22, i8* %23, i32 %24, double %25)
  ret i32 0
}
