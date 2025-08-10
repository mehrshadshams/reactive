// Generated from /Users/mehrshadshams/Projects/github/mehrshadshams/reactive/Reactive.Expressions/Grammar/DynamicExpression.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast", "CheckReturnValue"})
public class DynamicExpressionParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.13.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, T__15=16, T__16=17, 
		T__17=18, T__18=19, T__19=20, T__20=21, T__21=22, T__22=23, T__23=24, 
		T__24=25, T__25=26, T__26=27, T__27=28, T__28=29, T__29=30, T__30=31, 
		IDENTIFIER=32, NUMBER=33, WS=34;
	public static final int
		RULE_expression = 0, RULE_orExpression = 1, RULE_andExpression = 2, RULE_condition = 3, 
		RULE_aggregationCondition = 4, RULE_simpleCondition = 5, RULE_aggregationType = 6, 
		RULE_timeWindow = 7, RULE_timeUnit = 8, RULE_operator = 9, RULE_threshold = 10, 
		RULE_arithmeticExpression = 11, RULE_multiplyDivideExpression = 12, RULE_primaryExpression = 13, 
		RULE_metricName = 14;
	private static String[] makeRuleNames() {
		return new String[] {
			"expression", "orExpression", "andExpression", "condition", "aggregationCondition", 
			"simpleCondition", "aggregationType", "timeWindow", "timeUnit", "operator", 
			"threshold", "arithmeticExpression", "multiplyDivideExpression", "primaryExpression", 
			"metricName"
		};
	}
	public static final String[] ruleNames = makeRuleNames();

	private static String[] makeLiteralNames() {
		return new String[] {
			null, "'||'", "'OR'", "'&&'", "'AND'", "'('", "')'", "','", "'avg'", 
			"'sum'", "'max'", "'min'", "'AVG'", "'SUM'", "'MAX'", "'MIN'", "'s'", 
			"'m'", "'h'", "'S'", "'M'", "'H'", "'>='", "'<='", "'>'", "'<'", "'=='", 
			"'!='", "'+'", "'-'", "'*'", "'/'"
		};
	}
	private static final String[] _LITERAL_NAMES = makeLiteralNames();
	private static String[] makeSymbolicNames() {
		return new String[] {
			null, null, null, null, null, null, null, null, null, null, null, null, 
			null, null, null, null, null, null, null, null, null, null, null, null, 
			null, null, null, null, null, null, null, null, "IDENTIFIER", "NUMBER", 
			"WS"
		};
	}
	private static final String[] _SYMBOLIC_NAMES = makeSymbolicNames();
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "DynamicExpression.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public DynamicExpressionParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ExpressionContext extends ParserRuleContext {
		public OrExpressionContext orExpression() {
			return getRuleContext(OrExpressionContext.class,0);
		}
		public ExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expression; }
	}

	public final ExpressionContext expression() throws RecognitionException {
		ExpressionContext _localctx = new ExpressionContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_expression);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(30);
			orExpression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class OrExpressionContext extends ParserRuleContext {
		public List<AndExpressionContext> andExpression() {
			return getRuleContexts(AndExpressionContext.class);
		}
		public AndExpressionContext andExpression(int i) {
			return getRuleContext(AndExpressionContext.class,i);
		}
		public OrExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_orExpression; }
	}

	public final OrExpressionContext orExpression() throws RecognitionException {
		OrExpressionContext _localctx = new OrExpressionContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_orExpression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(32);
			andExpression();
			setState(37);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__0 || _la==T__1) {
				{
				{
				setState(33);
				_la = _input.LA(1);
				if ( !(_la==T__0 || _la==T__1) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				setState(34);
				andExpression();
				}
				}
				setState(39);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class AndExpressionContext extends ParserRuleContext {
		public List<ConditionContext> condition() {
			return getRuleContexts(ConditionContext.class);
		}
		public ConditionContext condition(int i) {
			return getRuleContext(ConditionContext.class,i);
		}
		public AndExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_andExpression; }
	}

	public final AndExpressionContext andExpression() throws RecognitionException {
		AndExpressionContext _localctx = new AndExpressionContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_andExpression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(40);
			condition();
			setState(45);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__2 || _la==T__3) {
				{
				{
				setState(41);
				_la = _input.LA(1);
				if ( !(_la==T__2 || _la==T__3) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				setState(42);
				condition();
				}
				}
				setState(47);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ConditionContext extends ParserRuleContext {
		public AggregationConditionContext aggregationCondition() {
			return getRuleContext(AggregationConditionContext.class,0);
		}
		public SimpleConditionContext simpleCondition() {
			return getRuleContext(SimpleConditionContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public ConditionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_condition; }
	}

	public final ConditionContext condition() throws RecognitionException {
		ConditionContext _localctx = new ConditionContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_condition);
		try {
			setState(54);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__7:
			case T__8:
			case T__9:
			case T__10:
			case T__11:
			case T__12:
			case T__13:
			case T__14:
				enterOuterAlt(_localctx, 1);
				{
				setState(48);
				aggregationCondition();
				}
				break;
			case IDENTIFIER:
				enterOuterAlt(_localctx, 2);
				{
				setState(49);
				simpleCondition();
				}
				break;
			case T__4:
				enterOuterAlt(_localctx, 3);
				{
				setState(50);
				match(T__4);
				setState(51);
				expression();
				setState(52);
				match(T__5);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class AggregationConditionContext extends ParserRuleContext {
		public AggregationTypeContext aggregationType() {
			return getRuleContext(AggregationTypeContext.class,0);
		}
		public MetricNameContext metricName() {
			return getRuleContext(MetricNameContext.class,0);
		}
		public TimeWindowContext timeWindow() {
			return getRuleContext(TimeWindowContext.class,0);
		}
		public OperatorContext operator() {
			return getRuleContext(OperatorContext.class,0);
		}
		public ThresholdContext threshold() {
			return getRuleContext(ThresholdContext.class,0);
		}
		public AggregationConditionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_aggregationCondition; }
	}

	public final AggregationConditionContext aggregationCondition() throws RecognitionException {
		AggregationConditionContext _localctx = new AggregationConditionContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_aggregationCondition);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(56);
			aggregationType();
			setState(57);
			match(T__4);
			setState(58);
			metricName();
			setState(59);
			match(T__6);
			setState(60);
			timeWindow();
			setState(61);
			match(T__5);
			setState(62);
			operator();
			setState(63);
			threshold();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class SimpleConditionContext extends ParserRuleContext {
		public MetricNameContext metricName() {
			return getRuleContext(MetricNameContext.class,0);
		}
		public OperatorContext operator() {
			return getRuleContext(OperatorContext.class,0);
		}
		public ThresholdContext threshold() {
			return getRuleContext(ThresholdContext.class,0);
		}
		public SimpleConditionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_simpleCondition; }
	}

	public final SimpleConditionContext simpleCondition() throws RecognitionException {
		SimpleConditionContext _localctx = new SimpleConditionContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_simpleCondition);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(65);
			metricName();
			setState(66);
			operator();
			setState(67);
			threshold();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class AggregationTypeContext extends ParserRuleContext {
		public AggregationTypeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_aggregationType; }
	}

	public final AggregationTypeContext aggregationType() throws RecognitionException {
		AggregationTypeContext _localctx = new AggregationTypeContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_aggregationType);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(69);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 65280L) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class TimeWindowContext extends ParserRuleContext {
		public TerminalNode NUMBER() { return getToken(DynamicExpressionParser.NUMBER, 0); }
		public TimeUnitContext timeUnit() {
			return getRuleContext(TimeUnitContext.class,0);
		}
		public TimeWindowContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_timeWindow; }
	}

	public final TimeWindowContext timeWindow() throws RecognitionException {
		TimeWindowContext _localctx = new TimeWindowContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_timeWindow);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(71);
			match(NUMBER);
			setState(72);
			timeUnit();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class TimeUnitContext extends ParserRuleContext {
		public TimeUnitContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_timeUnit; }
	}

	public final TimeUnitContext timeUnit() throws RecognitionException {
		TimeUnitContext _localctx = new TimeUnitContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_timeUnit);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(74);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 4128768L) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class OperatorContext extends ParserRuleContext {
		public OperatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operator; }
	}

	public final OperatorContext operator() throws RecognitionException {
		OperatorContext _localctx = new OperatorContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_operator);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(76);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 264241152L) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ThresholdContext extends ParserRuleContext {
		public ArithmeticExpressionContext arithmeticExpression() {
			return getRuleContext(ArithmeticExpressionContext.class,0);
		}
		public ThresholdContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_threshold; }
	}

	public final ThresholdContext threshold() throws RecognitionException {
		ThresholdContext _localctx = new ThresholdContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_threshold);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(78);
			arithmeticExpression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ArithmeticExpressionContext extends ParserRuleContext {
		public List<MultiplyDivideExpressionContext> multiplyDivideExpression() {
			return getRuleContexts(MultiplyDivideExpressionContext.class);
		}
		public MultiplyDivideExpressionContext multiplyDivideExpression(int i) {
			return getRuleContext(MultiplyDivideExpressionContext.class,i);
		}
		public ArithmeticExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_arithmeticExpression; }
	}

	public final ArithmeticExpressionContext arithmeticExpression() throws RecognitionException {
		ArithmeticExpressionContext _localctx = new ArithmeticExpressionContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_arithmeticExpression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(80);
			multiplyDivideExpression();
			setState(85);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__27 || _la==T__28) {
				{
				{
				setState(81);
				_la = _input.LA(1);
				if ( !(_la==T__27 || _la==T__28) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				setState(82);
				multiplyDivideExpression();
				}
				}
				setState(87);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class MultiplyDivideExpressionContext extends ParserRuleContext {
		public List<PrimaryExpressionContext> primaryExpression() {
			return getRuleContexts(PrimaryExpressionContext.class);
		}
		public PrimaryExpressionContext primaryExpression(int i) {
			return getRuleContext(PrimaryExpressionContext.class,i);
		}
		public MultiplyDivideExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_multiplyDivideExpression; }
	}

	public final MultiplyDivideExpressionContext multiplyDivideExpression() throws RecognitionException {
		MultiplyDivideExpressionContext _localctx = new MultiplyDivideExpressionContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_multiplyDivideExpression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(88);
			primaryExpression();
			setState(93);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__29 || _la==T__30) {
				{
				{
				setState(89);
				_la = _input.LA(1);
				if ( !(_la==T__29 || _la==T__30) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				setState(90);
				primaryExpression();
				}
				}
				setState(95);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class PrimaryExpressionContext extends ParserRuleContext {
		public PrimaryExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_primaryExpression; }
	 
		public PrimaryExpressionContext() { }
		public void copyFrom(PrimaryExpressionContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class ParenthesizedArithmeticExpressionContext extends PrimaryExpressionContext {
		public ArithmeticExpressionContext arithmeticExpression() {
			return getRuleContext(ArithmeticExpressionContext.class,0);
		}
		public ParenthesizedArithmeticExpressionContext(PrimaryExpressionContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class NumberExpressionContext extends PrimaryExpressionContext {
		public TerminalNode NUMBER() { return getToken(DynamicExpressionParser.NUMBER, 0); }
		public NumberExpressionContext(PrimaryExpressionContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class VariableExpressionContext extends PrimaryExpressionContext {
		public TerminalNode IDENTIFIER() { return getToken(DynamicExpressionParser.IDENTIFIER, 0); }
		public VariableExpressionContext(PrimaryExpressionContext ctx) { copyFrom(ctx); }
	}

	public final PrimaryExpressionContext primaryExpression() throws RecognitionException {
		PrimaryExpressionContext _localctx = new PrimaryExpressionContext(_ctx, getState());
		enterRule(_localctx, 26, RULE_primaryExpression);
		try {
			setState(102);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case NUMBER:
				_localctx = new NumberExpressionContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(96);
				match(NUMBER);
				}
				break;
			case IDENTIFIER:
				_localctx = new VariableExpressionContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(97);
				match(IDENTIFIER);
				}
				break;
			case T__4:
				_localctx = new ParenthesizedArithmeticExpressionContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(98);
				match(T__4);
				setState(99);
				arithmeticExpression();
				setState(100);
				match(T__5);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class MetricNameContext extends ParserRuleContext {
		public TerminalNode IDENTIFIER() { return getToken(DynamicExpressionParser.IDENTIFIER, 0); }
		public MetricNameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_metricName; }
	}

	public final MetricNameContext metricName() throws RecognitionException {
		MetricNameContext _localctx = new MetricNameContext(_ctx, getState());
		enterRule(_localctx, 28, RULE_metricName);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(104);
			match(IDENTIFIER);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static final String _serializedATN =
		"\u0004\u0001\"k\u0002\u0000\u0007\u0000\u0002\u0001\u0007\u0001\u0002"+
		"\u0002\u0007\u0002\u0002\u0003\u0007\u0003\u0002\u0004\u0007\u0004\u0002"+
		"\u0005\u0007\u0005\u0002\u0006\u0007\u0006\u0002\u0007\u0007\u0007\u0002"+
		"\b\u0007\b\u0002\t\u0007\t\u0002\n\u0007\n\u0002\u000b\u0007\u000b\u0002"+
		"\f\u0007\f\u0002\r\u0007\r\u0002\u000e\u0007\u000e\u0001\u0000\u0001\u0000"+
		"\u0001\u0001\u0001\u0001\u0001\u0001\u0005\u0001$\b\u0001\n\u0001\f\u0001"+
		"\'\t\u0001\u0001\u0002\u0001\u0002\u0001\u0002\u0005\u0002,\b\u0002\n"+
		"\u0002\f\u0002/\t\u0002\u0001\u0003\u0001\u0003\u0001\u0003\u0001\u0003"+
		"\u0001\u0003\u0001\u0003\u0003\u00037\b\u0003\u0001\u0004\u0001\u0004"+
		"\u0001\u0004\u0001\u0004\u0001\u0004\u0001\u0004\u0001\u0004\u0001\u0004"+
		"\u0001\u0004\u0001\u0005\u0001\u0005\u0001\u0005\u0001\u0005\u0001\u0006"+
		"\u0001\u0006\u0001\u0007\u0001\u0007\u0001\u0007\u0001\b\u0001\b\u0001"+
		"\t\u0001\t\u0001\n\u0001\n\u0001\u000b\u0001\u000b\u0001\u000b\u0005\u000b"+
		"T\b\u000b\n\u000b\f\u000bW\t\u000b\u0001\f\u0001\f\u0001\f\u0005\f\\\b"+
		"\f\n\f\f\f_\t\f\u0001\r\u0001\r\u0001\r\u0001\r\u0001\r\u0001\r\u0003"+
		"\rg\b\r\u0001\u000e\u0001\u000e\u0001\u000e\u0000\u0000\u000f\u0000\u0002"+
		"\u0004\u0006\b\n\f\u000e\u0010\u0012\u0014\u0016\u0018\u001a\u001c\u0000"+
		"\u0007\u0001\u0000\u0001\u0002\u0001\u0000\u0003\u0004\u0001\u0000\b\u000f"+
		"\u0001\u0000\u0010\u0015\u0001\u0000\u0016\u001b\u0001\u0000\u001c\u001d"+
		"\u0001\u0000\u001e\u001fc\u0000\u001e\u0001\u0000\u0000\u0000\u0002 \u0001"+
		"\u0000\u0000\u0000\u0004(\u0001\u0000\u0000\u0000\u00066\u0001\u0000\u0000"+
		"\u0000\b8\u0001\u0000\u0000\u0000\nA\u0001\u0000\u0000\u0000\fE\u0001"+
		"\u0000\u0000\u0000\u000eG\u0001\u0000\u0000\u0000\u0010J\u0001\u0000\u0000"+
		"\u0000\u0012L\u0001\u0000\u0000\u0000\u0014N\u0001\u0000\u0000\u0000\u0016"+
		"P\u0001\u0000\u0000\u0000\u0018X\u0001\u0000\u0000\u0000\u001af\u0001"+
		"\u0000\u0000\u0000\u001ch\u0001\u0000\u0000\u0000\u001e\u001f\u0003\u0002"+
		"\u0001\u0000\u001f\u0001\u0001\u0000\u0000\u0000 %\u0003\u0004\u0002\u0000"+
		"!\"\u0007\u0000\u0000\u0000\"$\u0003\u0004\u0002\u0000#!\u0001\u0000\u0000"+
		"\u0000$\'\u0001\u0000\u0000\u0000%#\u0001\u0000\u0000\u0000%&\u0001\u0000"+
		"\u0000\u0000&\u0003\u0001\u0000\u0000\u0000\'%\u0001\u0000\u0000\u0000"+
		"(-\u0003\u0006\u0003\u0000)*\u0007\u0001\u0000\u0000*,\u0003\u0006\u0003"+
		"\u0000+)\u0001\u0000\u0000\u0000,/\u0001\u0000\u0000\u0000-+\u0001\u0000"+
		"\u0000\u0000-.\u0001\u0000\u0000\u0000.\u0005\u0001\u0000\u0000\u0000"+
		"/-\u0001\u0000\u0000\u000007\u0003\b\u0004\u000017\u0003\n\u0005\u0000"+
		"23\u0005\u0005\u0000\u000034\u0003\u0000\u0000\u000045\u0005\u0006\u0000"+
		"\u000057\u0001\u0000\u0000\u000060\u0001\u0000\u0000\u000061\u0001\u0000"+
		"\u0000\u000062\u0001\u0000\u0000\u00007\u0007\u0001\u0000\u0000\u0000"+
		"89\u0003\f\u0006\u00009:\u0005\u0005\u0000\u0000:;\u0003\u001c\u000e\u0000"+
		";<\u0005\u0007\u0000\u0000<=\u0003\u000e\u0007\u0000=>\u0005\u0006\u0000"+
		"\u0000>?\u0003\u0012\t\u0000?@\u0003\u0014\n\u0000@\t\u0001\u0000\u0000"+
		"\u0000AB\u0003\u001c\u000e\u0000BC\u0003\u0012\t\u0000CD\u0003\u0014\n"+
		"\u0000D\u000b\u0001\u0000\u0000\u0000EF\u0007\u0002\u0000\u0000F\r\u0001"+
		"\u0000\u0000\u0000GH\u0005!\u0000\u0000HI\u0003\u0010\b\u0000I\u000f\u0001"+
		"\u0000\u0000\u0000JK\u0007\u0003\u0000\u0000K\u0011\u0001\u0000\u0000"+
		"\u0000LM\u0007\u0004\u0000\u0000M\u0013\u0001\u0000\u0000\u0000NO\u0003"+
		"\u0016\u000b\u0000O\u0015\u0001\u0000\u0000\u0000PU\u0003\u0018\f\u0000"+
		"QR\u0007\u0005\u0000\u0000RT\u0003\u0018\f\u0000SQ\u0001\u0000\u0000\u0000"+
		"TW\u0001\u0000\u0000\u0000US\u0001\u0000\u0000\u0000UV\u0001\u0000\u0000"+
		"\u0000V\u0017\u0001\u0000\u0000\u0000WU\u0001\u0000\u0000\u0000X]\u0003"+
		"\u001a\r\u0000YZ\u0007\u0006\u0000\u0000Z\\\u0003\u001a\r\u0000[Y\u0001"+
		"\u0000\u0000\u0000\\_\u0001\u0000\u0000\u0000][\u0001\u0000\u0000\u0000"+
		"]^\u0001\u0000\u0000\u0000^\u0019\u0001\u0000\u0000\u0000_]\u0001\u0000"+
		"\u0000\u0000`g\u0005!\u0000\u0000ag\u0005 \u0000\u0000bc\u0005\u0005\u0000"+
		"\u0000cd\u0003\u0016\u000b\u0000de\u0005\u0006\u0000\u0000eg\u0001\u0000"+
		"\u0000\u0000f`\u0001\u0000\u0000\u0000fa\u0001\u0000\u0000\u0000fb\u0001"+
		"\u0000\u0000\u0000g\u001b\u0001\u0000\u0000\u0000hi\u0005 \u0000\u0000"+
		"i\u001d\u0001\u0000\u0000\u0000\u0006%-6U]f";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}