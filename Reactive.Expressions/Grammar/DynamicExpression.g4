grammar DynamicExpression;

// Parser rules
expression: orExpression;

orExpression: andExpression (('||' | 'OR') andExpression)*;

andExpression: condition (('&&' | 'AND') condition)*;

condition:
	aggregationCondition
	| simpleCondition
	| '(' expression ')';

aggregationCondition:
	aggregationType '(' metricName ',' timeWindow ')' operator threshold;

simpleCondition: metricName operator threshold;

aggregationType:
	'avg'
	| 'sum'
	| 'max'
	| 'min'
	| 'AVG'
	| 'SUM'
	| 'MAX'
	| 'MIN';

timeWindow: NUMBER timeUnit;

timeUnit: 's' | 'm' | 'h' | 'S' | 'M' | 'H';

operator: '>=' | '<=' | '>' | '<' | '==' | '!=';

threshold: arithmeticExpression;

arithmeticExpression:
	multiplyDivideExpression (
		('+' | '-') multiplyDivideExpression
	)*;

multiplyDivideExpression:
	primaryExpression (('*' | '/') primaryExpression)*;

primaryExpression:
	NUMBER							# NumberExpression
	| IDENTIFIER					# VariableExpression
	| '(' arithmeticExpression ')'	# ParenthesizedArithmeticExpression;

metricName: IDENTIFIER;

// Lexer rules
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;

NUMBER: [0-9]+ ('.' [0-9]+)?;

// Skip whitespace
WS: [ \t\r\n]+ -> skip;
