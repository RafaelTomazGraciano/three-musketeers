grammar Expr;

start
    : prog* mainFunction EOF
    ;

prog
    : stm
    | new_type
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
    | att_var EOL
    | printfStatement
    | scanfStatement
    | getsStatement
    | putsStatement
    | RETURN expr? EOL
    ;

    declaration
    : type ID index*
    ;

function
    : function_return ID '(' args? ')' '{' func_body '}'
    ;

function_return
    : type
    | VOID
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
    : type? ID '=' expr
    ;

att_var 
    : ID index+ '=' expr             #SingleAtt
    ;

new_type
    : 'type' ID 'as' type EOL
    ;

args
    : type ID (',' type ID)*
    ;

index
    : '[' INT ']'
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
    | INT                            # IntLiteral
    | DOUBLE                         # DoubleLiteral
    | STRING_LITERAL                 # StringLiteral
    | CHAR_LITERAL                   # CharLiteral
    | TRUE                           # TrueLiteral
    | FALSE                          # FalseLiteral
    ;

type
    : 'int'
    | 'double'
    | 'bool'
    | 'char'
    | 'string'
    | ID
    ;

/* -------- TOKENS -------- */
RETURN        : 'return';
VOID          : 'void';
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
