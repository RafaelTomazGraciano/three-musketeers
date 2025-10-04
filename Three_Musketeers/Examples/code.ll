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
@.str.9 = private unnamed_addr constant [13 x i8] c"temp[1]: %d\0A\00"
@.str.10 = private unnamed_addr constant [17 x i8] c"New value: %.2f\0A\00"
@.str.11 = private unnamed_addr constant [15 x i8] c"Character %c!\0A\00"

declare i32 @printf(i8*, ...)
declare i32 @scanf(i8*, ...)
declare i8* @strcpy(i8*, i8*)
declare i32 @puts(i8*)
@.str.puts.bd829a2e = private unnamed_addr constant [37 x i8] c"Please enter the details of the book\00"
declare i8* @fgets(i8*, i32, %struct._IO_FILE*)
@stdin = external global %struct._IO_FILE*
%struct._IO_FILE = type { i32, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, %struct._IO_marker*, %struct._IO_FILE*, i32, i32, i64, i16, i8, [1 x i8], i8*, i64, i8*, i8*, i8*, i8*, i64, i32, [20 x i8] }
%struct._IO_marker = type { %struct._IO_marker*, %struct._IO_FILE*, i32 }
@.str.puts.618f3237 = private unnamed_addr constant [21 x i8] c"   --- Book Data ---\00"
define i32 @main() {
entry:
  %1 = alloca i8*, align 1
  %2 = alloca i8*, align 1
  %3 = alloca i32, align 4
  %4 = alloca double, align 8
  %5 = alloca [2 x i32], align 4
  %6 = getelementptr inbounds [2 x i32], [2 x i32]* %5, i32 0, i32 1
  store i32 0, i32* %6, align 4
  %7 = alloca i1, align 1
  store i1 0, i1* %7, align 1
  %8 = getelementptr inbounds [256 x i8], [256 x i8]* %1, i32 0, i32 0
  %9 = call i32 @puts(i8* %8)
  %10 = getelementptr inbounds [37 x i8], [37 x i8]* @.str.puts.bd829a2e, i32 0, i32 0
  %11 = call i32 @puts(i8* %10)
  %12 = getelementptr [28 x i8], [28 x i8]* @.str.2, i32 0, i32 0
  %13 = call i32 (i8*, ...) @printf(i8* %12)
  %14 = getelementptr [29 x i8], [29 x i8]* @.str.3, i32 0, i32 0
  %15 = call i32 (i8*, ...) @printf(i8* %14)
  %16 = getelementptr inbounds [256 x i8], [256 x i8]* %2, i32 0, i32 0
  %17 = load %struct._IO_FILE*, %struct._IO_FILE** @stdin
  %18 = call i8* @fgets(i8* %16, i32 256, %struct._IO_FILE* %17)
  %19 = getelementptr [30 x i8], [30 x i8]* @.str.4, i32 0, i32 0
  %20 = call i32 (i8*, ...) @printf(i8* %19)
  %21 = getelementptr inbounds [3 x i8], [3 x i8]* @.str.5, i32 0, i32 0
  %22 = call i32 (i8*, ...) @scanf(i8* %21, i32* %3)
  %23 = getelementptr [30 x i8], [30 x i8]* @.str.6, i32 0, i32 0
  %24 = call i32 (i8*, ...) @printf(i8* %23)
  %25 = getelementptr inbounds [4 x i8], [4 x i8]* @.str.7, i32 0, i32 0
  %26 = call i32 (i8*, ...) @scanf(i8* %25, double* %4)
  %27 = getelementptr inbounds [21 x i8], [21 x i8]* @.str.puts.618f3237, i32 0, i32 0
  %28 = call i32 @puts(i8* %27)
  %29 = getelementptr [39 x i8], [39 x i8]* @.str.8, i32 0, i32 0
  %30 = getelementptr inbounds [256 x i8], [256 x i8]* %2, i32 0, i32 0
  %31 = load i32, i32* %3, align 4
  %32 = load double, double* %4, align 8
  %33 = call i32 (i8*, ...) @printf(i8* %29, i8* %30, i32 %31, double %32)
  %34 = getelementptr [13 x i8], [13 x i8]* @.str.9, i32 0, i32 0
  %35 = getelementptr inbounds [2 x i32], [2 x i32]* %5, i32 0, i32 1
  %36 = load i32, i32* %35, align 4
  %37 = call i32 (i8*, ...) @printf(i8* %34, i32 %36)
  store double 5.0, double* %4, align 8
  %38 = getelementptr [17 x i8], [17 x i8]* @.str.10, i32 0, i32 0
  %39 = load double, double* %4, align 8
  %40 = call i32 (i8*, ...) @printf(i8* %38, double %39)
  %41 = alloca i8, align 1
  store i8 97, i8* %41, align 1
  %42 = getelementptr [15 x i8], [15 x i8]* @.str.11, i32 0, i32 0
  %43 = load i8, i8* %41, align 1
  %44 = call i32 (i8*, ...) @printf(i8* %42, i8 %43)
  ret i32 0
}
