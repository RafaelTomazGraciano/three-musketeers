; ModuleID = 'Three_Musketeers'
target triple = "x86_64-pc-linux-gnu"

@.str.0 = private unnamed_addr constant [32 x i8] c"Welcome to the Three Musketeers\00", align 1
@.str.1 = private unnamed_addr constant [1 x i8] c"\00", align 1
@.str.2 = private unnamed_addr constant [28 x i8] c"   --- Enter Book Data ---\0A\00"
@.str.3 = private unnamed_addr constant [29 x i8] c"Enter the name of the book: \00"
@.str.4 = private unnamed_addr constant [30 x i8] c"Enter the quantity of books: \00"
@.str.5 = private unnamed_addr constant [3 x i8] c"%d\00", align 1
@.str.6 = private unnamed_addr constant [30 x i8] c"Enter the value of the book: \00"
@.str.7 = private unnamed_addr constant [4 x i8] c"%lf\00", align 1
@.str.8 = private unnamed_addr constant [39 x i8] c"Name: %s | Quantity: %d | Value: %.2f\0A\00"

declare i32 @printf(i8*, ...)
declare i32 @scanf(i8*, ...)
declare i8* @strcpy(i8*, i8*)
declare i32 @puts(i8*)
@.str.puts.dc8be8f7 = private unnamed_addr constant [37 x i8] c"Please enter the details of the book\00"
declare i8* @fgets(i8*, i32, %struct._IO_FILE*)
@stdin = external global %struct._IO_FILE*
%struct._IO_FILE = type { i32, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, %struct._IO_marker*, %struct._IO_FILE*, i32, i32, i64, i16, i8, [1 x i8], i8*, i64, i8*, i8*, i8*, i8*, i64, i32, [20 x i8] }
%struct._IO_marker = type { %struct._IO_marker*, %struct._IO_FILE*, i32 }
@.str.puts.9639a40c = private unnamed_addr constant [21 x i8] c"   --- Book Data ---\00"
define i32 @main() {
entry:
  %1 = alloca [256 x i8], align 1
  %2 = getelementptr inbounds [256 x i8], [256 x i8]* %1, i32 0, i32 0
  %3 = bitcast i8* getelementptr inbounds ([256 x i8], [256 x i8]* @.str.0, i32 0, i32 0) to i8*
  %4 = call i8* @strcpy(i8* %2, i8* %3)
  %5 = alloca [256 x i8], align 1
  %6 = getelementptr inbounds [256 x i8], [256 x i8]* %5, i32 0, i32 0
  %7 = bitcast i8* getelementptr inbounds ([256 x i8], [256 x i8]* @.str.1, i32 0, i32 0) to i8*
  %8 = call i8* @strcpy(i8* %6, i8* %7)
  %9 = alloca i32, align 4
  store i32 0, i32* %9, align 4
  %10 = alloca double, align 4
  store double 0.0, double* %10, align 4
  %11 = getelementptr inbounds [256 x i8], [256 x i8]* %1, i32 0, i32 0
  %12 = call i32 @puts(i8* %11)
  %13 = getelementptr inbounds [37 x i8], [37 x i8]* @.str.puts.dc8be8f7, i32 0, i32 0
  %14 = call i32 @puts(i8* %13)
  %15 = getelementptr [28 x i8], [28 x i8]* @.str.2, i32 0, i32 0
  %16 = call i32 (i8*, ...) @printf(i8* %15)
  %17 = getelementptr [29 x i8], [29 x i8]* @.str.3, i32 0, i32 0
  %18 = call i32 (i8*, ...) @printf(i8* %17)
  %19 = getelementptr inbounds [256 x i8], [256 x i8]* %5, i32 0, i32 0
  %20 = load %struct._IO_FILE*, %struct._IO_FILE** @stdin
  %21 = call i8* @fgets(i8* %19, i32 256, %struct._IO_FILE* %20)
  %22 = getelementptr [30 x i8], [30 x i8]* @.str.4, i32 0, i32 0
  %23 = call i32 (i8*, ...) @printf(i8* %22)
  %24 = getelementptr inbounds [3 x i8], [3 x i8]* @.str.5, i32 0, i32 0
  %25 = call i32 (i8*, ...) @scanf(i8* %24, i32* %9)
  %26 = getelementptr [30 x i8], [30 x i8]* @.str.6, i32 0, i32 0
  %27 = call i32 (i8*, ...) @printf(i8* %26)
  %28 = getelementptr inbounds [4 x i8], [4 x i8]* @.str.7, i32 0, i32 0
  %29 = call i32 (i8*, ...) @scanf(i8* %28, double* %10)
  %30 = getelementptr inbounds [21 x i8], [21 x i8]* @.str.puts.9639a40c, i32 0, i32 0
  %31 = call i32 @puts(i8* %30)
  %32 = getelementptr [39 x i8], [39 x i8]* @.str.8, i32 0, i32 0
  %33 = getelementptr inbounds [256 x i8], [256 x i8]* %5, i32 0, i32 0
  %34 = load i32, i32* %9, align 4
  %35 = load double, double* %10, align 4
  %36 = call i32 (i8*, ...) @printf(i8* %32, i8* %33, i32 %34, double %35)
  ret i32 0
}
