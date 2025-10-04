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
@.str.11 = private unnamed_addr constant [28 x i8] c"Character %c! Digite outro:\00"
@.str.12 = private unnamed_addr constant [3 x i8] c"%c\00", align 1

declare i32 @printf(i8*, ...)
declare i32 @scanf(i8*, ...)
declare i8* @strcpy(i8*, i8*)
declare i32 @puts(i8*)
@.str.puts.5b9b686b = private unnamed_addr constant [37 x i8] c"Please enter the details of the book\00"
declare i8* @fgets(i8*, i32, %struct._IO_FILE*)
@stdin = external global %struct._IO_FILE*
%struct._IO_FILE = type { i32, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, %struct._IO_marker*, %struct._IO_FILE*, i32, i32, i64, i16, i8, [1 x i8], i8*, i64, i8*, i8*, i8*, i8*, i64, i32, [20 x i8] }
%struct._IO_marker = type { %struct._IO_marker*, %struct._IO_FILE*, i32 }
@.str.puts.73df7584 = private unnamed_addr constant [21 x i8] c"   --- Book Data ---\00"
define i32 @main() {
entry:
  %1 = alloca i8*, align 1
  %2 = getelementptr inbounds [256 x i8], [256 x i8]* %1, i32 0, i32 0
  %3 = bitcast i8* getelementptr inbounds ([256 x i8], [256 x i8]* @.str.0, i32 0, i32 0) to i8*
  %4 = call i8* @strcpy(i8* %2, i8* %3)
  %5 = alloca i8*, align 1
  %6 = getelementptr inbounds [256 x i8], [256 x i8]* %5, i32 0, i32 0
  %7 = bitcast i8* getelementptr inbounds ([256 x i8], [256 x i8]* @.str.1, i32 0, i32 0) to i8*
  %8 = call i8* @strcpy(i8* %6, i8* %7)
  %9 = alloca i32, align 4
  %10 = alloca double, align 8
  %11 = alloca [2 x i32], align 4
  %12 = getelementptr inbounds [2 x i32], [2 x i32]* %11, i32 0, i32 1
  store i32 0, i32* %12, align 4
  %13 = alloca i1, align 1
  store i1 0, i1* %13, align 1
  %14 = getelementptr inbounds [256 x i8], [256 x i8]* %1, i32 0, i32 0
  %15 = call i32 @puts(i8* %14)
  %16 = getelementptr inbounds [37 x i8], [37 x i8]* @.str.puts.5b9b686b, i32 0, i32 0
  %17 = call i32 @puts(i8* %16)
  %18 = getelementptr [28 x i8], [28 x i8]* @.str.2, i32 0, i32 0
  %19 = call i32 (i8*, ...) @printf(i8* %18)
  %20 = getelementptr [29 x i8], [29 x i8]* @.str.3, i32 0, i32 0
  %21 = call i32 (i8*, ...) @printf(i8* %20)
  %22 = getelementptr inbounds [256 x i8], [256 x i8]* %5, i32 0, i32 0
  %23 = load %struct._IO_FILE*, %struct._IO_FILE** @stdin
  %24 = call i8* @fgets(i8* %22, i32 256, %struct._IO_FILE* %23)
  %25 = getelementptr [30 x i8], [30 x i8]* @.str.4, i32 0, i32 0
  %26 = call i32 (i8*, ...) @printf(i8* %25)
  %27 = getelementptr inbounds [3 x i8], [3 x i8]* @.str.5, i32 0, i32 0
  %28 = call i32 (i8*, ...) @scanf(i8* %27, i32* %9)
  %29 = getelementptr [30 x i8], [30 x i8]* @.str.6, i32 0, i32 0
  %30 = call i32 (i8*, ...) @printf(i8* %29)
  %31 = getelementptr inbounds [4 x i8], [4 x i8]* @.str.7, i32 0, i32 0
  %32 = call i32 (i8*, ...) @scanf(i8* %31, double* %10)
  %33 = getelementptr inbounds [21 x i8], [21 x i8]* @.str.puts.73df7584, i32 0, i32 0
  %34 = call i32 @puts(i8* %33)
  %35 = getelementptr [39 x i8], [39 x i8]* @.str.8, i32 0, i32 0
  %36 = getelementptr inbounds [256 x i8], [256 x i8]* %5, i32 0, i32 0
  %37 = load i32, i32* %9, align 4
  %38 = load double, double* %10, align 8
  %39 = call i32 (i8*, ...) @printf(i8* %35, i8* %36, i32 %37, double %38)
  %40 = getelementptr [13 x i8], [13 x i8]* @.str.9, i32 0, i32 0
  %41 = getelementptr inbounds [2 x i32], [2 x i32]* %11, i32 0, i32 1
  %42 = load i32, i32* %41, align 4
  %43 = call i32 (i8*, ...) @printf(i8* %40, i32 %42)
  store double 5.0, double* %10, align 8
  %44 = getelementptr [17 x i8], [17 x i8]* @.str.10, i32 0, i32 0
  %45 = load double, double* %10, align 8
  %46 = call i32 (i8*, ...) @printf(i8* %44, double %45)
  %47 = alloca i8, align 1
  store i8 97, i8* %47, align 1
  %48 = getelementptr [28 x i8], [28 x i8]* @.str.11, i32 0, i32 0
  %49 = load i8, i8* %47, align 1
  %50 = call i32 (i8*, ...) @printf(i8* %48, i8 %49)
  %51 = getelementptr inbounds [3 x i8], [3 x i8]* @.str.12, i32 0, i32 0
  %52 = call i32 (i8*, ...) @scanf(i8* %51, i8* %47)
  ret i32 0
}
