using System;

namespace Union.SourceGenerator;



// todo: link to do the union nuget package once I publish it
/// <summary>
/// Add this attribute above a class inheriting from Union to source generate the necessary constructor and implicit conversions operators./>
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class GenerateUnionAttribute : Attribute;