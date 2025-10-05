	.file	"code.ll"
	.section	.rodata.cst8,"aM",@progbits,8
	.p2align	3, 0x0                          # -- Begin function main
.LCPI0_0:
	.quad	0x4033fd70a3d70a3d              # double 19.989999999999998
.LCPI0_1:
	.quad	0x400921f9f01b866e              # double 3.1415899999999999
	.text
	.globl	main
	.p2align	4
	.type	main,@function
main:                                   # @main
# %bb.0:                                # %entry
	pushq	%r14
	pushq	%rbx
	subq	$360, %rsp                      # imm = 0x168
	movl	$.L.str.0, %edi
	callq	puts@PLT
	movl	$.Lstr, %edi
	callq	puts@PLT
	movl	$.L.str.2, %edi
	xorl	%esi, %esi
	xorl	%eax, %eax
	callq	printf@PLT
	movl	$.L.str.3, %edi
	movl	$1, %esi
	xorl	%eax, %eax
	callq	printf@PLT
	movl	$.L.str.4, %edi
	movl	$65, %esi
	xorl	%eax, %eax
	callq	printf@PLT
	movl	$.L.str.5, %edi
	movl	$42, %esi
	xorl	%eax, %eax
	callq	printf@PLT
	movsd	.LCPI0_0(%rip), %xmm0           # xmm0 = [1.9989999999999998E+1,0.0E+0]
	movl	$.L.str.6, %edi
	movb	$1, %al
	callq	printf@PLT
	movl	$.Lstr.1, %edi
	callq	puts@PLT
	movl	$.L.str.8, %edi
	movl	$10, %esi
	xorl	%eax, %eax
	callq	printf@PLT
	movl	$.L.str.9, %edi
	movl	$20, %esi
	xorl	%eax, %eax
	callq	printf@PLT
	movl	$.L.str.10, %edi
	movl	$30, %esi
	xorl	%eax, %eax
	callq	printf@PLT
	movl	$.Lstr.2, %edi
	callq	puts@PLT
	movl	$.L.str.12, %edi
	xorl	%eax, %eax
	callq	printf@PLT
	movq	stdin@GOTPCREL(%rip), %rax
	movq	(%rax), %rdx
	leaq	104(%rsp), %rbx
	movq	%rbx, %rdi
	movl	$256, %esi                      # imm = 0x100
	callq	fgets@PLT
	movl	$.L.str.13, %edi
	movq	%rbx, %rsi
	xorl	%eax, %eax
	callq	printf@PLT
	movl	$.Lstr.3, %edi
	callq	puts@PLT
	movl	$.L.str.15, %edi
	xorl	%eax, %eax
	callq	printf@PLT
	leaq	4(%rsp), %rsi
	movl	$.L.fmt.d, %edi
	xorl	%eax, %eax
	callq	scanf@PLT
	movl	$.L.str.17, %edi
	xorl	%eax, %eax
	callq	printf@PLT
	leaq	16(%rsp), %rsi
	movl	$.L.fmt.lf, %edi
	xorl	%eax, %eax
	callq	scanf@PLT
	movl	4(%rsp), %esi
	movsd	16(%rsp), %xmm0                 # xmm0 = mem[0],zero
	movl	$.L.str.19, %edi
	movb	$1, %al
	callq	printf@PLT
	movl	$.Lstr.4, %edi
	callq	puts@PLT
	movl	$3355185, 28(%rsp)              # imm = 0x333231
	leaq	28(%rsp), %rdi
	callq	atoi@PLT
	movl	$.L.str.22, %edi
	movl	%eax, %esi
	xorl	%eax, %eax
	callq	printf@PLT
	movl	$908997940, 8(%rsp)             # imm = 0x362E3534
	movw	$55, 12(%rsp)
	leaq	8(%rsp), %rdi
	callq	atof@PLT
	movl	$.L.str.24, %edi
	movb	$1, %al
	callq	printf@PLT
	movl	$.Lstr.5, %edi
	callq	puts@PLT
	leaq	52(%rsp), %rbx
	movl	$.L.fmt.d, %esi
	movq	%rbx, %rdi
	movl	$999, %edx                      # imm = 0x3E7
	xorl	%eax, %eax
	callq	sprintf@PLT
	leaq	44(%rsp), %r14
	movq	%r14, %rdi
	movq	%rbx, %rsi
	callq	strcpy@PLT
	movl	$.L.str.26, %edi
	movq	%r14, %rsi
	xorl	%eax, %eax
	callq	printf@PLT
	leaq	72(%rsp), %rbx
	movsd	.LCPI0_1(%rip), %xmm0           # xmm0 = [3.1415899999999999E+0,0.0E+0]
	movl	$.L.fmt.lf, %esi
	movq	%rbx, %rdi
	movb	$1, %al
	callq	sprintf@PLT
	leaq	36(%rsp), %r14
	movq	%r14, %rdi
	movq	%rbx, %rsi
	callq	strcpy@PLT
	movl	$.L.str.27, %edi
	movq	%r14, %rsi
	xorl	%eax, %eax
	callq	printf@PLT
	movl	$.Lstr.6, %edi
	callq	puts@PLT
	movl	$.L.str.puts.53049198, %edi
	callq	puts@PLT
	xorl	%eax, %eax
	addq	$360, %rsp                      # imm = 0x168
	popq	%rbx
	popq	%r14
	retq
