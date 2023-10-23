using System;

namespace Union.SourceGenerator;



[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class GenerateUnionAttribute : Attribute;