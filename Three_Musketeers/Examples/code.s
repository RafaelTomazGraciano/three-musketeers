	.text
	.file	"code.ll"
	.globl	main                            # -- Begin function main
	.p2align	4, 0x90
	.type	main,@function
main:                                   # @main
# %bb.0:                                # %entry
	pushq	%rbx
	subq	$272, %rsp                      # imm = 0x110
	movb	$0, 16(%rsp)
	movl	$0, 4(%rsp)
	movq	$0, 8(%rsp)
	movl	$.L.str.1, %edi
	xorl	%eax, %eax
	callq	printf@PLT
	movq	stdin@GOTPCREL(%rip), %rax
	movq	(%rax), %rdx
	leaq	16(%rsp), %rbx
	movq	%rbx, %rdi
	movl	$256, %esi                      # imm = 0x100
	callq	fgets@PLT
	movl	$.L.str.2, %edi
	xorl	%eax, %eax
	callq	printf@PLT
	leaq	4(%rsp), %rsi
	movl	$.L.str.3, %edi
	xorl	%eax, %eax
	callq	scanf@PLT
	movl	$.L.str.4, %edi
	xorl	%eax, %eax
	callq	printf@PLT
	leaq	8(%rsp), %rsi
	movl	$.L.str.5, %edi
	xorl	%eax, %eax
	callq	scanf@PLT
	movl	$.Lstr, %edi
	callq	puts@PLT
	movl	4(%rsp), %edx
	movsd	8(%rsp), %xmm0                  # xmm0 = mem[0],zero
	movl	$.L.str.7, %edi
	movq	%rbx, %rsi
	movb	$1, %al
	callq	printf@PLT
	xorl	%eax, %eax
	addq	$272, %rsp                      # imm = 0x110
	popq	%rbx
	retq
.Lfunc_end0:
	.size	main, .Lfunc_end0-main
                                        # -- End function
	.type	.L.str.1,@object                # @.str.1
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.1:
	.asciz	"Enter the name of the book: "
	.size	.L.str.1, 29

	.type	.L.str.2,@object                # @.str.2
	.p2align	4, 0x0
.L.str.2:
	.asciz	"Enter the quantity of books: "
	.size	.L.str.2, 30

	.type	.L.str.3,@object                # @.str.3
	.section	.rodata.str1.1,"aMS",@progbits,1
.L.str.3:
	.asciz	"%d"
	.size	.L.str.3, 3

	.type	.L.str.4,@object                # @.str.4
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.4:
	.asciz	"Enter the value of the book: "
	.size	.L.str.4, 30

	.type	.L.str.5,@object                # @.str.5
	.section	.rodata.str1.1,"aMS",@progbits,1
.L.str.5:
	.asciz	"%lf"
	.size	.L.str.5, 4

	.type	.L.str.7,@object                # @.str.7
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.7:
	.asciz	"Name: %s | Quantity: %d | Value: %.2f\n"
	.size	.L.str.7, 39

	.type	.Lstr,@object                   # @str
	.section	.rodata.str1.1,"aMS",@progbits,1
.Lstr:
	.asciz	"   --- Book Data ---"
	.size	.Lstr, 21

	.section	".note.GNU-stack","",@progbits
