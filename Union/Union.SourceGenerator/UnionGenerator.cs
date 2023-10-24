﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Union.SourceGenerator;



[Generator]
public class UnionGenerator : IIncrementalGenerator {

	private static readonly string AttributeName = typeof(GenerateUnionAttribute).FullName!;

	public void Initialize(IncrementalGeneratorInitializationContext context) {

		IncrementalValuesProvider<INamedTypeSymbol> classDeclarations = context.SyntaxProvider
			.CreateSyntaxProvider(
				predicate: static (syntaxNode, _) => SyntaxFilter(syntaxNode),
				transform: static (syntaxContext, _) => SyntaxToSymbol(syntaxContext))
			.Where(static classDeclarationSyntax => classDeclarationSyntax is not null)!;

		IncrementalValueProvider<(Compilation, ImmutableArray<INamedTypeSymbol>)> unionClasses =
			context.CompilationProvider.Combine(classDeclarations.Collect());

		context.RegisterSourceOutput(unionClasses, Execute);
	}

	private static bool SyntaxFilter(SyntaxNode node) {
		return node is ClassDeclarationSyntax { AttributeLists.Count: > 0, BaseList.Types.Count: > 0 } classDeclaration 
		       && classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword);
	}

	private static INamedTypeSymbol? SyntaxToSymbol(GeneratorSyntaxContext context) {

		ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

		INamedTypeSymbol? typeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);

		AttributeData ? attributeData = typeSymbol?.GetAttributes().FirstOrDefault(ad => string.Equals(
			ad.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), $"global::{AttributeName}"));

		return attributeData is null ? null : typeSymbol;
	}



	private static void Execute(SourceProductionContext context,
		(Compilation compilation, ImmutableArray<INamedTypeSymbol> classes) source) {

		if (source.classes.IsEmpty) {
			return;
		}

		IEnumerable<INamedTypeSymbol> distinctClasses = source.classes.Distinct();
		List<string> fileNames = new();

		foreach (INamedTypeSymbol typeSymbol in distinctClasses) {

			string? classSource = ProcessClass(typeSymbol, context);

			if (classSource is null) {
				continue;
			}

			// todo improve the filename-uniqueifying (currently messing with file extensions)
			string fileName = $"{typeSymbol.ContainingNamespace}_{typeSymbol.Name}.g.cs";

			while (fileNames.Contains(fileName)) {

				fileName += "1";
			}

			fileNames.Add(fileName);
			context.AddSource(fileName, classSource);
		}
	}

	private static string? ProcessClass(INamedTypeSymbol classSymbol, SourceProductionContext context) {

		Location? attributeLocation = classSymbol.Locations.FirstOrDefault() ?? Location.None;

		if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default)) {
			CreateDiagnosticError(DiagnosticDescriptors.UnionClassMustBeNestedError);
			return null;
		}

		if (classSymbol.BaseType?.Name is not "Union" || classSymbol.BaseType.ContainingNamespace.ToString() is not "Union") {
			CreateDiagnosticError(DiagnosticDescriptors.UnionClassMustInheritFromUnion);
			return null;
		}

		ImmutableArray<ITypeSymbol> typeArguments = classSymbol.BaseType.TypeArguments;

		foreach (ITypeSymbol? typeSymbol in typeArguments) {

			if (typeSymbol.Name == nameof(Object)) {
				CreateDiagnosticError(DiagnosticDescriptors.ObjectIsOneOfType);
				return null;
			}

			if (typeSymbol.TypeKind is TypeKind.Interface) {
				CreateDiagnosticError(DiagnosticDescriptors.UserDefinedConversionsToOrFromAnInterfaceAreNotAllowed);
				return null;
			}
		}

		return GenerateClassSource(classSymbol, classSymbol.BaseType);

		void CreateDiagnosticError(DiagnosticDescriptor descriptor) {
			context.ReportDiagnostic(Diagnostic.Create(descriptor, attributeLocation, classSymbol.Name,
				DiagnosticSeverity.Error));
		}
	}

	private static string GenerateClassSource(INamedTypeSymbol classSymbol, INamedTypeSymbol baseClassSymbol) {

		string className = ConcreteName(classSymbol);
		string baseClassName = ConcreteName(baseClassSymbol);

		string implicitOperators = baseClassSymbol.TypeArguments
			.Select(x => $"public static implicit operator {className}({x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} value) => new(value);")
			.Join("\r\n    \r\n    ");

		return new StringBuilder(
			$$"""
			  // <auto-generated />
			  using Union;
			  
			  #pragma warning disable 1591

			  namespace {{classSymbol.ContainingNamespace.ToDisplayString()}};
			  
			  
			  
			  partial class {{className}} {
			  
			      public {{classSymbol.Name}}({{baseClassName}} value) : base(value) { }
			      
			      {{implicitOperators}}
			      
			  }
			  """).ToString();
	}

	// todo benchmark different ways of building the string
	private static string ConcreteName(INamedTypeSymbol typeSymbol) {

		return typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

		if (!typeSymbol.TypeArguments.Any()) {
			return typeSymbol.Name;
		}

		StringBuilder stringBuilder = new(typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) + "<");

		foreach (ITypeSymbol typeArgument in typeSymbol.TypeArguments) {

			stringBuilder.Append(typeArgument.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
			stringBuilder.Append(", ");
		}

		stringBuilder.Append(">");
		return stringBuilder.ToString();
	}

}



public static class CollectionExtensions {

	public static string Join(this IEnumerable<string> array, string separator) {

		return string.Join(separator, array);
	}

}