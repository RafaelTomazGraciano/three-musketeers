; ModuleID = 'Three_Musketeers'
target triple = "x86_64-pc-linux-gnu"

@.str.0 = private unnamed_addr constant [42 x i8] c"Welcome to the Three Musketeers Language!\00", align 1
@.str.1 = private unnamed_addr constant [29 x i8] c"=== Testing Basic Types ===\0A\00"
@.str.2 = private unnamed_addr constant [16 x i8] c"Bool value: %d\0A\00"
@.str.3 = private unnamed_addr constant [24 x i8] c"Bool value changed: %d\0A\00"
@.str.4 = private unnamed_addr constant [15 x i8] c"Character: %c\0A\00"
@.str.5 = private unnamed_addr constant [13 x i8] c"Integer: %d\0A\00"
@.str.6 = private unnamed_addr constant [14 x i8] c"Double: %.2f\0A\00"
@.str.7 = private unnamed_addr constant [25 x i8] c"\0A=== Testing Arrays ===\0A\00"
@.str.8 = private unnamed_addr constant [14 x i8] c"Array[0]: %d\0A\00"
@.str.9 = private unnamed_addr constant [14 x i8] c"Array[1]: %d\0A\00"
@.str.10 = private unnamed_addr constant [14 x i8] c"Array[2]: %d\0A\00"
@.str.11 = private unnamed_addr constant [31 x i8] c"\0A=== Testing String Input ===\0A\00"
@.str.12 = private unnamed_addr constant [18 x i8] c"Enter your name: \00"
@.str.13 = private unnamed_addr constant [12 x i8] c"Hello, %s!\0A\00"
@.str.14 = private unnamed_addr constant [24 x i8] c"\0A=== Testing Scanf ===\0A\00"
@.str.15 = private unnamed_addr constant [17 x i8] c"Enter your age: \00"
@.str.16 = private unnamed_addr constant [3 x i8] c"%d\00", align 1
@.str.17 = private unnamed_addr constant [20 x i8] c"Enter your weight: \00"
@.str.18 = private unnamed_addr constant [4 x i8] c"%lf\00", align 1
@.str.19 = private unnamed_addr constant [40 x i8] c"You are %d years old and weigh %.1f kg\0A\00"
@.str.20 = private unnamed_addr constant [47 x i8] c"\0A=== Testing String to Number Conversions ===\0A\00"
@.str.21 = private unnamed_addr constant [4 x i8] c"123\00", align 1
@.str.22 = private unnamed_addr constant [35 x i8] c"String '123' converted to int: %d\0A\00"
@.str.23 = private unnamed_addr constant [6 x i8] c"45.67\00", align 1
@.str.24 = private unnamed_addr constant [42 x i8] c"String '45.67' converted to double: %.2f\0A\00"
@.str.25 = private unnamed_addr constant [47 x i8] c"\0A=== Testing Number to String Conversions ===\0A\00"
@.str.26 = private unnamed_addr constant [33 x i8] c"Int 999 converted to string: %s\0A\00"
@.str.27 = private unnamed_addr constant [40 x i8] c"Double 3.14159 converted to string: %s\0A\00"
@.str.28 = private unnamed_addr constant [30 x i8] c"\0A=== All Tests Completed ===\0A\00"

