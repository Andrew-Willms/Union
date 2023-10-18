using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AnalyzerTests;



[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AnalyzerTestsAnalyzer : DiagnosticAnalyzer {

	public const string DiagnosticId = "AnalyzerTests";

	private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
	private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
	private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
	private const string Category = "Naming";

	private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category,
		DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

	public override void Initialize(AnalysisContext context) {

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchExpression, SyntaxKind.SwitchStatement);
	}

	private static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext) {

		if (syntaxNodeAnalysisContext.Node is not SwitchStatementSyntax switchStatement) {
			throw new InvalidOperationException();
		}

		ExpressionSyntax switchStatementExpression = switchStatement.Expression;
		SemanticModel semanticModel = syntaxNodeAnalysisContext.SemanticModel;

		ITypeSymbol expressionReturnType = semanticModel.GetTypeInfo(switchStatementExpression).Type;
		INamedTypeSymbol objectType = semanticModel.Compilation.GetTypeByMetadataName(typeof(object).FullName);

		if (!expressionReturnType.Equals(objectType)) {
			return;
		}

		SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

		SyntaxList<SwitchLabelSyntax> labels = sections[0].Labels;

		//labels[0].SyntaxTree;

		SyntaxNode node = syntaxNodeAnalysisContext.Node;

		Compilation compilation = syntaxNodeAnalysisContext.Compilation;

	}

}