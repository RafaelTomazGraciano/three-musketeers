grammar Expr;

start
    : prog* mainFunction EOF
    ;

prog
    : stm
    | newType
    | function
    ;

mainFunction
    : 'int' 'main' '(' mainArgs? ')' '{' func_body '}'
    ;

mainArgs
    : 'int' ID ',' 'char' ID '[' ']'
    ;

stm
    : expr EOL
    | declaration EOL
    | att  EOL
    | attVar EOL
    | printfStatement
    | scanfStatement
    | getsStatement
    | putsStatement
    | freeStatement
    | RETURN expr? EOL
    ;

declaration
    : type ID index*    #BaseDec
    | type POINTER+ ID  #PointerDec
    ;


function
    : function_return ID '(' args? ')' '{' func_body '}'
    ;

function_return
    : type
    ;

func_body
    : stm*
    ;

printfStatement
    : 'printf' '(' STRING_LITERAL (',' expr)* ')' EOL
    ;

scanfStatement
    : 'scanf' '(' ID (',' ID)* ')' EOL
    ;

getsStatement
    : 'gets' '(' ID ')' EOL
    ;

putsStatement
    : 'puts' '(' (ID index?| STRING_LITERAL) ')' EOL
    ;

att
    : type? POINTER* ID '=' expr                      #GenericAtt
    | (type POINTER+)? ID '=' 'malloc' '(' expr ')'   #MallocAtt
    | derref '=' expr                                 #DerrefAtt
    ;

attVar 
    : ID index+ '=' expr             #SingleAtt
    ;

newType
    : 'type' ID 'as' type EOL
    ;

args
    : type ID (',' type ID)*
    ;

index
    : '[' INT ']'
    ;

freeStatement
    : 'free''('ID')'EOL
    ;

expr
    : expr ('&&'|'||') expr          # LogicalAndOr
    | expr ('=='|'!=') expr          # Equality
    | expr ('>'|'<'|'>='|'<=') expr  # Comparison
    | expr ('*'|'/'|'%') expr        # MulDivMod
    | expr ('+'|'-') expr            # AddSub
    | '(' expr ')'                   # Parens
    | '!' expr                       # LogicalNot
    | '-' expr                       # UnaryMinus
    | 'atoi' '(' expr ')'            # AtoiConversion
    | 'atod' '(' expr ')'            # AtodConversion
    | 'itoa' '(' expr ')'            # ItoaConversion
    | 'dtoa' '(' expr ')'            # DtoaConversion
    | ID '(' (expr (',' expr)*)? ')' # FunctionCall
    | ID                             # Var
    | ID index+                      # VarArray
    | derref                         # DerrefExpr
    | ADDRESS expr                   # ExprAddress
    | INT                            # IntLiteral
    | DOUBLE                         # DoubleLiteral
    | STRING_LITERAL                 # StringLiteral
    | CHAR_LITERAL                   # CharLiteral
    | TRUE                           # TrueLiteral
    | FALSE                          # FalseLiteral
    ;

derref
    : POINTER expr
    ;

type
    : 'int'
    | 'double'
    | 'bool'
    | 'char'
    | 'string'
    | 'void'
    | ID
    ;

/* -------- TOKENS -------- */
RETURN        : 'return';
IF            : 'if';
ELSE          : 'else';
GR            : '>';
GRT           : '>=';
LE            : '<';
LET           : '<=';
AND           : '&&';
OR            : '||';
NOT           : '!';
EQ            : '==';
NE            : '!=';
TRUE          : 'true';
FALSE         : 'false';
ID            : [a-zA-Z_][a-zA-Z0-9_]*;
INT           : [0-9]+;
DOUBLE        : [0-9]+'.'[0-9]* | [0-9]*'.'[0-9]+;
EOL           : ';';
WS            : [ \t\r\n]+ -> skip;
LINE_COMMENT  : '//' ~[\r\n]* -> skip;
BLOCK_COMMENT : '/*' .*? '*/' -> skip;
STRING_LITERAL: '"' (~["\\\r\n] | '\\' .)* '"';
CHAR_LITERAL  : '\'' ( ~['\\] | '\\' [0trn'\\] ) '\'';
POINTER       : '*';
ADDRESS       : '&';