declare i32 @printf(i8*, ...)
declare i32 @scanf(i8*, ...)
declare i8* @strcpy(i8*, i8*)
declare i32 @puts(i8*)
declare i8* @fgets(i8*, i32, %struct._IO_FILE*)
@stdin = external global %struct._IO_FILE*
%struct._IO_FILE = type { i32, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, %struct._IO_marker*, %struct._IO_FILE*, i32, i32, i64, i16, i8, [1 x i8], i8*, i64, i8*, i8*, i8*, i8*, i64, i32, [20 x i8] }
%struct._IO_marker = type { %struct._IO_marker*, %struct._IO_FILE*, i32 }
declare i32 @atoi(i8*)
declare double @atof(i8*)
declare i32 @sprintf(i8*, i8*, ...)
@.fmt.d = private unnamed_addr constant [3 x i8] c"%d\00", align 1
@.fmt.lf = private unnamed_addr constant [4 x i8] c"%lf\00", align 1
@.str.puts.f1afb6e2 = private unnamed_addr constant [38 x i8] c"Three Musketeers compiler is working!\00"
define i32 @main() {
entry:
  %1 = alloca i8*, align 1
  %2 = getelementptr inbounds [256 x i8], [256 x i8]* %1, i32 0, i32 0
  %3 = getelementptr inbounds [256 x i8], [256 x i8]* @.str.0, i32 0, i32 0
  %4 = call i8* @strcpy(i8* %2, i8* %3)
  %5 = getelementptr inbounds [256 x i8], [256 x i8]* %1, i32 0, i32 0
  %6 = call i32 @puts(i8* %5)
  %7 = getelementptr [29 x i8], [29 x i8]* @.str.1, i32 0, i32 0
  %8 = call i32 (i8*, ...) @printf(i8* %7)
  %9 = alloca i1, align 1
  store i1 0, i1* %9, align 1
  %10 = getelementptr [16 x i8], [16 x i8]* @.str.2, i32 0, i32 0
  %11 = load i1, i1* %9, align 1
  %12 = call i32 (i8*, ...) @printf(i8* %10, i1 %11)
  store i1 1, i1* %9, align 1
  %13 = getelementptr [24 x i8], [24 x i8]* @.str.3, i32 0, i32 0
  %14 = load i1, i1* %9, align 1
  %15 = call i32 (i8*, ...) @printf(i8* %13, i1 %14)
  %16 = alloca i8, align 1
  store i8 65, i8* %16, align 1
  %17 = getelementptr [15 x i8], [15 x i8]* @.str.4, i32 0, i32 0
  %18 = load i8, i8* %16, align 1
  %19 = call i32 (i8*, ...) @printf(i8* %17, i8 %18)
  %20 = alloca i32, align 4
  store i32 42, i32* %20, align 4
  %21 = getelementptr [13 x i8], [13 x i8]* @.str.5, i32 0, i32 0
  %22 = load i32, i32* %20, align 4
  %23 = call i32 (i8*, ...) @printf(i8* %21, i32 %22)
  %24 = alloca double, align 8
  store double 19.99, double* %24, align 8
  %25 = getelementptr [14 x i8], [14 x i8]* @.str.6, i32 0, i32 0
  %26 = load double, double* %24, align 8
  %27 = call i32 (i8*, ...) @printf(i8* %25, double %26)
  %28 = getelementptr [25 x i8], [25 x i8]* @.str.7, i32 0, i32 0
  %29 = call i32 (i8*, ...) @printf(i8* %28)
  %30 = alloca [3 x i32], align 4
  %31 = getelementptr inbounds [3 x i32], [3 x i32]* %30, i32 0, i32 0
  store i32 10, i32* %31, align 4
  %32 = getelementptr inbounds [3 x i32], [3 x i32]* %30, i32 0, i32 1
  store i32 20, i32* %32, align 4
  %33 = getelementptr inbounds [3 x i32], [3 x i32]* %30, i32 0, i32 2
  store i32 30, i32* %33, align 4
  %34 = getelementptr [14 x i8], [14 x i8]* @.str.8, i32 0, i32 0
  %35 = getelementptr inbounds [3 x i32], [3 x i32]* %30, i32 0, i32 0
  %36 = load i32, i32* %35, align 4
  %37 = call i32 (i8*, ...) @printf(i8* %34, i32 %36)
  %38 = getelementptr [14 x i8], [14 x i8]* @.str.9, i32 0, i32 0
  %39 = getelementptr inbounds [3 x i32], [3 x i32]* %30, i32 0, i32 1
  %40 = load i32, i32* %39, align 4
  %41 = call i32 (i8*, ...) @printf(i8* %38, i32 %40)
  %42 = getelementptr [14 x i8], [14 x i8]* @.str.10, i32 0, i32 0
  %43 = getelementptr inbounds [3 x i32], [3 x i32]* %30, i32 0, i32 2
  %44 = load i32, i32* %43, align 4
  %45 = call i32 (i8*, ...) @printf(i8* %42, i32 %44)
  %46 = getelementptr [31 x i8], [31 x i8]* @.str.11, i32 0, i32 0
  %47 = call i32 (i8*, ...) @printf(i8* %46)
  %48 = alloca [256 x i8], align 1
  %49 = getelementptr [18 x i8], [18 x i8]* @.str.12, i32 0, i32 0
  %50 = call i32 (i8*, ...) @printf(i8* %49)
  %51 = getelementptr inbounds [256 x i8], [256 x i8]* %48, i32 0, i32 0
  %52 = load %struct._IO_FILE*, %struct._IO_FILE** @stdin
  %53 = call i8* @fgets(i8* %51, i32 256, %struct._IO_FILE* %52)
  %54 = getelementptr [12 x i8], [12 x i8]* @.str.13, i32 0, i32 0
  %55 = getelementptr inbounds [256 x i8], [256 x i8]* %48, i32 0, i32 0
  %56 = call i32 (i8*, ...) @printf(i8* %54, i8* %55)
  %57 = getelementptr [24 x i8], [24 x i8]* @.str.14, i32 0, i32 0
  %58 = call i32 (i8*, ...) @printf(i8* %57)
  %59 = alloca i32, align 4
  %60 = alloca double, align 8
  %61 = getelementptr [17 x i8], [17 x i8]* @.str.15, i32 0, i32 0
  %62 = call i32 (i8*, ...) @printf(i8* %61)
  %63 = getelementptr inbounds [3 x i8], [3 x i8]* @.str.16, i32 0, i32 0
  %64 = call i32 (i8*, ...) @scanf(i8* %63, i32* %59)
  %65 = getelementptr [20 x i8], [20 x i8]* @.str.17, i32 0, i32 0
  %66 = call i32 (i8*, ...) @printf(i8* %65)
  %67 = getelementptr inbounds [4 x i8], [4 x i8]* @.str.18, i32 0, i32 0
  %68 = call i32 (i8*, ...) @scanf(i8* %67, double* %60)
  %69 = getelementptr [40 x i8], [40 x i8]* @.str.19, i32 0, i32 0
  %70 = load i32, i32* %59, align 4
  %71 = load double, double* %60, align 8
  %72 = call i32 (i8*, ...) @printf(i8* %69, i32 %70, double %71)
  %73 = getelementptr [47 x i8], [47 x i8]* @.str.20, i32 0, i32 0
  %74 = call i32 (i8*, ...) @printf(i8* %73)
  %75 = alloca i8*, align 1
  %76 = getelementptr inbounds [256 x i8], [256 x i8]* %75, i32 0, i32 0
  %77 = getelementptr inbounds [256 x i8], [256 x i8]* @.str.21, i32 0, i32 0
  %78 = call i8* @strcpy(i8* %76, i8* %77)
  %79 = alloca i32, align 4
  %80 = getelementptr inbounds [256 x i8], [256 x i8]* %75, i32 0, i32 0
  %81 = call i32 @atoi(i8* %80)
  store i32 %81, i32* %79, align 4
  %82 = getelementptr [35 x i8], [35 x i8]* @.str.22, i32 0, i32 0
  %83 = load i32, i32* %79, align 4
  %84 = call i32 (i8*, ...) @printf(i8* %82, i32 %83)
  %85 = alloca i8*, align 1
  %86 = getelementptr inbounds [256 x i8], [256 x i8]* %85, i32 0, i32 0
  %87 = getelementptr inbounds [256 x i8], [256 x i8]* @.str.23, i32 0, i32 0
  %88 = call i8* @strcpy(i8* %86, i8* %87)
  %89 = alloca double, align 8
  %90 = getelementptr inbounds [256 x i8], [256 x i8]* %85, i32 0, i32 0
  %91 = call double @atof(i8* %90)
  store double %91, double* %89, align 8
  %92 = getelementptr [42 x i8], [42 x i8]* @.str.24, i32 0, i32 0
  %93 = load double, double* %89, align 8
  %94 = call i32 (i8*, ...) @printf(i8* %92, double %93)
  %95 = getelementptr [47 x i8], [47 x i8]* @.str.25, i32 0, i32 0
  %96 = call i32 (i8*, ...) @printf(i8* %95)
  %97 = alloca i32, align 4
  store i32 999, i32* %97, align 4
  %98 = alloca i8*, align 1
  %99 = load i32, i32* %97, align 4
  %100 = alloca [20 x i8], align 1
  %101 = getelementptr inbounds [20 x i8], [20 x i8]* %100, i32 0, i32 0
  %102 = call i32 (i8*, i8*, ...) @sprintf(i8* %101, i8* getelementptr inbounds ([3 x i8], [3 x i8]* @.fmt.d, i32 0, i32 0), i32 %99)
  %103 = getelementptr inbounds [256 x i8], [256 x i8]* %98, i32 0, i32 0
  %104 = getelementptr inbounds [256 x i8], [256 x i8]* %101, i32 0, i32 0
  %105 = call i8* @strcpy(i8* %103, i8* %104)
  %106 = getelementptr [33 x i8], [33 x i8]* @.str.26, i32 0, i32 0
  %107 = getelementptr inbounds [256 x i8], [256 x i8]* %98, i32 0, i32 0
  %108 = call i32 (i8*, ...) @printf(i8* %106, i8* %107)
  %109 = alloca double, align 8
  store double 3.14159, double* %109, align 8
  %110 = alloca i8*, align 1
  %111 = load double, double* %109, align 8
  %112 = alloca [32 x i8], align 1
  %113 = getelementptr inbounds [32 x i8], [32 x i8]* %112, i32 0, i32 0
  %114 = call i32 (i8*, i8*, ...) @sprintf(i8* %113, i8* getelementptr inbounds ([4 x i8], [4 x i8]* @.fmt.lf, i32 0, i32 0), double %111)
  %115 = getelementptr inbounds [256 x i8], [256 x i8]* %110, i32 0, i32 0
  %116 = getelementptr inbounds [256 x i8], [256 x i8]* %113, i32 0, i32 0
  %117 = call i8* @strcpy(i8* %115, i8* %116)
  %118 = getelementptr [40 x i8], [40 x i8]* @.str.27, i32 0, i32 0
  %119 = getelementptr inbounds [256 x i8], [256 x i8]* %110, i32 0, i32 0
  %120 = call i32 (i8*, ...) @printf(i8* %118, i8* %119)
  %121 = getelementptr [30 x i8], [30 x i8]* @.str.28, i32 0, i32 0
  %122 = call i32 (i8*, ...) @printf(i8* %121)
  %123 = getelementptr inbounds [38 x i8], [38 x i8]* @.str.puts.f1afb6e2, i32 0, i32 0
  %124 = call i32 @puts(i8* %123)
  ret i32 0
}