.Lfunc_end0:
	.size	main, .Lfunc_end0-main
                                        # -- End function
	.type	.L.str.0,@object                # @.str.0
	.section	.rodata.str1.1,"aMS",@progbits,1
.L.str.0:
	.asciz	"Welcome to the Three Musketeers Language!"
	.size	.L.str.0, 42

	.type	.L.str.2,@object                # @.str.2
.L.str.2:
	.asciz	"Bool value: %d\n"
	.size	.L.str.2, 16

	.type	.L.str.3,@object                # @.str.3
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.3:
	.asciz	"Bool value changed: %d\n"
	.size	.L.str.3, 24

	.type	.L.str.4,@object                # @.str.4
	.section	.rodata.str1.1,"aMS",@progbits,1
.L.str.4:
	.asciz	"Character: %c\n"
	.size	.L.str.4, 15

	.type	.L.str.5,@object                # @.str.5
.L.str.5:
	.asciz	"Integer: %d\n"
	.size	.L.str.5, 13

	.type	.L.str.6,@object                # @.str.6
.L.str.6:
	.asciz	"Double: %.2f\n"
	.size	.L.str.6, 14

	.type	.L.str.8,@object                # @.str.8
.L.str.8:
	.asciz	"Array[0]: %d\n"
	.size	.L.str.8, 14

	.type	.L.str.9,@object                # @.str.9
.L.str.9:
	.asciz	"Array[1]: %d\n"
	.size	.L.str.9, 14

	.type	.L.str.10,@object               # @.str.10
.L.str.10:
	.asciz	"Array[2]: %d\n"
	.size	.L.str.10, 14

	.type	.L.str.12,@object               # @.str.12
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.12:
	.asciz	"Enter your name: "
	.size	.L.str.12, 18

	.type	.L.str.13,@object               # @.str.13
	.section	.rodata.str1.1,"aMS",@progbits,1
.L.str.13:
	.asciz	"Hello, %s!\n"
	.size	.L.str.13, 12

	.type	.L.str.15,@object               # @.str.15
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.15:
	.asciz	"Enter your age: "
	.size	.L.str.15, 17

	.type	.L.str.17,@object               # @.str.17
	.p2align	4, 0x0
.L.str.17:
	.asciz	"Enter your weight: "
	.size	.L.str.17, 20

	.type	.L.str.19,@object               # @.str.19
	.p2align	4, 0x0
.L.str.19:
	.asciz	"You are %d years old and weigh %.1f kg\n"
	.size	.L.str.19, 40

	.type	.L.str.22,@object               # @.str.22
	.p2align	4, 0x0
.L.str.22:
	.asciz	"String '123' converted to int: %d\n"
	.size	.L.str.22, 35

	.type	.L.str.23,@object               # @.str.23
	.section	.rodata.str1.1,"aMS",@progbits,1
.L.str.23:
	.asciz	"45.67"
	.size	.L.str.23, 6

	.type	.L.str.24,@object               # @.str.24
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.24:
	.asciz	"String '45.67' converted to double: %.2f\n"
	.size	.L.str.24, 42

	.type	.L.str.26,@object               # @.str.26
	.p2align	4, 0x0
.L.str.26:
	.asciz	"Int 999 converted to string: %s\n"
	.size	.L.str.26, 33

	.type	.L.str.27,@object               # @.str.27
	.p2align	4, 0x0
.L.str.27:
	.asciz	"Double 3.14159 converted to string: %s\n"
	.size	.L.str.27, 40

	.type	.L.fmt.d,@object                # @.fmt.d
	.section	.rodata.str1.1,"aMS",@progbits,1
.L.fmt.d:
	.asciz	"%d"
	.size	.L.fmt.d, 3

	.type	.L.fmt.lf,@object               # @.fmt.lf
.L.fmt.lf:
	.asciz	"%lf"
	.size	.L.fmt.lf, 4

	.type	.L.str.puts.53049198,@object    # @.str.puts.53049198
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.puts.53049198:
	.asciz	"Three Musketeers compiler is working!"
	.size	.L.str.puts.53049198, 38

	.type	.Lstr,@object                   # @str
	.section	.rodata.str1.1,"aMS",@progbits,1
.Lstr:
	.asciz	"=== Testing Basic Types ==="
	.size	.Lstr, 28

	.type	.Lstr.1,@object                 # @str.1
.Lstr.1:
	.asciz	"\n=== Testing Arrays ==="
	.size	.Lstr.1, 24

	.type	.Lstr.2,@object                 # @str.2
.Lstr.2:
	.asciz	"\n=== Testing String Input ==="
	.size	.Lstr.2, 30

	.type	.Lstr.3,@object                 # @str.3
.Lstr.3:
	.asciz	"\n=== Testing Scanf ==="
	.size	.Lstr.3, 23

	.type	.Lstr.4,@object                 # @str.4
.Lstr.4:
	.asciz	"\n=== Testing String to Number Conversions ==="
	.size	.Lstr.4, 46

	.type	.Lstr.5,@object                 # @str.5
.Lstr.5:
	.asciz	"\n=== Testing Number to String Conversions ==="
	.size	.Lstr.5, 46

	.type	.Lstr.6,@object                 # @str.6
.Lstr.6:
	.asciz	"\n=== All Tests Completed ==="
	.size	.Lstr.6, 29

	.section	".note.GNU-stack","",@progbits
