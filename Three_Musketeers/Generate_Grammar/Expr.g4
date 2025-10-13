grammar Expr;

start
    : prog+ EOF
    ;

prog
    : stm
    | new_type
    | function
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
    : type ID '(' args? ')' '{' func_body '}'
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
    : 'puts' '(' (ID | STRING_LITERAL) ')' EOL
    ;

att
    : type? ID '=' expr                    # AttRegular
    | type? ID '+=' expr                   # AttPlusEquals
    | type? ID '-=' expr                   # AttMinusEquals
    | type? ID '*=' expr                   # AttMultiplyEquals
    | type? ID '/=' expr                   # AttDivideEquals
    ;

att_var 
    : ID index+ '=' expr                          # SingleAtt
    | ID index+ '+=' expr                         # SingleAttPlusEquals
    | ID index+ '-=' expr                         # SingleAttMinusEquals
    | ID index+ '*=' expr                         # SingleAttMultiplyEquals
    | ID index+ '/=' expr                         # SingleAttDivideEquals
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
    : '++' ID                      # PrefixIncrement
    | '--' ID                      # PrefixDecrement
    | ID '++'                      # PostfixIncrement
    | ID '--'                      # PostfixDecrement
    | '++' ID index+               # PrefixIncrementArray
    | '--' ID index+               # PrefixDecrementArray
    | ID index+ '++'               # PostfixIncrementArray
    | ID index+ '--'               # PostfixDecrementArray
    | expr ('&&'|'||') expr        # LogicalAndOr
    | expr ('=='|'!=') expr        # Equality
    | expr ('>'|'<'|'>='|'<=') expr # Comparison
    | expr ('*'|'/'|'%') expr  # MulDivMod
    | expr ('+'|'-') expr      # AddSub
    | '(' expr ')'             # Parens
    | '!' expr                 # LogicalNot
    | '-' expr                 # UnaryMinus
    | 'atoi' '(' expr ')'           # AtoiConversion
    | 'atod' '(' expr ')'           # AtodConversion
    | 'itoa' '(' expr ')'           # ItoaConversion
    | 'dtoa' '(' expr ')'           # DtoaConversion
    | ID                       # Var
    | ID index+                # VarArray
    | INT                      # IntLiteral
    | DOUBLE                   # DoubleLiteral
    | STRING_LITERAL           # StringLiteral
    | CHAR_LITERAL             # CharLiteral
    | TRUE                     # TrueLiteral
    | FALSE                    # FalseLiteral
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
