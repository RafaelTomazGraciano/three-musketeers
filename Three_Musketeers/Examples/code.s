	.text
	.file	"code.ll"
	.section	.rodata.cst8,"aM",@progbits,8
	.p2align	3, 0x0                          # -- Begin function main
.LCPI0_0:
	.quad	0x40091eb851eb851f              # double 3.1400000000000001
	.text
	.globl	main
	.p2align	4, 0x90
	.type	main,@function
main:                                   # @main
# %bb.0:                                # %entry
	pushq	%rax
	movl	$.Lstr, %edi
	callq	puts@PLT
	movsd	.LCPI0_0(%rip), %xmm0           # xmm0 = [3.1400000000000001E+0,0.0E+0]
	movl	$.L.str.1, %edi
	movl	$42, %esi
	movb	$1, %al
	callq	printf@PLT
	xorl	%eax, %eax
	popq	%rcx
	retq
.Lfunc_end0:
	.size	main, .Lfunc_end0-main
                                        # -- End function
	.type	.L.str.1,@object                # @.str.1
	.section	.rodata.str1.16,"aMS",@progbits,1
	.p2align	4, 0x0
.L.str.1:
	.asciz	"\tInt: %d, Double: %.4f\n"
	.size	.L.str.1, 24

	.type	.Lstr,@object                   # @str
	.section	.rodata.str1.1,"aMS",@progbits,1
.Lstr:
	.asciz	"Hello, World!"
	.size	.Lstr, 14

	.section	".note.GNU-stack","",@progbits
