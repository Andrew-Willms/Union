using Microsoft.CodeAnalysis;

namespace Union.SourceGenerator; 



public class DiagnosticDescriptors {

	public static readonly DiagnosticDescriptor TestError = new(
		id: "UnionGenerator_0",
		title: "Class must be top level",
		messageFormat: "Class '{0}' using OneOfGenerator must be top level",
		category: "UnionGenerator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "",
		helpLinkUri: "",
		customTags: new[] { "" });

	// todo: add support for nested union classes
	public static readonly DiagnosticDescriptor UnionClassMustBeNestedError = new(
		id: "UnionGenerator_1",
		title: "Union class must not be a nested class",
		messageFormat: "Class '{0}' using UnionGeneratorAttribute not be a nested class",
		category: "UnionGenerator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "",
		helpLinkUri: "",
		customTags: new[] { "" });

	public static readonly DiagnosticDescriptor UnionClassMustInheritFromUnion = new(
		id: "UnionGenerator_2",
		title: "Union class must inherit from Union",
		messageFormat: "Class '{0}' does not inherit from Union",
		category: "UnionGenerator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "",
		helpLinkUri: "",
		customTags: new[] { "" });

	public static readonly DiagnosticDescriptor ObjectIsOneOfType = new(
		id: "UnionGenerator_3",
		title: "Object is not a valid type parameter",
		messageFormat: "Defined conversions to or from a base type are not allowed for class '{0}'",
		category: "UnionGenerator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "",
		helpLinkUri: "",
		customTags: new[] { "" });

	public static readonly DiagnosticDescriptor UserDefinedConversionsToOrFromAnInterfaceAreNotAllowed = new(
		id: "UnionGenerator_4",
		title: "User-defined conversions to or from an interface are not allowed",
		messageFormat: "User-defined conversions to or from an interface are not allowed",
		category: "UnionGenerator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "",
		helpLinkUri: "",
		customTags: new[] { "" });

}