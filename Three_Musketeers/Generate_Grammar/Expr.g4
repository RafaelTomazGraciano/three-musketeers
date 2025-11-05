grammar Expr;

start: include* define* prog* mainFunction prog* EOF;

include
    : INCLUDE ANGLE_STRING		# IncludeSystem
	| INCLUDE STRING_LITERAL	# IncludeUser;

define
    : DEFINE ID INT				# DefineInt
	| DEFINE ID DOUBLE			# DefineDouble
	| DEFINE ID STRING_LITERAL	# DefineString;

prog
    : stm
	| heteregeneousDeclaration
	| function
	| declaration EOL
	| att EOL;

mainFunction: 'int' 'main' '(' mainArgs? ')' '{' func_body '}';

mainArgs: 'int' ID ',' 'char' ID '[' ']';

stm
    : expr EOL
	| declaration EOL
	| att EOL
	| attVar EOL
	| printfStatement
	| scanfStatement
	| getsStatement
	| putsStatement
	| freeStatement
	| RETURN expr? EOL
	| ifStatement
	| switchStatement
	| forStatement
	| whileStatement
	| doWhileStatement
	| BREAK EOL
	| CONTINUE EOL;

heteregeneousDeclaration
    : structStatement 
    | unionStatement;

declaration: type POINTER* ID intIndex*;

structStatement: 'struct' ID '{' declaration (',' declaration)* '}' EOL;

unionStatement: 'union' ID '{' declaration (',' declaration)* '}' EOL;

function: function_return ID '(' args? ')' '{' func_body '}';

function_return: type POINTER* | VOID;

func_body: stm*;

printfStatement:'printf' '(' STRING_LITERAL (',' expr)* ')' EOL;

scanfStatement: 'scanf' '(' ID (',' ID)* ')' EOL;

getsStatement: 'gets' '(' ID ')' EOL;

putsStatement: 'puts' '(' (ID index? | STRING_LITERAL) ')' EOL;

att
    : (type POINTER*)? ID '=' expr					# GenericAtt
	| (type POINTER+)? ID '=' 'malloc' '(' expr ')'	# MallocAtt
	| derref '=' expr								# DerrefAtt
	| structGet '=' expr							# StructAtt;

attVar
    : ID index* '=' expr	# SingleArrayAtt
	| ID index* '+=' expr	# SingleAttPlusEquals
	| ID index* '-=' expr	# SingleAttMinusEquals
	| ID index* '*=' expr	# SingleAttMultiplyEquals
	| ID index* '/=' expr	# SingleAttDivideEquals;

args: type POINTER* ID (',' type POINTER* ID)*;

index: '[' expr ']';

intIndex: '[' INT ']';

freeStatement: 'free' '(' ID ')' EOL;

ifStatement:
	IF '(' expr ')' '{' func_body '}' (
		ELSE IF '(' expr ')' '{' func_body '}'
	)* (ELSE '{' func_body '}')?;

switchStatement:
	SWITCH '(' expr ')' '{' caseLabel* defaultLabel? '}';

caseLabel: CASE (INT | CHAR_LITERAL) ':' func_body;

defaultLabel: DEFAULT ':' func_body;

forStatement:
	FOR '(' forInit? EOL forCondition? EOL forIncrement? ')' '{' func_body '}';

forInit: declaration | att | attVar;

forCondition: expr;

forIncrement: expr | att | attVar;

whileStatement: WHILE '(' expr ')' '{' func_body '}';

doWhileStatement: DO '{' func_body '}' WHILE '(' expr ')' EOL;

expr:
	'++' ID									# PrefixIncrement
	| '--' ID								# PrefixDecrement
	| ID '++'								# PostfixIncrement
	| ID '--'								# PostfixDecrement
	| '++' ID index+						# PrefixIncrementArray
	| '--' ID index+						# PrefixDecrementArray
	| ID index+ '++'						# PostfixIncrementArray
	| ID index+ '--'						# PostfixDecrementArray
	| expr ('&&' | '||') expr				# LogicalAndOr
	| expr ('==' | '!=') expr				# Equality
	| expr ('>' | '<' | '>=' | '<=') expr	# Comparison
	| expr ('*' | '/' | '%') expr			# MulDivMod
	| expr ('+' | '-') expr					# AddSub
	| '(' expr ')'							# Parens
	| '!' expr								# LogicalNot
	| '-' expr								# UnaryMinus
	| 'atoi' '(' expr ')'					# AtoiConversion
	| 'atod' '(' expr ')'					# AtodConversion
	| 'itoa' '(' expr ')'					# ItoaConversion
	| 'dtoa' '(' expr ')'					# DtoaConversion
	| ID '(' (expr (',' expr)*)? ')'		# FunctionCall
	| ID									# Var
	| structGet								# VarStruct
	| ID index+								# VarArray
	| derref								# DerrefExpr
	| ADDRESS expr							# ExprAddress
	| INT									# IntLiteral
	| DOUBLE								# DoubleLiteral
	| STRING_LITERAL						# StringLiteral
	| CHAR_LITERAL							# CharLiteral
	| TRUE									# TrueLiteral
	| FALSE									# FalseLiteral;

structGet:
	ID index* '.' structContinue
	| ID index* '->' structContinue;

structContinue: ID index* | structGet;

derref: '(' POINTER expr ')';

type: 'int' | 'double' | 'bool' | 'char' | 'string' | ID;

/* -------- TOKENS -------- */
INCLUDE: '#include';
DEFINE: '#define';
RETURN: 'return';
IF: 'if';
ELSE: 'else';
SWITCH: 'switch';
CASE: 'case';
DEFAULT: 'default';
BREAK: 'break';
FOR: 'for';
WHILE: 'while';
DO: 'do';
CONTINUE: 'continue';
GR: '>';
GRT: '>=';
LE: '<';
LET: '<=';
AND: '&&';
OR: '||';
NOT: '!';
EQ: '==';
NE: '!=';
TRUE: 'true';
FALSE: 'false';
VOID: 'void';
ID: [a-zA-Z_][a-zA-Z0-9_]*;
INT: [0-9]+;
DOUBLE: [0-9]+ '.' [0-9]* | [0-9]* '.' [0-9]+;
EOL: ';';
WS: [ \t\r\n]+ -> skip;
LINE_COMMENT: '//' ~[\r\n]* -> skip;
ANGLE_STRING: '<' (~[>\r\n])+ '>';
BLOCK_COMMENT: '/*' .*? '*/' -> skip;
STRING_LITERAL: '"' (~["\\\r\n] | '\\' .)* '"';
CHAR_LITERAL: '\'' ( ~['\\] | '\\' [0trn'\\]) '\'';
POINTER: '*';
ADDRESS: '&';