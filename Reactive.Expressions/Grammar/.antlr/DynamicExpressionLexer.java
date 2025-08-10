// Generated from /Users/mehrshadshams/Projects/github/mehrshadshams/reactive/Reactive.Expressions/Grammar/DynamicExpression.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.Token;
import org.antlr.v4.runtime.TokenStream;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.misc.*;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast", "CheckReturnValue", "this-escape"})
public class DynamicExpressionLexer extends Lexer {
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
	public static String[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static String[] modeNames = {
		"DEFAULT_MODE"
	};

	private static String[] makeRuleNames() {
		return new String[] {
			"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
			"T__9", "T__10", "T__11", "T__12", "T__13", "T__14", "T__15", "T__16", 
			"T__17", "T__18", "T__19", "T__20", "T__21", "T__22", "T__23", "T__24", 
			"T__25", "T__26", "T__27", "T__28", "T__29", "T__30", "IDENTIFIER", "NUMBER", 
			"WS"
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


	public DynamicExpressionLexer(CharStream input) {
		super(input);
		_interp = new LexerATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@Override
	public String getGrammarFileName() { return "DynamicExpression.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public String[] getChannelNames() { return channelNames; }

	@Override
	public String[] getModeNames() { return modeNames; }

	@Override
	public ATN getATN() { return _ATN; }

	public static final String _serializedATN =
		"\u0004\u0000\"\u00b7\u0006\uffff\uffff\u0002\u0000\u0007\u0000\u0002\u0001"+
		"\u0007\u0001\u0002\u0002\u0007\u0002\u0002\u0003\u0007\u0003\u0002\u0004"+
		"\u0007\u0004\u0002\u0005\u0007\u0005\u0002\u0006\u0007\u0006\u0002\u0007"+
		"\u0007\u0007\u0002\b\u0007\b\u0002\t\u0007\t\u0002\n\u0007\n\u0002\u000b"+
		"\u0007\u000b\u0002\f\u0007\f\u0002\r\u0007\r\u0002\u000e\u0007\u000e\u0002"+
		"\u000f\u0007\u000f\u0002\u0010\u0007\u0010\u0002\u0011\u0007\u0011\u0002"+
		"\u0012\u0007\u0012\u0002\u0013\u0007\u0013\u0002\u0014\u0007\u0014\u0002"+
		"\u0015\u0007\u0015\u0002\u0016\u0007\u0016\u0002\u0017\u0007\u0017\u0002"+
		"\u0018\u0007\u0018\u0002\u0019\u0007\u0019\u0002\u001a\u0007\u001a\u0002"+
		"\u001b\u0007\u001b\u0002\u001c\u0007\u001c\u0002\u001d\u0007\u001d\u0002"+
		"\u001e\u0007\u001e\u0002\u001f\u0007\u001f\u0002 \u0007 \u0002!\u0007"+
		"!\u0001\u0000\u0001\u0000\u0001\u0000\u0001\u0001\u0001\u0001\u0001\u0001"+
		"\u0001\u0002\u0001\u0002\u0001\u0002\u0001\u0003\u0001\u0003\u0001\u0003"+
		"\u0001\u0003\u0001\u0004\u0001\u0004\u0001\u0005\u0001\u0005\u0001\u0006"+
		"\u0001\u0006\u0001\u0007\u0001\u0007\u0001\u0007\u0001\u0007\u0001\b\u0001"+
		"\b\u0001\b\u0001\b\u0001\t\u0001\t\u0001\t\u0001\t\u0001\n\u0001\n\u0001"+
		"\n\u0001\n\u0001\u000b\u0001\u000b\u0001\u000b\u0001\u000b\u0001\f\u0001"+
		"\f\u0001\f\u0001\f\u0001\r\u0001\r\u0001\r\u0001\r\u0001\u000e\u0001\u000e"+
		"\u0001\u000e\u0001\u000e\u0001\u000f\u0001\u000f\u0001\u0010\u0001\u0010"+
		"\u0001\u0011\u0001\u0011\u0001\u0012\u0001\u0012\u0001\u0013\u0001\u0013"+
		"\u0001\u0014\u0001\u0014\u0001\u0015\u0001\u0015\u0001\u0015\u0001\u0016"+
		"\u0001\u0016\u0001\u0016\u0001\u0017\u0001\u0017\u0001\u0018\u0001\u0018"+
		"\u0001\u0019\u0001\u0019\u0001\u0019\u0001\u001a\u0001\u001a\u0001\u001a"+
		"\u0001\u001b\u0001\u001b\u0001\u001c\u0001\u001c\u0001\u001d\u0001\u001d"+
		"\u0001\u001e\u0001\u001e\u0001\u001f\u0001\u001f\u0005\u001f\u009f\b\u001f"+
		"\n\u001f\f\u001f\u00a2\t\u001f\u0001 \u0004 \u00a5\b \u000b \f \u00a6"+
		"\u0001 \u0001 \u0004 \u00ab\b \u000b \f \u00ac\u0003 \u00af\b \u0001!"+
		"\u0004!\u00b2\b!\u000b!\f!\u00b3\u0001!\u0001!\u0000\u0000\"\u0001\u0001"+
		"\u0003\u0002\u0005\u0003\u0007\u0004\t\u0005\u000b\u0006\r\u0007\u000f"+
		"\b\u0011\t\u0013\n\u0015\u000b\u0017\f\u0019\r\u001b\u000e\u001d\u000f"+
		"\u001f\u0010!\u0011#\u0012%\u0013\'\u0014)\u0015+\u0016-\u0017/\u0018"+
		"1\u00193\u001a5\u001b7\u001c9\u001d;\u001e=\u001f? A!C\"\u0001\u0000\u0004"+
		"\u0003\u0000AZ__az\u0004\u000009AZ__az\u0001\u000009\u0003\u0000\t\n\r"+
		"\r  \u00bb\u0000\u0001\u0001\u0000\u0000\u0000\u0000\u0003\u0001\u0000"+
		"\u0000\u0000\u0000\u0005\u0001\u0000\u0000\u0000\u0000\u0007\u0001\u0000"+
		"\u0000\u0000\u0000\t\u0001\u0000\u0000\u0000\u0000\u000b\u0001\u0000\u0000"+
		"\u0000\u0000\r\u0001\u0000\u0000\u0000\u0000\u000f\u0001\u0000\u0000\u0000"+
		"\u0000\u0011\u0001\u0000\u0000\u0000\u0000\u0013\u0001\u0000\u0000\u0000"+
		"\u0000\u0015\u0001\u0000\u0000\u0000\u0000\u0017\u0001\u0000\u0000\u0000"+
		"\u0000\u0019\u0001\u0000\u0000\u0000\u0000\u001b\u0001\u0000\u0000\u0000"+
		"\u0000\u001d\u0001\u0000\u0000\u0000\u0000\u001f\u0001\u0000\u0000\u0000"+
		"\u0000!\u0001\u0000\u0000\u0000\u0000#\u0001\u0000\u0000\u0000\u0000%"+
		"\u0001\u0000\u0000\u0000\u0000\'\u0001\u0000\u0000\u0000\u0000)\u0001"+
		"\u0000\u0000\u0000\u0000+\u0001\u0000\u0000\u0000\u0000-\u0001\u0000\u0000"+
		"\u0000\u0000/\u0001\u0000\u0000\u0000\u00001\u0001\u0000\u0000\u0000\u0000"+
		"3\u0001\u0000\u0000\u0000\u00005\u0001\u0000\u0000\u0000\u00007\u0001"+
		"\u0000\u0000\u0000\u00009\u0001\u0000\u0000\u0000\u0000;\u0001\u0000\u0000"+
		"\u0000\u0000=\u0001\u0000\u0000\u0000\u0000?\u0001\u0000\u0000\u0000\u0000"+
		"A\u0001\u0000\u0000\u0000\u0000C\u0001\u0000\u0000\u0000\u0001E\u0001"+
		"\u0000\u0000\u0000\u0003H\u0001\u0000\u0000\u0000\u0005K\u0001\u0000\u0000"+
		"\u0000\u0007N\u0001\u0000\u0000\u0000\tR\u0001\u0000\u0000\u0000\u000b"+
		"T\u0001\u0000\u0000\u0000\rV\u0001\u0000\u0000\u0000\u000fX\u0001\u0000"+
		"\u0000\u0000\u0011\\\u0001\u0000\u0000\u0000\u0013`\u0001\u0000\u0000"+
		"\u0000\u0015d\u0001\u0000\u0000\u0000\u0017h\u0001\u0000\u0000\u0000\u0019"+
		"l\u0001\u0000\u0000\u0000\u001bp\u0001\u0000\u0000\u0000\u001dt\u0001"+
		"\u0000\u0000\u0000\u001fx\u0001\u0000\u0000\u0000!z\u0001\u0000\u0000"+
		"\u0000#|\u0001\u0000\u0000\u0000%~\u0001\u0000\u0000\u0000\'\u0080\u0001"+
		"\u0000\u0000\u0000)\u0082\u0001\u0000\u0000\u0000+\u0084\u0001\u0000\u0000"+
		"\u0000-\u0087\u0001\u0000\u0000\u0000/\u008a\u0001\u0000\u0000\u00001"+
		"\u008c\u0001\u0000\u0000\u00003\u008e\u0001\u0000\u0000\u00005\u0091\u0001"+
		"\u0000\u0000\u00007\u0094\u0001\u0000\u0000\u00009\u0096\u0001\u0000\u0000"+
		"\u0000;\u0098\u0001\u0000\u0000\u0000=\u009a\u0001\u0000\u0000\u0000?"+
		"\u009c\u0001\u0000\u0000\u0000A\u00a4\u0001\u0000\u0000\u0000C\u00b1\u0001"+
		"\u0000\u0000\u0000EF\u0005|\u0000\u0000FG\u0005|\u0000\u0000G\u0002\u0001"+
		"\u0000\u0000\u0000HI\u0005O\u0000\u0000IJ\u0005R\u0000\u0000J\u0004\u0001"+
		"\u0000\u0000\u0000KL\u0005&\u0000\u0000LM\u0005&\u0000\u0000M\u0006\u0001"+
		"\u0000\u0000\u0000NO\u0005A\u0000\u0000OP\u0005N\u0000\u0000PQ\u0005D"+
		"\u0000\u0000Q\b\u0001\u0000\u0000\u0000RS\u0005(\u0000\u0000S\n\u0001"+
		"\u0000\u0000\u0000TU\u0005)\u0000\u0000U\f\u0001\u0000\u0000\u0000VW\u0005"+
		",\u0000\u0000W\u000e\u0001\u0000\u0000\u0000XY\u0005a\u0000\u0000YZ\u0005"+
		"v\u0000\u0000Z[\u0005g\u0000\u0000[\u0010\u0001\u0000\u0000\u0000\\]\u0005"+
		"s\u0000\u0000]^\u0005u\u0000\u0000^_\u0005m\u0000\u0000_\u0012\u0001\u0000"+
		"\u0000\u0000`a\u0005m\u0000\u0000ab\u0005a\u0000\u0000bc\u0005x\u0000"+
		"\u0000c\u0014\u0001\u0000\u0000\u0000de\u0005m\u0000\u0000ef\u0005i\u0000"+
		"\u0000fg\u0005n\u0000\u0000g\u0016\u0001\u0000\u0000\u0000hi\u0005A\u0000"+
		"\u0000ij\u0005V\u0000\u0000jk\u0005G\u0000\u0000k\u0018\u0001\u0000\u0000"+
		"\u0000lm\u0005S\u0000\u0000mn\u0005U\u0000\u0000no\u0005M\u0000\u0000"+
		"o\u001a\u0001\u0000\u0000\u0000pq\u0005M\u0000\u0000qr\u0005A\u0000\u0000"+
		"rs\u0005X\u0000\u0000s\u001c\u0001\u0000\u0000\u0000tu\u0005M\u0000\u0000"+
		"uv\u0005I\u0000\u0000vw\u0005N\u0000\u0000w\u001e\u0001\u0000\u0000\u0000"+
		"xy\u0005s\u0000\u0000y \u0001\u0000\u0000\u0000z{\u0005m\u0000\u0000{"+
		"\"\u0001\u0000\u0000\u0000|}\u0005h\u0000\u0000}$\u0001\u0000\u0000\u0000"+
		"~\u007f\u0005S\u0000\u0000\u007f&\u0001\u0000\u0000\u0000\u0080\u0081"+
		"\u0005M\u0000\u0000\u0081(\u0001\u0000\u0000\u0000\u0082\u0083\u0005H"+
		"\u0000\u0000\u0083*\u0001\u0000\u0000\u0000\u0084\u0085\u0005>\u0000\u0000"+
		"\u0085\u0086\u0005=\u0000\u0000\u0086,\u0001\u0000\u0000\u0000\u0087\u0088"+
		"\u0005<\u0000\u0000\u0088\u0089\u0005=\u0000\u0000\u0089.\u0001\u0000"+
		"\u0000\u0000\u008a\u008b\u0005>\u0000\u0000\u008b0\u0001\u0000\u0000\u0000"+
		"\u008c\u008d\u0005<\u0000\u0000\u008d2\u0001\u0000\u0000\u0000\u008e\u008f"+
		"\u0005=\u0000\u0000\u008f\u0090\u0005=\u0000\u0000\u00904\u0001\u0000"+
		"\u0000\u0000\u0091\u0092\u0005!\u0000\u0000\u0092\u0093\u0005=\u0000\u0000"+
		"\u00936\u0001\u0000\u0000\u0000\u0094\u0095\u0005+\u0000\u0000\u00958"+
		"\u0001\u0000\u0000\u0000\u0096\u0097\u0005-\u0000\u0000\u0097:\u0001\u0000"+
		"\u0000\u0000\u0098\u0099\u0005*\u0000\u0000\u0099<\u0001\u0000\u0000\u0000"+
		"\u009a\u009b\u0005/\u0000\u0000\u009b>\u0001\u0000\u0000\u0000\u009c\u00a0"+
		"\u0007\u0000\u0000\u0000\u009d\u009f\u0007\u0001\u0000\u0000\u009e\u009d"+
		"\u0001\u0000\u0000\u0000\u009f\u00a2\u0001\u0000\u0000\u0000\u00a0\u009e"+
		"\u0001\u0000\u0000\u0000\u00a0\u00a1\u0001\u0000\u0000\u0000\u00a1@\u0001"+
		"\u0000\u0000\u0000\u00a2\u00a0\u0001\u0000\u0000\u0000\u00a3\u00a5\u0007"+
		"\u0002\u0000\u0000\u00a4\u00a3\u0001\u0000\u0000\u0000\u00a5\u00a6\u0001"+
		"\u0000\u0000\u0000\u00a6\u00a4\u0001\u0000\u0000\u0000\u00a6\u00a7\u0001"+
		"\u0000\u0000\u0000\u00a7\u00ae\u0001\u0000\u0000\u0000\u00a8\u00aa\u0005"+
		".\u0000\u0000\u00a9\u00ab\u0007\u0002\u0000\u0000\u00aa\u00a9\u0001\u0000"+
		"\u0000\u0000\u00ab\u00ac\u0001\u0000\u0000\u0000\u00ac\u00aa\u0001\u0000"+
		"\u0000\u0000\u00ac\u00ad\u0001\u0000\u0000\u0000\u00ad\u00af\u0001\u0000"+
		"\u0000\u0000\u00ae\u00a8\u0001\u0000\u0000\u0000\u00ae\u00af\u0001\u0000"+
		"\u0000\u0000\u00afB\u0001\u0000\u0000\u0000\u00b0\u00b2\u0007\u0003\u0000"+
		"\u0000\u00b1\u00b0\u0001\u0000\u0000\u0000\u00b2\u00b3\u0001\u0000\u0000"+
		"\u0000\u00b3\u00b1\u0001\u0000\u0000\u0000\u00b3\u00b4\u0001\u0000\u0000"+
		"\u0000\u00b4\u00b5\u0001\u0000\u0000\u0000\u00b5\u00b6\u0006!\u0000\u0000"+
		"\u00b6D\u0001\u0000\u0000\u0000\u0006\u0000\u00a0\u00a6\u00ac\u00ae\u00b3"+
		"\u0001\u0006\u0000\u0000";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}