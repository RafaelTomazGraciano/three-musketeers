	.text
	.file	"code.ll"
	.globl	main                            # -- Begin function main
	.p2align	4, 0x90
	.type	main,@function
main:                                   # @main
# %bb.0:                                # %entry
	pushq	%rbx
	subq	$528, %rsp                      # imm = 0x210
	movabsq	$32495402141115755, %rax        # imm = 0x7372656574656B
	movq	%rax, 40(%rsp)
	movabsq	$8319640688172298856, %rax      # imm = 0x73754D2065657268
	movq	%rax, 32(%rsp)
	movabsq	$6061956597739974516, %rax      # imm = 0x5420656874206F74
	movq	%rax, 24(%rsp)
	movabsq	$2334392307038315863, %rax      # imm = 0x20656D6F636C6557
	movq	%rax, 16(%rsp)
	movb	$0, 272(%rsp)
	movl	$0, 4(%rsp)
	movq	$0, 8(%rsp)
	leaq	16(%rsp), %rdi
	callq	puts@PLT
	movl	$.L.str.puts.dc8be8f7, %edi
	callq	puts@PLT
	movl	$.Lstr, %edi
	callq	puts@PLT
	movl	$.L.str.3, %edi
	xorl	%eax, %eax
	callq	printf@PLT
	movq	stdin@GOTPCREL(%rip), %rax
	movq	(%rax), %rdx
	leaq	272(%rsp), %rbx
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
	movl	$.L.str.puts.9639a40c, %edi
	callq	puts@PLT
	movl	4(%rsp), %edx
	movsd	8(%rsp), %xmm0                  # xmm0 = mem[0],zero
	movl	$.L.str.8, %edi
	movq	%rbx, %rsi
	movb	$1, %al
	callq	printf@PLT
	xorl	%eax, %eax
	addq	$528, %rsp                      # imm = 0x210
	popq	%rbx
	retq
.Lfunc_end0:
	.size	main, .Lfunc_end0-main
                                        # -- End function
	.type	.L.str.0,@object                # @.str.0
	.section	.rodata.str1.1,"aMS",@progbits,1
.L.str.0:
	.asciz	"Welcome to the Three Musketeers"
	.size	.L.str.0, 32

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

	.type	.L.str.puts.dc8be8f7,@object    # @.str.puts.dc8be8f7
	.p2align	4, 0x0
.L.str.puts.dc8be8f7:
	.asciz	"Please enter the details of the book"
	.size	.L.str.puts.dc8be8f7, 37

	.type	.L.str.puts.9639a40c,@object    # @.str.puts.9639a40c
	.p2align	4, 0x0
.L.str.puts.9639a40c:
	.asciz	"   --- Book Data ---"
	.size	.L.str.puts.9639a40c, 21

	.type	.Lstr,@object                   # @str
	.section	.rodata.str1.1,"aMS",@progbits,1
.Lstr:
	.asciz	"   --- Enter Book Data ---"
	.size	.Lstr, 27

	.section	".note.GNU-stack","",@progbits
