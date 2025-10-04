	.file	"code.ll"
	.section	.rodata.cst8,"aM",@progbits,8
	.p2align	3, 0x0                          # -- Begin function main
.LCPI0_0:
	.quad	0x4014000000000000              # double 5
	.text
	.globl	main
	.p2align	4
	.type	main,@function
main:                                   # @main
# %bb.0:                                # %entry
	pushq	%rbx
	subq	$32, %rsp
	leaq	24(%rsp), %rdi
	callq	puts@PLT
	movl	$.L.str.puts.bd829a2e, %edi
	callq	puts@PLT
	movl	$.Lstr, %edi
	callq	puts@PLT
	movl	$.L.str.3, %edi
	xorl	%eax, %eax
	callq	printf@PLT
	movq	stdin@GOTPCREL(%rip), %rax
	movq	(%rax), %rdx
	leaq	16(%rsp), %rbx
	movq	%rbx, %rdi
	movl	$256, %esi                      # imm = 0x100
	callq	fgets@PLT
	movl	$.L.str.4, %edi
	xorl	%eax, %eax
	callq	printf@PLT
	leaq	4(%rsp), %rsi
	movl	$.L.str.5, %edi
	xorl	%eax, %eax
	callq	scanf@PLT
	movl	$.L.str.6, %edi
	xorl	%eax, %eax
	callq	printf@PLT
	leaq	8(%rsp), %rsi
	movl	$.L.str.7, %edi
	xorl	%eax, %eax
	callq	scanf@PLT
	movl	$.L.str.puts.618f3237, %edi
	callq	puts@PLT
	movl	4(%rsp), %edx
	movsd	8(%rsp), %xmm0                  # xmm0 = mem[0],zero
	movl	$.L.str.8, %edi
	movq	%rbx, %rsi
	movb	$1, %al
	callq	printf@PLT
	movl	$.L.str.9, %edi
	xorl	%esi, %esi
	xorl	%eax, %eax
	callq	printf@PLT
	movabsq	$4617315517961601024, %rax      # imm = 0x4014000000000000
	movq	%rax, 8(%rsp)
	movsd	.LCPI0_0(%rip), %xmm0           # xmm0 = [5.0E+0,0.0E+0]
	movl	$.L.str.10, %edi
	movb	$1, %al
	callq	printf@PLT
	movl	$.L.str.11, %edi
	movl	$97, %esi
	xorl	%eax, %eax
	callq	printf@PLT
	xorl	%eax, %eax
	addq	$32, %rsp
	popq	%rbx
	retq
.Lfunc_end0:
	.size	main, .Lfunc_end0-main
                                        # -- End function
	.type	.L.str.3,@object                # @.str.3
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.3:
	.asciz	"Enter the name of the book: "
	.size	.L.str.3, 29

	.type	.L.str.4,@object                # @.str.4
	.p2align	4, 0x0
.L.str.4:
	.asciz	"Enter the quantity of books: "
	.size	.L.str.4, 30

	.type	.L.str.5,@object                # @.str.5
	.section	.rodata.str1.1,"aMS",@progbits,1
.L.str.5:
	.asciz	"%d"
	.size	.L.str.5, 3

	.type	.L.str.6,@object                # @.str.6
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.6:
	.asciz	"Enter the value of the book: "
	.size	.L.str.6, 30

	.type	.L.str.7,@object                # @.str.7
	.section	.rodata.str1.1,"aMS",@progbits,1
.L.str.7:
	.asciz	"%lf"
	.size	.L.str.7, 4

	.type	.L.str.8,@object                # @.str.8
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.8:
	.asciz	"Name: %s | Quantity: %d | Value: %.2f\n"
	.size	.L.str.8, 39

	.type	.L.str.9,@object                # @.str.9
	.section	.rodata.str1.1,"aMS",@progbits,1
.L.str.9:
	.asciz	"temp[1]: %d\n"
	.size	.L.str.9, 13

	.type	.L.str.10,@object               # @.str.10
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.10:
	.asciz	"New value: %.2f\n"
	.size	.L.str.10, 17

	.type	.L.str.11,@object               # @.str.11
	.section	.rodata.str1.1,"aMS",@progbits,1
.L.str.11:
	.asciz	"Character %c!\n"
	.size	.L.str.11, 15

	.type	.L.str.puts.bd829a2e,@object    # @.str.puts.bd829a2e
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.puts.bd829a2e:
	.asciz	"Please enter the details of the book"
	.size	.L.str.puts.bd829a2e, 37

	.type	.L.str.puts.618f3237,@object    # @.str.puts.618f3237
	.p2align	4, 0x0
.L.str.puts.618f3237:
	.asciz	"   --- Book Data ---"
	.size	.L.str.puts.618f3237, 21

	.type	.Lstr,@object                   # @str
	.section	.rodata.str1.1,"aMS",@progbits,1
.Lstr:
	.asciz	"   --- Enter Book Data ---"
	.size	.Lstr, 27

	.section	".note.GNU-stack","",@progbits
